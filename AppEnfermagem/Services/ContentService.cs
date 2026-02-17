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
    Task<bool> CriarTopicoAsync(Topic topico);
}

public class ContentService : IContentService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/Content";

    public ContentService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<ArtigoItem?> ObterTopicosAsync() => await _httpClient.GetFromJsonAsync<ArtigoItem>(BaseUrl);

    public async Task<bool> CriarArtigoAsync(Article artigo)
    {
        var resp = await _httpClient.PostAsJsonAsync(BaseUrl, artigo);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> AtualizarArtigoAsync(Article artigo)
    {
        var resp = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{artigo.ArticleID}", artigo);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeletarArtigoAsync(long id)
    {
        var resp = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> CriarTopicoAsync(Topic topico)
    {
        var resp = await _httpClient.PostAsJsonAsync($"{BaseUrl}/topico", topico);
        return resp.IsSuccessStatusCode;
    }
}