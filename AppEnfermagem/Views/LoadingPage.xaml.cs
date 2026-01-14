using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class LoadingPage : ContentPage
{
    private readonly LoadingViewModel _viewModel;

    public LoadingPage(LoadingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Chama a verificação toda vez que a tela aparece
        await _viewModel.VerificarLogin();
    }
}