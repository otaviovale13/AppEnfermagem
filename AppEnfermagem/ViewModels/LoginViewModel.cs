using AppEnfermagem.Models;
using AppEnfermagem.Moldes; // Certifique-se que o LoginRequestDto está aqui
using AppEnfermagem.Services;
using AppEnfermagem.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppEnfermagem.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ILoginService _loginService;

    // 1. ADICIONADO: Propriedade para o Nome de Usuário
    [ObservableProperty]
    private string usuario;

    [ObservableProperty]
    private string senha;

    [ObservableProperty]
    private bool isLoading;

    public LoginViewModel(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [RelayCommand]
    private async Task Login()
    {
        Preferences.Clear();

        // Valida se preencheu tudo
        if (!ValidarCampos())
            return;

        IsLoading = true;

        // 2. ALTERADO: Agora enviamos Usuário E Senha para a API
        var loginRequest = new LoginRequestDto
        {
            Username = Usuario.Trim(), // Pega do campo de texto
            Password = Senha.Trim()    // Pega do campo de senha
        };

        var usuarioLogado = await _loginService.LoginAsync(loginRequest);

        if (usuarioLogado != null)
        {
            // Salva dados na sessão
            long userId = usuarioLogado.AdminID; // Certifique-se que seu Model User tem UserId ou AdminID

            Preferences.Set("UserId", userId.ToString());
            Preferences.Set("UserName", usuarioLogado.Username); // Ou usuarioLogado.Username

            try
            {
                IsLoading = false;

                await Shell.Current.DisplayAlert("Sucesso", $"Bem-vindo, {Usuario}!", "OK");
                LimparCampos();

                await Shell.Current.GoToAsync($"//{nameof(AdmPage)}");
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await Shell.Current.DisplayAlert("Erro", "Login feito, mas falha ao carregar a Home.", "OK");
            }
        }
        else
        {
            IsLoading = false;
            // Mostra o erro que veio da API (ex: "Usuário ou senha inválidos")
            await Shell.Current.DisplayAlert("Falha", _loginService.LastErrorMessage ?? "Erro desconhecido", "OK");
        }
    }

    private bool ValidarCampos()
    {
        // 3. ALTERADO: Valida os dois campos
        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Senha))
        {
            Shell.Current.DisplayAlert("Atenção", "Preencha usuário e senha.", "OK");
            return false;
        }

        return true;
    }

    private void LimparCampos()
    {
        Senha = string.Empty;
        // Opcional: Se quiser limpar o usuário também, descomente a linha abaixo
        // Usuario = string.Empty; 
    }

    [RelayCommand]
    private async Task Voltar()
    {
        // Força a ida para a Home, garantindo que funciona sempre
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }
}