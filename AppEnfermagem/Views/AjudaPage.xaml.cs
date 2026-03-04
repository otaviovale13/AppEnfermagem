using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class AjudaPage : ContentPage
{
	public AjudaPage(AjudaViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}