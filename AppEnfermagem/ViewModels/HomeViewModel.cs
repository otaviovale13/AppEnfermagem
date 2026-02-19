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

    // Coleção para as imagens do tópico
    public ObservableCollection<TopicImage> ImagensExibidas { get; } = new();

    private List<TopicUiModel> _todosTopicos = new();
    private List<Article> _todosArtigosDaApi = new();
    private TopicUiModel _topicoSelecionadoAtual;

    // --- Propriedades de Visibilidade e UI ---
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isVisible = true;
    [ObservableProperty] private bool isVisibleArtigos = true;
    [ObservableProperty] private bool isVisibleResultado = false;
    [ObservableProperty] private bool isVisibleBotaoLimpar = false;
    [ObservableProperty] private string input;
    [ObservableProperty] private bool isAdmin;

    // --- Funcionalidades de Abas (Texto/Imagem) ---
    [ObservableProperty] private bool isModeArticles = true;
    [ObservableProperty] private bool isModeImages = false;
    [ObservableProperty] private bool isArticlesEmpty;
    [ObservableProperty] private bool isImagesEmpty;
    [ObservableProperty] private Color dotColorArticles = Color.FromArgb("#004AAD");
    [ObservableProperty] private Color dotColorImages = Color.FromArgb("#CCCCCC");
    // --- Propriedades de Modo (Abas) ---
    [ObservableProperty] private bool isModeArticles = true;
    [ObservableProperty] private bool isModeImages = false;

    // --- NOVAS PROPRIEDADES: Controle de Lista Vazia ---
    [ObservableProperty] private bool isArticlesEmpty;
    [ObservableProperty] private bool isImagesEmpty;

    // Cores das bolinhas
    [ObservableProperty] private Color dotColorArticles = Color.FromArgb("#004AAD");
    [ObservableProperty] private Color dotColorImages = Color.FromArgb("#CCCCCC");

    // Popup de Zoom
    [ObservableProperty] private bool isImagePopupVisible = false;
    [ObservableProperty] private TopicImage imagemSelecionadaParaZoom;

    private TopicUiModel _topicoSelecionadoAtual;

    // --- Zoom de Imagem ---
    [ObservableProperty] private bool isImagePopupVisible = false;
    [ObservableProperty] private TopicImage imagemSelecionadaParaZoom;

    public HomeViewModel(IContentService contentService) => _contentService = contentService;

    public void VerificarStatusAdmin() => IsAdmin = !string.IsNullOrEmpty(Preferences.Get("UserId", null));

    #region NAVEGAÇÃO E MODO
    [RelayCommand]
    public void MudarParaArtigos()
    {
        IsModeArticles = true;
        IsModeImages = false;
        AtualizarCoresBolinhas();
    }

    [RelayCommand]
    public void MudarParaImagens()
    public void MudarParaArtigos()
    {
        IsModeArticles = true;
        IsModeImages = false;
        AtualizarCoresBolinhas();
    }

    [RelayCommand]
    public void MudarParaImagens()
    {
        IsModeArticles = false;
        IsModeImages = true;
        AtualizarCoresBolinhas();
    }

    private void AtualizarCoresBolinhas()
    {
        if (IsModeArticles)
        {
            DotColorArticles = Color.FromArgb("#004AAD");
            DotColorImages = Color.FromArgb("#CCCCCC");
        }
        else
        {
            DotColorArticles = Color.FromArgb("#CCCCCC");
            DotColorImages = Color.FromArgb("#004AAD");
        }
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
    public void SelecionarTopico(TopicUiModel itemSelecionado)
    {
        IsModeArticles = false;
        IsModeImages = true;
        AtualizarCoresBolinhas();
    }
        foreach (var item in _todosTopicos)
        {
            item.IsSelected = false;
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
            _topicoSelecionadoAtual = itemSelecionado;

            // 1. Carregar Artigos
            FiltrarArtigosPorTopico(itemSelecionado.TopicData.TopicID);

            // Verifica se a lista de artigos ficou vazia
            IsArticlesEmpty = ArtigosExibidos.Count == 0;

            // 2. Carregar Imagens
            ImagensExibidas.Clear();
            if (itemSelecionado.TopicData.Images != null)
            {
                var imagensOrdenadas = itemSelecionado.TopicData.Images.OrderBy(x => x.DisplayOrder);
                foreach (var img in imagensOrdenadas)
                {
                    ImagensExibidas.Add(img);
                }
            }

            // Verifica se a lista de imagens ficou vazia
            IsImagesEmpty = ImagensExibidas.Count == 0;

            // Volta para a aba principal (texto)
            MudarParaArtigos();
        }
    }

    [RelayCommand]
    public void FecharImagem()
    public void Pesquisar()
    {
        IsImagePopupVisible = false;
        ImagemSelecionadaParaZoom = null;
    }
    #endregion
        MudarParaArtigos();

        ArtigosExibidos.Clear();

    #region GERENCIAMENTO DE TÓPICOS
    [RelayCommand]
    public void SelecionarTopico(TopicUiModel itemSelecionado)
    {
        if (itemSelecionado == null) return;
        var inputPesquisa = Input?.Trim();

        if (string.IsNullOrEmpty(inputPesquisa))
        {
            IsVisible = false;
            IsVisibleBotaoLimpar = true;

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
            foreach (var artigo in _todosArtigosDaApi)
            {
                ArtigosExibidos.Add(artigo);
            }
        }
        else
        {
            var artigosFiltrados = _todosArtigosDaApi
                .Where(a => a.Title != null &&
                            a.Title.Contains(inputPesquisa, StringComparison.OrdinalIgnoreCase))
                .ToList();

    #region PESQUISA
    [RelayCommand]
    public void Pesquisar()
    {
        MudarParaArtigos(); // Pesquisa sempre foca em texto
        ArtigosExibidos.Clear();
        var inputPesquisa = Input?.Trim();

        if (string.IsNullOrEmpty(inputPesquisa))
        {
            IsVisible = false;
            IsVisibleBotaoLimpar = true;
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);
        }
        else
        {
            var filtrados = _todosArtigosDaApi.Where(a => a.Title != null && a.Title.Contains(inputPesquisa, StringComparison.OrdinalIgnoreCase)).ToList();
            IsVisible = false;
            IsVisibleBotaoLimpar = true;
            IsVisibleArtigos = filtrados.Any();
            IsVisibleResultado = !filtrados.Any();
            foreach (var artigo in filtrados) ArtigosExibidos.Add(artigo);
            if (artigosFiltrados.Count == 0)
            {
                IsVisibleArtigos = false;
                IsVisibleBotaoLimpar = true;
                IsVisibleResultado = true; // "Nada foi encontrado" (Pesquisa)
            }
            else
            {
                IsVisibleBotaoLimpar = true;
                IsVisibleArtigos = true;
                IsVisibleResultado = false;

                foreach (var artigo in artigosFiltrados)
                {
                    ArtigosExibidos.Add(artigo);
                }
            }
        }
        IsArticlesEmpty = ArtigosExibidos.Count == 0;

        // Atualiza o estado de vazio também na pesquisa
        IsArticlesEmpty = ArtigosExibidos.Count == 0;
    }

    [RelayCommand]
    public void LimparPesquisa()
    {
        ArtigosExibidos.Clear();
        Input = string.Empty;
        Input = "";

        if (_topicoSelecionadoAtual != null)
            SelecionarTopico(_topicoSelecionadoAtual);
        {
            FiltrarArtigosPorTopico(_topicoSelecionadoAtual.TopicData.TopicID);

            // Recalcula vazio para o tópico atual
            IsArticlesEmpty = ArtigosExibidos.Count == 0;
        }
        else
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);

        IsVisible = true;
        IsVisibleArtigos = true;
        IsVisibleResultado = false;
        IsVisibleBotaoLimpar = false;
        IsArticlesEmpty = ArtigosExibidos.Count == 0;

        MudarParaArtigos();
    }
    #endregion

    #region CRUD E ADMINISTRAÇÃO
    public async Task InicializarTela()
    {
        try
        {
            IsLoading = true;
            var data = await _contentService.ObterTopicosAsync();
            PaginasDeTopicos.Clear(); _todosTopicos.Clear(); _todosArtigosDaApi.Clear();
    private void FiltrarArtigosPorTopico(int topicId)
    {
        ArtigosExibidos.Clear();

        var artigosFiltrados = _todosArtigosDaApi.Where(a => a.TopicID == topicId).ToList();

            if (data != null)
            {
                if (data.Artigos != null) _todosArtigosDaApi.AddRange(data.Artigos);
                if (data.Topicos != null)
                {
                    foreach (var t in data.Topicos) _todosTopicos.Add(new TopicUiModel(t));
                    for (int i = 0; i < _todosTopicos.Count; i += 3)
                        PaginasDeTopicos.Add(new TopicPage(_todosTopicos.Skip(i).Take(3)));

                    if (_todosTopicos.Any()) SelecionarTopico(_todosTopicos[0]);
                }
            }
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task AdminIconClick()
    {
        if (IsAdmin)
        {
            if (await Shell.Current.DisplayAlert("Logout", "Deseja sair?", "Sim", "Não"))
            {
                Preferences.Clear(); IsAdmin = false; await InicializarTela();
            }
        }
        else await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    [RelayCommand]
    public async Task SelecionarTipoPostagem()
    {
        string acao = await App.Current.MainPage.DisplayActionSheet("Postar:", "Cancelar", null, "Texto", "Imagem");
        if (acao == "Texto") await Shell.Current.GoToAsync(nameof(FormularioArtigoPage));
        else if (acao == "Imagem") await Shell.Current.GoToAsync(nameof(FormularioImagemPage));
    }

    [RelayCommand] public async Task IrParaCriarTopico() => await Shell.Current.GoToAsync(nameof(FormularioTopicoPage));
    [RelayCommand] public async Task EditarArtigo(Article artigo) => await Shell.Current.GoToAsync(nameof(FormularioArtigoPage), new Dictionary<string, object> { { "ArtigoObjeto", artigo } });
    [RelayCommand] public async Task EditarTopico(TopicUiModel item) => await Shell.Current.GoToAsync(nameof(FormularioTopicoPage), new Dictionary<string, object> { { "TopicoObjeto", item.TopicData } });
            PaginasDeTopicos.Clear();
            _todosTopicos.Clear();
            _todosArtigosDaApi.Clear();

    [RelayCommand]
    public async Task DeletarArtigo(Article artigo)
    {
        if (artigo == null || !await App.Current.MainPage.DisplayAlert("Excluir", "Tem certeza?", "Sim", "Não")) return;
        if (await _contentService.DeletarArtigoAsync(artigo.ArticleID)) ArtigosExibidos.Remove(artigo);
    }
            if (artigoItem != null)
            {
                if (artigoItem.Artigos != null)
                {
                    _todosArtigosDaApi.AddRange(artigoItem.Artigos);
                }

    [RelayCommand]
    public async Task DeletarTopico(TopicUiModel item)
    {
        if (item == null || !await App.Current.MainPage.DisplayAlert("Excluir", "Apagar categoria e tudo nela?", "Sim", "Não")) return;
        if (await _contentService.DeletarTopicoAsync(item.TopicData.TopicID)) await InicializarTela();
    }

    // Novos comandos para imagens se precisar editar/excluir individualmente
    [RelayCommand]
    public async Task DeletarImagem(TopicImage imagem)
    {
        if (imagem == null || !await App.Current.MainPage.DisplayAlert("Excluir", "Remover esta imagem?", "Sim", "Não")) return;

        IsLoading = true;
        try
        {
            var sucesso = await _contentService.DeletarImagemTopicoAsync(imagem.ImageId);
            if (sucesso)
            {
                // 1. Remove da lista que está na tela agora
                ImagensExibidas.Remove(imagem);
                    for (int i = 0; i < _todosTopicos.Count; i += 3)
                    {
                        var grupo = _todosTopicos.Skip(i).Take(3);
                        PaginasDeTopicos.Add(new TopicPage(grupo));
                    }

                // 2. CORREÇÃO: Remove do cache interno do tópico
                if (_topicoSelecionadoAtual?.TopicData?.Images != null)
                {
                    _topicoSelecionadoAtual.TopicData.Images.Remove(imagem);
                }

                // 3. Atualiza o estado de lista vazia
                IsImagesEmpty = ImagensExibidas.Count == 0;
            }
                    if (_todosTopicos.Count > 0)
                    {
                        var primeiroTopico = _todosTopicos[0];
                        SelecionarTopico(primeiroTopico);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task EditarImagem(TopicImage imagem) =>
    await Shell.Current.GoToAsync(nameof(FormularioImagemPage),
        new Dictionary<string, object> { { "ImagemObjeto", imagem } });
    #endregion
}