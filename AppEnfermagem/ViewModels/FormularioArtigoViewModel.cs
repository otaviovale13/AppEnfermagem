using AppEnfermagem.Models;
using AppEnfermagem.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppEnfermagem.ViewModels;

[QueryProperty(nameof(Artigo), "ArtigoObjeto")]
public partial class FormularioArtigoViewModel : ObservableObject
{
    private readonly IContentService _contentService;

    [ObservableProperty] private Article artigo;
    [ObservableProperty] private string tituloPagina = "Novo Artigo";
    [ObservableProperty] private Topic topicoSelecionado;

    public ObservableCollection<Topic> ListaTopicos { get; } = new();

    public FormularioArtigoViewModel(IContentService contentService)
    {
        _contentService = contentService;
        CarregarTopicos();

        // Se o artigo não vier via QueryProperty, criamos um novo
        if (Artigo == null)
            Artigo = new Article();
    }

    private async void CarregarTopicos()
    {
        var data = await _contentService.ObterTopicosAsync();
        foreach (var t in data.Topicos) ListaTopicos.Add(t);

        // Se for edição, pré-seleciona o tópico correto
        if (Artigo?.TopicID != 0)
        {
            TopicoSelecionado = ListaTopicos.FirstOrDefault(t => t.TopicID == Artigo.TopicID);
        }
    }

    [RelayCommand]
    public async Task Salvar()
    {
        if (string.IsNullOrWhiteSpace(Artigo.Title) || TopicoSelecionado == null)
        {
            await App.Current.MainPage.DisplayAlert("Atenção", "Preencha o título e selecione um tópico.", "OK");
            return;
        }

        Artigo.TopicID = TopicoSelecionado.TopicID;

        // Mostra um alerta de carregamento se quiser ou use uma propriedade IsLoading
        bool sucesso;

        if (Artigo.ArticleID == 0)
        {
            // NOVO ARTIGO
            sucesso = await _contentService.CriarArtigoAsync(Artigo);
        }
        else
        {
            // EDIÇÃO
            sucesso = await _contentService.AtualizarArtigoAsync(Artigo);
        }

        if (sucesso)
        {
            await App.Current.MainPage.DisplayAlert("Sucesso", "Banco de dados atualizado!", "OK");
            // Volta para a Home. O ideal é que a Home recarregue os dados no OnAppearing
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Erro", "Não foi possível salvar na API.", "OK");
        }
    }
}