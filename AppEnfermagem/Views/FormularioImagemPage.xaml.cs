using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class FormularioImagemPage : ContentPage
{
	public FormularioImagemPage(FormularioImagemViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
    }
}