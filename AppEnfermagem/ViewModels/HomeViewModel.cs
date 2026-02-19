using AppEnfermagem.Models;
using AppEnfermagem.Services;
using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Xml.Linq;

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
    private TopicUiModel _topicoSelecionadoAtual;

    // --- Propriedades de Visibilidade e UI ---
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isVisible = true;
    [ObservableProperty] private bool isVisibleArtigos = true;
    [ObservableProperty] private bool isVisibleResultado = false;
    [ObservableProperty] private bool isVisibleBotaoLimpar = false;
    [ObservableProperty] private string input;
    [ObservableProperty] private bool isAdmin;
    [ObservableProperty] private string artigosText;

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

    public void VerificarStatusAdmin() => IsAdmin = !string.IsNullOrEmpty(Preferences.Get("UserId", null));

    #region NAVEGAÇÃO E MODO
    [RelayCommand]
    public void MudarParaArtigos()
    {
        ArtigosText = "Artigos escritos";
        IsModeArticles = true;
        IsModeImages = false;
        AtualizarCoresBolinhas();
    }

    [RelayCommand]
    public void MudarParaImagens()
    {
        ArtigosText = "Imagens";
        IsModeArticles = false;
        IsModeImages = true;
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
        }
        IsArticlesEmpty = ArtigosExibidos.Count == 0;
    }

    [RelayCommand]
    public void LimparPesquisa()
    {
        ArtigosExibidos.Clear();
        Input = string.Empty;

        if (_topicoSelecionadoAtual != null)
            SelecionarTopico(_topicoSelecionadoAtual);
        else
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);

        IsVisible = true;
        IsVisibleArtigos = true;
        IsVisibleResultado = false;
        IsVisibleBotaoLimpar = false;
        IsArticlesEmpty = ArtigosExibidos.Count == 0;
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

    [RelayCommand]
    public async Task DeletarArtigo(Article artigo)
    {
        if (artigo == null || !await App.Current.MainPage.DisplayAlert("Excluir", "Tem certeza?", "Sim", "Não")) return;

        if (await _contentService.DeletarArtigoAsync(artigo.ArticleID))
        {
            ArtigosExibidos.Remove(artigo);
            _todosArtigosDaApi.Remove(artigo); // <-- ESSA É A LINHA MÁGICA QUE FALTAVA
        }
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

                // 2. CORREÇÃO: Remove do cache interno do tópico
                if (_topicoSelecionadoAtual?.TopicData?.Images != null)
                {
                    _topicoSelecionadoAtual.TopicData.Images.Remove(imagem);
                }

                // 3. Atualiza o estado de lista vazia
                IsImagesEmpty = ImagensExibidas.Count == 0;
            }
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task EditarImagem(TopicImage imagem) =>
    await Shell.Current.GoToAsync(nameof(FormularioImagemPage),
        new Dictionary<string, object> { { "ImagemObjeto", imagem } });
    #endregion

    [RelayCommand]
    public async Task AbrirOpcoesTopico(TopicUiModel item)
    {
        if (item == null) return;

        // Abre um menu nativo do celular subindo pela parte de baixo da tela
        string acao = await App.Current.MainPage.DisplayActionSheet($"Opções: {item.TopicData.Name}", "Cancelar", null, "Editar", "Excluir");

        // Redireciona para os comandos que você já tem prontos!
        if (acao == "Editar")
        {
            await EditarTopico(item);
        }
        else if (acao == "Excluir")
        {
            await DeletarTopico(item);
        }
    }
}