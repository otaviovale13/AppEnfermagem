using AppEnfermagem.Services; // Para acessar o UserSession
using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace AppEnfermagem.ViewModels;

public partial class LoadingViewModel : ObservableObject
{
    public LoadingViewModel()
    {
    }

    public async Task VerificarLogin()
    {
        await Task.Delay(1000);

        bool estaLogado = UserSession.RestoreSession();

        if (estaLogado)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
        else
        {
            await Shell.Current.GoToAsync($"//{nameof(InicioPage)}");
        }
    }
}