using AppEnfermagem.Moldes;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace AppEnfermagem.Services;

public interface IContentService
{
    Task<ArtigoItem?> ObterTopicosAsync();
}

public class ContentService : IContentService
{
    private readonly HttpClient _httpClient;

    private const string BaseUrl = "https://apienfermagem.runasp.net/api/Content";

    public ContentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ArtigoItem?> ObterTopicosAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ArtigoItem>($"{BaseUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar artigos: {ex.Message}");
            return null;
        }
    }
}
