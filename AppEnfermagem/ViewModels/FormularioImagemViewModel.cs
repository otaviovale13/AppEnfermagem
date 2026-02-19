using AppEnfermagem.Models;
using AppEnfermagem.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppEnfermagem.ViewModels;

// Permite receber o objeto TopicImage para edição via navegação
[QueryProperty(nameof(NovaImagem), "ImagemObjeto")]
public partial class FormularioImagemViewModel : ObservableObject
{
    private readonly IContentService _contentService;

    [ObservableProperty] private TopicImage novaImagem;
    [ObservableProperty] private Topic topicoSelecionado;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string tituloPagina = "Nova Imagem";

    public ObservableCollection<Topic> ListaTopicos { get; } = new();

    public FormularioImagemViewModel(IContentService contentService)
    {
        _contentService = contentService;

        // Se não vier via navegação (Criação), inicializa um novo objeto
        if (NovaImagem == null)
            NovaImagem = new TopicImage();

        CarregarTopicos();
    }

    // Disparado automaticamente quando o parâmetro de navegação é atribuído
    partial void OnNovaImagemChanged(TopicImage value)
    {
        if (value != null && value.ImageId != 0)
        {
            TituloPagina = "Editar Imagem";

            // Tenta selecionar o tópico correto no Picker automaticamente
            if (ListaTopicos.Any())
            {
                TopicoSelecionado = ListaTopicos.FirstOrDefault(t => t.TopicID == value.TopicId);
            }
        }
    }

    private async void CarregarTopicos()
    {
        var data = await _contentService.ObterTopicosAsync();
        if (data?.Topicos != null)
        {
            foreach (var t in data.Topicos) ListaTopicos.Add(t);

            // Caso seja uma edição, precisamos garantir que o tópico seja selecionado
            // após a lista ser carregada da API
            if (NovaImagem?.TopicId != 0)
            {
                TopicoSelecionado = ListaTopicos.FirstOrDefault(t => t.TopicID == NovaImagem.TopicId);
            }
        }
    }

    [RelayCommand]
    public async Task SalvarImagem()
    {
        // Validação básica
        if (TopicoSelecionado == null || string.IsNullOrWhiteSpace(NovaImagem.ImageUrl))
        {
            await App.Current.MainPage.DisplayAlert("Atenção", "Selecione um tópico e insira a URL da imagem.", "OK");
            return;
        }

        try
        {
            IsLoading = true;
            NovaImagem.TopicId = TopicoSelecionado.TopicID;

            bool sucesso;

            // Decide entre Criar (POST) ou Atualizar (PUT) baseado no ID
            if (NovaImagem.ImageId == 0)
            {
                sucesso = await _contentService.CriarImagemTopicoAsync(NovaImagem);
            }
            else
            {
                sucesso = await _contentService.AtualizarImagemTopicoAsync(NovaImagem);
            }

            if (sucesso)
            {
                await App.Current.MainPage.DisplayAlert("Sucesso", "Banco de dados atualizado!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Erro", "Não foi possível salvar na API.", "OK");
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}