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
        // 1. Um pequeno delay estético (opcional) para não piscar a tela muito rápido
        await Task.Delay(1000);

        // 2. Verifica se existe sessão salva
        bool estaLogado = UserSession.RestoreSession();

        if (estaLogado)
        {
            // Se sim, vai direto para a Home
            // Usamos "///" para limpar a pilha de navegação (não deixar voltar)
            await Shell.Current.GoToAsync($"//{nameof(AdmPage)}");
        }
        else
        {
            // Se não, manda para o Login
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }
    }
}