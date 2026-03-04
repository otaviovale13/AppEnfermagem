using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEnfermagem.ViewModels;

public partial class AjudaViewModel : ObservableObject
{
    [ObservableProperty] private string copyButtonText = "Copiar Pix";

    [RelayCommand]
    public async Task Voltar()
    {
        await Shell.Current.GoToAsync($"//{nameof(OpcoesPage)}");
    }

    [RelayCommand]
    private async Task CopyPix()
    {
        await Clipboard.Default.SetTextAsync("00020126360014BR.GOV.BCB.PIX0114+55119393944275204000053039865802BR5901N6001C62070503***6304C18A");

        string original = CopyButtonText;
        CopyButtonText = "Copiado!";
        await Task.Delay(1500);
        CopyButtonText = original;
    }
}
