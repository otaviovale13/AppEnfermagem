using AppEnfermagem.Models;
using AppEnfermagem.Moldes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppEnfermagem.Services;

public interface ILoginService
{
    Task<Admin?> LoginAsync(LoginRequestDto loginRequest);

    string LastErrorMessage { get; }
}

public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://apienfermagem.runasp.net/api/Security";

    public string LastErrorMessage { get; private set; }

    public LoginService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Admin?> LoginAsync(LoginRequestDto loginRequest)
    {
        try
        {
            // 1. Serializa manualmente para garantir o controle do JSON
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 2. Envia usando PostAsync (mais seguro que PostAsJsonAsync neste caso)
            var response = await _httpClient.PostAsync($"{BaseUrl}/logar", content);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();

                var usuarioLogado = JsonSerializer.Deserialize<Admin>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (usuarioLogado == null)
                {
                    LastErrorMessage = "O servidor retornou dados vazios.";
                    return null;
                }

                UserSession.Login(usuarioLogado);
                return usuarioLogado;
            }
            else
            {
                // Tratamento de erro detalhado
                string erroApi = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(erroApi))
                {
                    LastErrorMessage = erroApi;
                }
                else
                {
                    LastErrorMessage = $"Erro {response.StatusCode}: O servidor falhou ao processar a requisição.";
                }

                return null;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Erro de conexão: {ex.Message}";
            return null;
        }
    }
}
