using Microsoft.Maui.Controls;

namespace AppEnfermagem.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnConhecerClicked(object sender, EventArgs e)
        {
  
            await Shell.Current.GoToAsync("ApresentacaoPage");
        }
    }
}
