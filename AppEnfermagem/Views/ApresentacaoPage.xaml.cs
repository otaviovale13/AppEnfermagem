using System;
using Microsoft.Maui.Controls;

namespace AppEnfermagem.Views
{
    public partial class ApresentacaoPage : ContentPage
    {
        public ApresentacaoPage()
        {
            InitializeComponent();
        }

        private async void SaibaMais_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//InicioPage");
        }
        
        private async void BtnComecar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//InicioPage");
        }
    }
}