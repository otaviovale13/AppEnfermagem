using AppEnfermagem.ViewModels;

namespace AppEnfermagem.Views;

public partial class FormularioArtigoPage : ContentPage
{
    public FormularioArtigoPage(FormularioArtigoViewModel viewModel)
    {
		InitializeComponent();

        BindingContext = viewModel; // ESSA LINHA É ESSENCIAL
    }
}