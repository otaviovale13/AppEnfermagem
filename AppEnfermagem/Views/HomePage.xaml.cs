using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeViewModel vm)
        {
            // 1. Atualiza se é admin ou não
            vm.VerificarStatusAdmin();

            // 2. Recarrega os dados (caso tenha editado algo)
            await vm.InicializarTela();
        }
    }
}