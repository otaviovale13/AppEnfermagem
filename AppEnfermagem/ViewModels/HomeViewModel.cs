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
    private List<TopicUiModel> _todosTopicos = new();
    private List<Article> _todosArtigosDaApi = new();
    private TopicUiModel _topicoSelecionadoAtual;

    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isVisible = true;
    [ObservableProperty] private bool isVisibleArtigos = true;
    [ObservableProperty] private bool isVisibleResultado = false;
    [ObservableProperty] private bool isVisibleBotaoLimpar = false;
    [ObservableProperty] private string input;
    [ObservableProperty] private bool isAdmin;

    public HomeViewModel(IContentService contentService) => _contentService = contentService;

    public void VerificarStatusAdmin() => IsAdmin = !string.IsNullOrEmpty(Preferences.Get("UserId", null));

    [RelayCommand]
    public void SelecionarTopico(TopicUiModel itemSelecionado)
    {
        if (itemSelecionado == null) return;
        foreach (var item in _todosTopicos) item.IsSelected = false;
        itemSelecionado.IsSelected = true;
        _topicoSelecionadoAtual = itemSelecionado;
        FiltrarArtigosPorTopico(itemSelecionado.TopicData.TopicID);
    }


    [RelayCommand]
    public void Pesquisar()
    {
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
    }

    [RelayCommand]
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
                    for (int i = 0; i < _todosTopicos.Count; i += 3) PaginasDeTopicos.Add(new TopicPage(_todosTopicos.Skip(i).Take(3)));
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
            if (await Shell.Current.DisplayAlert("Logout", "Deseja sair do modo administrador?", "Sim", "Não"))
            {
                Preferences.Clear(); IsAdmin = false; await InicializarTela();
            }
        }
        else await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    [RelayCommand] public async Task AdicionarArtigo() => await Shell.Current.GoToAsync(nameof(FormularioArtigoPage));
    [RelayCommand] public async Task IrParaCriarTopico() => await Shell.Current.GoToAsync(nameof(FormularioTopicoPage));

    // 1. Limpar Pesquisa (Certifique-se de que não recebe parâmetros)
    [RelayCommand]
    public void LimparPesquisa()
    {
        ArtigosExibidos.Clear();
        Input = string.Empty;

        if (_topicoSelecionadoAtual != null)
        {
            FiltrarArtigosPorTopico(_topicoSelecionadoAtual.TopicData.TopicID);
        }
        else
        {
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);
        }

        IsVisible = true;
        IsVisibleArtigos = true;
        IsVisibleResultado = false;
        IsVisibleBotaoLimpar = false;
    }

    // 2. Editar Artigo (Precisa receber o objeto Article)
    [RelayCommand]
    public async Task EditarArtigo(Article artigo)
    {
        if (artigo == null) return;
        var parametros = new Dictionary<string, object> { { "ArtigoObjeto", artigo } };
        await Shell.Current.GoToAsync(nameof(FormularioArtigoPage), parametros);
    }

    // 3. Deletar Artigo (Precisa receber o objeto Article)
    [RelayCommand]
    public async Task DeletarArtigo(Article artigo)
    {
        if (artigo == null) return;

        bool confirmar = await App.Current.MainPage.DisplayAlert("Confirmar",
            $"Deseja excluir o artigo '{artigo.Title}'?", "Sim", "Não");

        if (confirmar)
        {
            var sucesso = await _contentService.DeletarArtigoAsync(artigo.ArticleID);
            if (sucesso)
            {
                ArtigosExibidos.Remove(artigo);
                _todosArtigosDaApi.Remove(artigo);
            }
        }
    }

    // 4. Método auxiliar que estava faltando (Adicione se ainda não tiver)
    private void FiltrarArtigosPorTopico(int topicId)
    {
        ArtigosExibidos.Clear();
        var filtrados = _todosArtigosDaApi.Where(a => a.TopicID == topicId).ToList();
        foreach (var artigo in filtrados) ArtigosExibidos.Add(artigo);
    }
}