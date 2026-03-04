using AppEnfermagem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace AppEnfermagem.Views;

public partial class ApresentacaoPage : ContentPage
{
    public ApresentacaoPage()
    {
        InitializeComponent();
    }

    private async void SaibaMais_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(OpcoesPage)}");
    }

    private async void AdminIconClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}