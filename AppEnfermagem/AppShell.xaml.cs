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
            Routing.RegisterRoute(nameof(FormularioImagemPage), typeof(FormularioImagemPage));
        }

        // Add any additional methods or properties for AppShell here
    }
}
