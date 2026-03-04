using AppEnfermagem.Models;
using AppEnfermagem.Services;
using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppEnfermagem.ViewModels;

public class TopicPage
{
    public List<TopicUiModel> ItensDaPagina { get; set; }
    public TopicPage(IEnumerable<TopicUiModel> items)
    {
        ItensDaPagina = new List<TopicUiModel>(items);
    }
}

public partial class HomeViewModel : ObservableObject
{
    private readonly IContentService _contentService;

    // --- Coleções ---
    public ObservableCollection<TopicPage> PaginasDeTopicos { get; } = new();
    public ObservableCollection<Article> ArtigosExibidos { get; } = new();
    public ObservableCollection<TopicImage> ImagensExibidas { get; } = new();

    private List<TopicUiModel> _todosTopicos = new();
    private List<Article> _todosArtigosDaApi = new();
    private List<TopicImage> _todasImagensDaApi = new();
    private TopicUiModel _topicoSelecionadoAtual;

    // --- Propriedades de Visibilidade e UI ---
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isVisible = true;
    [ObservableProperty] private bool isVisibleArtigos = true;
    [ObservableProperty] private bool isVisibleResultado = false;
    [ObservableProperty] private bool isVisibleBotaoLimpar = false;
    [ObservableProperty] private string input;
    [ObservableProperty] private string artigosText;
    [ObservableProperty] private bool artigosTextVisiblie = true;

    // --- Funcionalidades de Abas (Texto/Imagem) ---
    [ObservableProperty] private bool isModeArticles = true;
    [ObservableProperty] private bool isModeImages = false;
    [ObservableProperty] private bool isArticlesEmpty;
    [ObservableProperty] private bool isImagesEmpty;
    [ObservableProperty] private Color dotColorArticles = Color.FromArgb("#004AAD");
    [ObservableProperty] private Color dotColorImages = Color.FromArgb("#CCCCCC");

    // --- Zoom de Imagem ---
    [ObservableProperty] private bool isImagePopupVisible = false;
    [ObservableProperty] private TopicImage imagemSelecionadaParaZoom;

    public HomeViewModel(IContentService contentService) => _contentService = contentService;

    #region NAVEGAÇÃO E MODO
    [RelayCommand]
    public void MudarParaArtigos()
    {
        ArtigosText = "Artigos escritos"; // Volta o título original
        IsModeArticles = true;
        IsModeImages = false; // Esconde as imagens para mostrar só texto
        AtualizarCoresBolinhas();
    }

    [RelayCommand]
    public void MudarParaImagens()
    {
        ArtigosText = "Imagens"; // Muda o título
        IsModeArticles = false; // Esconde os textos
        IsModeImages = true; // Mostra só as imagens
        AtualizarCoresBolinhas();
    }

    private void AtualizarCoresBolinhas()
    {
        DotColorArticles = IsModeArticles ? Color.FromArgb("#004AAD") : Color.FromArgb("#CCCCCC");
        DotColorImages = IsModeImages ? Color.FromArgb("#004AAD") : Color.FromArgb("#CCCCCC");
    }

    [RelayCommand]
    public void AbrirImagem(TopicImage img)
    {
        if (img != null)
        {
            ImagemSelecionadaParaZoom = img;
            IsImagePopupVisible = true;
        }
    }

    [RelayCommand]
    public void FecharImagem()
    {
        IsImagePopupVisible = false;
        ImagemSelecionadaParaZoom = null;
    }

    [RelayCommand]
    public async Task Voltar()
    {
        await Shell.Current.GoToAsync($"//{nameof(OpcoesPage)}");
    }

