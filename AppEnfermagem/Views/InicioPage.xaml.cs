using Microsoft.Maui.Controls;

namespace AppEnfermagem.Views
{
    public partial class InicioPage : ContentPage
    {
        public InicioPage ()
        {
            InitializeComponent();
        }

        private async void OnConhecerClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ApresentacaoPage");
        }
    }
}
