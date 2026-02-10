using AppEnfermagem.Views;

namespace AppEnfermagem
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(FormularioArtigoPage), typeof(FormularioArtigoPage));
        }

        // Add any additional methods or properties for AppShell here
    }
}
