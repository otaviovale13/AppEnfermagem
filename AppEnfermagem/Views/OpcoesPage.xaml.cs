using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class OpcoesPage : ContentPage
{
	public OpcoesPage(OpcoesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}