using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEnfermagem.ViewModels;

public partial class OpcoesViewModel
{
    [RelayCommand]
    public async Task IrArtigos()
    {
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }

    [RelayCommand]
    public async Task IrSuporte()
    {
        await Shell.Current.GoToAsync($"//{nameof(SuportePage)}");
    }

    [RelayCommand]
    public async Task IrAjuda()
    {
        await Shell.Current.GoToAsync($"//{nameof(AjudaPage)}");
    }
}
