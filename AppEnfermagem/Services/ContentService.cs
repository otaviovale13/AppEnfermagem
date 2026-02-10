using AppEnfermagem.Models;
using AppEnfermagem.Moldes;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace AppEnfermagem.Services;

public interface IContentService
{
    Task<ArtigoItem?> ObterTopicosAsync();
    Task<bool> CriarArtigoAsync(Article artigo);
    Task<bool> AtualizarArtigoAsync(Article artigo);
    Task<bool> DeletarArtigoAsync(long articleId);
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

    public async Task<bool> CriarArtigoAsync(Article artigo)
    {
        try
        {
            // Envia para: https://apienfermagem.runasp.net/api/Content
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, artigo);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> AtualizarArtigoAsync(Article artigo)
    {
        try
        {
            // Geralmente APIs esperam o ID na URL para o PUT: api/Content/5
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{artigo.ArticleID}", artigo);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeletarArtigoAsync(long articleId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{articleId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
