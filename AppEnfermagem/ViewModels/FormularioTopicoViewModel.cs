using AppEnfermagem.Models;
using AppEnfermagem.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppEnfermagem.ViewModels;

// Permite receber o tópico selecionado na Home para edição
[QueryProperty(nameof(NovoTopico), "TopicoObjeto")]
public partial class FormularioTopicoViewModel : ObservableObject
{
    private readonly IContentService _contentService;
    [ObservableProperty] private Topic novoTopico;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string tituloPagina = "Novo Tópico";

    public FormularioTopicoViewModel(IContentService contentService)
    {
        _contentService = contentService;
        // Se não vier via navegação, inicia um novo
        if (NovoTopico == null)
            NovoTopico = new Topic();

        IsLoading = false;
    }

    [RelayCommand]
    public async Task SalvarTopico()
    {
        if (string.IsNullOrWhiteSpace(NovoTopico.Name))
        {
            await App.Current.MainPage.DisplayAlert("Erro", "Nome obrigatório", "OK");
            return;
        }

        try
        {
            IsLoading = true;
            bool sucesso;

            // Se o ID for 0, é um novo registro (POST). Caso contrário, é edição (PUT).
            if (NovoTopico.TopicID == 0)
            {
                sucesso = await _contentService.CriarTopicoAsync(NovoTopico);
            }
            else
            {
                // Você precisará adicionar este método no ContentService (veja abaixo)
                sucesso = await _contentService.AtualizarTopicoAsync(NovoTopico);
            }

            if (sucesso)
            {
                await App.Current.MainPage.DisplayAlert("Sucesso", "Categoria salva!", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        finally { IsLoading = false; }
    }
}