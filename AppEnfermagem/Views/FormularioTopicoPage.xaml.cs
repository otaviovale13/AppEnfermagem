using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class FormularioTopicoPage : ContentPage
{
	public FormularioTopicoPage(FormularioTopicoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}