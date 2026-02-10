using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class AdmPage : ContentPage
{
	public AdmPage(AdmViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}