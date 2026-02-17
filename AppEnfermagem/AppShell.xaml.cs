using AppEnfermagem.Views;

namespace AppEnfermagem
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(FormularioArtigoPage), typeof(FormularioArtigoPage));
            Routing.RegisterRoute(nameof(FormularioTopicoPage), typeof(FormularioTopicoPage));
        }

        // Add any additional methods or properties for AppShell here
    }
}
