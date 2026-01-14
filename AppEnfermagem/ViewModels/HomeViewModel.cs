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

    // Lista para o Carrossel
    public ObservableCollection<TopicPage> PaginasDeTopicos { get; } = new();

    // 1. NOVA PROPRIEDADE: Lista de artigos que aparece na tela (Filtrada)
    public ObservableCollection<Article> ArtigosExibidos { get; } = new();

    private List<TopicUiModel> _todosTopicos = new();

    // 2. NOVA LISTA PRIVADA: Cache de TODOS os artigos vindos da API
    private List<Article> _todosArtigosDaApi = new();

    [ObservableProperty] private bool isLoading;

    [ObservableProperty] private bool isVisible = true;

    [ObservableProperty] private bool isVisibleArtigos = true;

    [ObservableProperty] private bool isVisibleResultado = false;

    [ObservableProperty] private bool isVisibleBotaoLimpar = false;

    [ObservableProperty] private string input;

    // Variável para lembrar qual tópico está pintado de Ciano/Selecionado atualmente
    private TopicUiModel _topicoSelecionadoAtual;

    public HomeViewModel(IContentService contentService)
    {
        _contentService = contentService;
    }

    [RelayCommand]
    public void SelecionarTopico(TopicUiModel itemSelecionado)
    {
        // Lógica visual existente (pinta/despinta)
        foreach (var item in _todosTopicos)
        {
            item.IsSelected = false;
        }

        if (itemSelecionado != null)
        {
            itemSelecionado.IsSelected = true;

            // --- ADICIONE ISSO AQUI: ---
            _topicoSelecionadoAtual = itemSelecionado; // Salva na memória
                                                       // ---------------------------

            FiltrarArtigosPorTopico(itemSelecionado.TopicData.TopicID);
        }
    }

    [RelayCommand]
    public void Pesquisar() // Não precisa ser async, pois não vamos chamar a API
    {
        // 1. Limpa apenas a lista visual
        ArtigosExibidos.Clear();

        // 2. Pega o texto digitado
        var inputPesquisa = Input?.Trim();

        // Cenário 1: Campo vazio (Mostra tudo ou reseta o estado)
        if (string.IsNullOrEmpty(inputPesquisa))
        {
            IsVisible = false;      // Esconde o carrossel (se for essa a intenção)
            IsVisibleBotaoLimpar = true;

            // Restaura todos os artigos que já estavam na memória
            foreach (var artigo in _todosArtigosDaApi)
            {
                ArtigosExibidos.Add(artigo);
            }
        }
        // Cenário 2: Tem texto (Filtra a lista existente)
        else
        {
            // MELHORIA: Usamos .Contains e Ignoramos Maiúsculas/Minúsculas
            // Antes estava (a.Title == inputPesquisa), que obrigava a digitar exatamente igual.
            var artigosFiltrados = _todosArtigosDaApi
                .Where(a => a.Title != null &&
                            a.Title.Contains(inputPesquisa, StringComparison.OrdinalIgnoreCase))
                .ToList();

            IsVisible = false;

            if (artigosFiltrados.Count == 0)
            {
                // Nenhum resultado encontrado
                IsVisibleArtigos = false;
                IsVisibleBotaoLimpar = true;
                IsVisibleResultado = true; // Mostra mensagem de "Nada encontrado"
            }
            else
            {
                // Encontrou resultados
                IsVisibleBotaoLimpar = true;
                IsVisibleArtigos = true;
                IsVisibleResultado = false;

                foreach (var artigo in artigosFiltrados)
                {
                    ArtigosExibidos.Add(artigo);
                }
            }
        }
    }

    [RelayCommand]
    public void LimparPesquisa() // Remova o (TopicUiModel itemSelecionado)
    {
        ArtigosExibidos.Clear();
        Input = ""; // Limpa o texto do Entry

        // Usa a variável que salvamos na memória
        if (_topicoSelecionadoAtual != null)
        {
            FiltrarArtigosPorTopico(_topicoSelecionadoAtual.TopicData.TopicID);
        }
        else
        {
            // Caso de segurança (se nada estiver selecionado, restaura tudo)
            foreach (var artigo in _todosArtigosDaApi) ArtigosExibidos.Add(artigo);
        }

        // Restaura a visualização
        IsVisible = true;
        IsVisibleArtigos = true;
        IsVisibleResultado = false;
        IsVisibleBotaoLimpar = false;
    }

    // Método auxiliar para filtrar
    private void FiltrarArtigosPorTopico(int topicId)
    {
        ArtigosExibidos.Clear();

        // Pega apenas os artigos onde o TopicID é igual ao do tópico clicado
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
            _todosArtigosDaApi.Clear(); // Limpa cache anterior

            if (artigoItem != null)
            {
                // 4. Salva todos os artigos recebidos na lista privada
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

                    // Lógica de Paginação do Carrossel (Mantida igual)
                    for (int i = 0; i < _todosTopicos.Count; i += 3)
                    {
                        var grupo = _todosTopicos.Skip(i).Take(3);
                        PaginasDeTopicos.Add(new TopicPage(grupo));
                    }

                    // Dentro do InicializarTela...
                    if (_todosTopicos.Count > 0)
                    {
                        var primeiroTopico = _todosTopicos[0];
                        SelecionarTopico(primeiroTopico); // Isso agora vai salvar na variavel _topicoSelecionadoAtual
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