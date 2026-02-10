using AppEnfermagem.Services;
using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEnfermagem.ViewModels;

public partial class AdmViewModel
{
    [RelayCommand]
    public async Task Sair()
    {
        bool resposta = await Shell.Current.DisplayAlert("Sair", "Deseja sair?", "Sim", "Não");
        if (resposta)
        {
            UserSession.Logout();
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
    }
}
