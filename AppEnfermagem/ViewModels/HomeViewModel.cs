using AppEnfermagem.Models;
using AppEnfermagem.Services;
using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppEnfermagem.ViewModels;

// (Mantenha a classe TopicPage como estava)
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

    public ObservableCollection<TopicPage> PaginasDeTopicos { get; } = new();

    public ObservableCollection<Article> ArtigosExibidos { get; } = new();

    // Coleção para as imagens do tópico
    public ObservableCollection<TopicImage> ImagensExibidas { get; } = new();

    private List<TopicUiModel> _todosTopicos = new();

    private List<Article> _todosArtigosDaApi = new();

    [ObservableProperty] private bool isLoading;

    [ObservableProperty] private bool isVisible = true;

    [ObservableProperty] private bool isVisibleArtigos = true;

    [ObservableProperty] private bool isVisibleResultado = false;

    [ObservableProperty] private bool isVisibleAlgo = false;

    [ObservableProperty] private bool isVisibleBotaoLimpar = false;

    [ObservableProperty] private string input;

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

    public HomeViewModel(IContentService contentService)
    {
        _contentService = contentService;
    }

    [RelayCommand]
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
        foreach (var item in _todosTopicos)
        {
            item.IsSelected = false;
        }

        if (itemSelecionado != null)
        {
            itemSelecionado.IsSelected = true;

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
    public void Pesquisar()
    {
        MudarParaArtigos();

        ArtigosExibidos.Clear();

        var inputPesquisa = Input?.Trim();

        if (string.IsNullOrEmpty(inputPesquisa))
        {
            IsVisible = false;
            IsVisibleBotaoLimpar = true;

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

            IsVisible = false;

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

        // Atualiza o estado de vazio também na pesquisa
        IsArticlesEmpty = ArtigosExibidos.Count == 0;
    }

    [RelayCommand]
    public void LimparPesquisa()
    {
        ArtigosExibidos.Clear();
        Input = "";

        if (_topicoSelecionadoAtual != null)
        {
            FiltrarArtigosPorTopico(_topicoSelecionadoAtual.TopicData.TopicID);

            // Recalcula vazio para o tópico atual
            IsArticlesEmpty = ArtigosExibidos.Count == 0;
        }
        else
        {
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);
            IsArticlesEmpty = ArtigosExibidos.Count == 0;
        }

        IsVisible = true;
        IsVisibleArtigos = true;
        IsVisibleResultado = false;
        IsVisibleBotaoLimpar = false;

        MudarParaArtigos();
    }

    private void FiltrarArtigosPorTopico(int topicId)
    {
        ArtigosExibidos.Clear();

        var artigosFiltrados = _todosArtigosDaApi.Where(a => a.TopicID == topicId).ToList();

        foreach (var artigo in artigosFiltrados)
        {
            ArtigosExibidos.Add(artigo);
        }
    }

    public async Task InicializarTela()
    {
        IsLoading = true;

        try
        {
            var artigoItem = await _contentService.ObterTopicosAsync();

            PaginasDeTopicos.Clear();
            _todosTopicos.Clear();
            _todosArtigosDaApi.Clear();

            if (artigoItem != null)
            {
                if (artigoItem.Artigos != null)
                {
                    _todosArtigosDaApi.AddRange(artigoItem.Artigos);
                }

                if (artigoItem.Topicos != null)
                {
                    foreach (var topico in artigoItem.Topicos)
                    {
                        _todosTopicos.Add(new TopicUiModel(topico));
                    }

                    for (int i = 0; i < _todosTopicos.Count; i += 3)
                    {
                        var grupo = _todosTopicos.Skip(i).Take(3);
                        PaginasDeTopicos.Add(new TopicPage(grupo));
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
    }

    [RelayCommand]
    public async Task IrLogin()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}