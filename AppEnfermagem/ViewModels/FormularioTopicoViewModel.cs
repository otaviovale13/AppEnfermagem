using AppEnfermagem.Models;
using AppEnfermagem.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppEnfermagem.ViewModels;

public partial class FormularioTopicoViewModel : ObservableObject
{
    private readonly IContentService _contentService;
    [ObservableProperty] private Topic novoTopico;
    [ObservableProperty] private bool isLoading;

    public FormularioTopicoViewModel(IContentService contentService)
    {
        _contentService = contentService;
        NovoTopico = new Topic();
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
            isLoading = true;
            if (await _contentService.CriarTopicoAsync(NovoTopico))
            {
                await App.Current.MainPage.DisplayAlert("Sucesso", "Tópico criado!", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        finally { isLoading = false; }
    }
}