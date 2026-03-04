using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class SuportePage : ContentPage
{
    private readonly SuporteViewModel _viewModel;

    public SuportePage(SuporteViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Toda vez que a tela aparecer, recarrega as dúvidas da API
        if (_viewModel != null)
        {
            await _viewModel.CarregarPosts();
        }
    }
}