    [RelayCommand]
    public async Task BaixarImagem()
    {
        if (ImagemSelecionadaParaZoom == null || string.IsNullOrEmpty(ImagemSelecionadaParaZoom.ImageUrl))
            return;

        try
        {
            IsLoading = true;

            // 1. Faz o download da imagem da internet
            using var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(ImagemSelecionadaParaZoom.ImageUrl);

            // 2. Cria um ficheiro temporário no telemóvel
            string extensao = ImagemSelecionadaParaZoom.ImageUrl.EndsWith(".png") ? ".png" : ".jpg";
            string fileName = $"enfermagem_{DateTime.Now:yyyyMMdd_HHmmss}{extensao}";
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(localFilePath, imageBytes);

            // 3. Abre o menu nativo do telemóvel para "Guardar na Galeria" ou "Compartilhar"
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Guardar ou Compartilhar Imagem",
                File = new ShareFile(localFilePath)
            });
        }
        catch (Exception)
        {
            await App.Current.MainPage.DisplayAlert("Erro", "Não foi possível transferir a imagem. Verifique a sua ligação.", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
    #endregion

    #region GERENCIAMENTO DE TÓPICOS
    [RelayCommand]
    public void SelecionarTopico(TopicUiModel itemSelecionado)
    {
        if (itemSelecionado == null) return;

        foreach (var item in _todosTopicos) item.IsSelected = false;
        itemSelecionado.IsSelected = true;
        _topicoSelecionadoAtual = itemSelecionado;

        // 1. Filtra Artigos
        FiltrarArtigosPorTopico(itemSelecionado.TopicData.TopicID);
        IsArticlesEmpty = ArtigosExibidos.Count == 0;

        // 2. Filtra Imagens
        ImagensExibidas.Clear();
        if (itemSelecionado.TopicData.Images != null)
        {
            var imagensOrdenadas = itemSelecionado.TopicData.Images.OrderBy(x => x.DisplayOrder);
            foreach (var img in imagensOrdenadas) ImagensExibidas.Add(img);
        }
        IsImagesEmpty = ImagensExibidas.Count == 0;

        // Reseta para aba de texto ao trocar tópico
        MudarParaArtigos();
    }

    private void FiltrarArtigosPorTopico(int topicId)
    {
        ArtigosExibidos.Clear();
        var filtrados = _todosArtigosDaApi.Where(a => a.TopicID == topicId).ToList();
        foreach (var artigo in filtrados) ArtigosExibidos.Add(artigo);
    }
    #endregion

    #region PESQUISA
    [RelayCommand]
    public void Pesquisar()
    {
        var inputPesquisa = Input?.Trim();

        // 1. PESQUISA VAZIA -> MOSTRA TODO O CATÁLOGO
        if (string.IsNullOrEmpty(inputPesquisa))
        {
            ArtigosText = "Todos os artigos";
            ArtigosTextVisiblie = true;

            IsModeArticles = true;
            IsModeImages = true;
            DotColorArticles = Color.FromArgb("#004AAD");
            DotColorImages = Color.FromArgb("#004AAD");

            ArtigosExibidos.Clear();
            ImagensExibidas.Clear();

            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);
            var imagensOrdenadas = _todasImagensDaApi.OrderBy(x => x.DisplayOrder);
            foreach (var img in imagensOrdenadas) ImagensExibidas.Add(img);

            IsVisible = false;
            IsVisibleBotaoLimpar = true;

            IsVisibleArtigos = true;
            IsVisibleResultado = false;
            IsArticlesEmpty = false;
            IsImagesEmpty = false;

            return;
        }

        // --- 2. PESQUISA COM TEXTO ---
        ArtigosExibidos.Clear();
        ImagensExibidas.Clear();

        // Filtra Artigos
        var filtradosArtigos = _todosArtigosDaApi
            .Where(a => !string.IsNullOrEmpty(a.Title) && a.Title.Contains(inputPesquisa, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Filtra Imagens
        var filtradasImagens = _todasImagensDaApi
            .Where(img => !string.IsNullOrEmpty(img.Caption) && img.Caption.Contains(inputPesquisa, StringComparison.OrdinalIgnoreCase))
            .ToList();

        IsVisible = false;
        IsVisibleBotaoLimpar = true;

        bool achouAlgo = filtradosArtigos.Any() || filtradasImagens.Any();

        IsVisibleResultado = !achouAlgo;
        ArtigosTextVisiblie = achouAlgo;

        foreach (var artigo in filtradosArtigos) ArtigosExibidos.Add(artigo);
        foreach (var img in filtradasImagens) ImagensExibidas.Add(img);

        if (!achouAlgo)
        {
            IsArticlesEmpty = false;
            IsImagesEmpty = false;
        }
        else
        {
            IsArticlesEmpty = !filtradosArtigos.Any();
            IsImagesEmpty = !filtradasImagens.Any();
        }

        if (filtradosArtigos.Any() && filtradasImagens.Any())
        {
            ArtigosText = "Resultados da pesquisa";
            IsModeArticles = true;
            IsModeImages = true;
            DotColorArticles = Color.FromArgb("#004AAD");
            DotColorImages = Color.FromArgb("#004AAD");
        }
        else if (filtradosArtigos.Any())
        {
            MudarParaArtigos();
            ArtigosText = "Resultados da pesquisa";
        }
        else if (filtradasImagens.Any())
        {
            MudarParaImagens();
            ArtigosText = "Resultados da pesquisa";
        }
    }

    [RelayCommand]
    public void LimparPesquisa()
    {
        ArtigosExibidos.Clear();
        ImagensExibidas.Clear();

        Input = string.Empty;

        if (_topicoSelecionadoAtual != null)
        {
            SelecionarTopico(_topicoSelecionadoAtual);
        }
        else
        {
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);
            foreach (var imagem in _todasImagensDaApi) ImagensExibidas.Add(imagem);
        }

        IsVisible = true;
        IsVisibleArtigos = true;
        IsVisibleResultado = false;
        IsVisibleBotaoLimpar = false;
        ArtigosTextVisiblie = true;

        IsArticlesEmpty = ArtigosExibidos.Count == 0;
    }
    #endregion

    #region INICIALIZAÇÃO E CARREGAMENTO
    public async Task InicializarTela()
    {
        try
        {
            IsLoading = true;
            var data = await _contentService.ObterTopicosAsync();

            PaginasDeTopicos.Clear();
            _todosTopicos.Clear();
            _todosArtigosDaApi.Clear();
            _todasImagensDaApi.Clear();

            if (data != null)
            {
                if (data.Artigos != null)
                    _todosArtigosDaApi.AddRange(data.Artigos);

                if (data.Topicos != null)
                {
                    foreach (var t in data.Topicos)
                    {
                        _todosTopicos.Add(new TopicUiModel(t));

                        if (t.Images != null)
                        {
                            _todasImagensDaApi.AddRange(t.Images);
                        }
                    }

                    for (int i = 0; i < _todosTopicos.Count; i += 3)
                        PaginasDeTopicos.Add(new TopicPage(_todosTopicos.Skip(i).Take(3)));

                    if (_todosTopicos.Any())
                    {
                        SelecionarTopico(_todosTopicos[0]);
                    }
                }
            }
        }
        finally { IsLoading = false; }
    }
    #endregion
}