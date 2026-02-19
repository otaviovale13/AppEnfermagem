using AppEnfermagem.Models;
using AppEnfermagem.Moldes;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace AppEnfermagem.Services;

public interface IContentService
{
    Task<ArtigoItem?> ObterTopicosAsync();

    // Artigos de Texto
    Task<bool> CriarArtigoAsync(Article artigo);
    Task<bool> AtualizarArtigoAsync(Article artigo);
    Task<bool> DeletarArtigoAsync(long articleId);

    // Tópicos
    Task<bool> CriarTopicoAsync(Topic topico);
    Task<bool> AtualizarTopicoAsync(Topic topico);
    Task<bool> DeletarTopicoAsync(int topicId);

    // Artigos de Imagem (TopicImage)
    Task<bool> CriarImagemTopicoAsync(TopicImage topicImage);
    Task<bool> AtualizarImagemTopicoAsync(TopicImage topicImage);
    Task<bool> DeletarImagemTopicoAsync(int imageId);
}

public class ContentService : IContentService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/Content";

    public ContentService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<ArtigoItem?> ObterTopicosAsync() => await _httpClient.GetFromJsonAsync<ArtigoItem>(BaseUrl);

    #region ARTIGOS DE TEXTO
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
    #endregion

    #region TÓPICOS
    public async Task<bool> CriarTopicoAsync(Topic topico)
    {
        var resp = await _httpClient.PostAsJsonAsync($"{BaseUrl}/topico", topico);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> AtualizarTopicoAsync(Topic topico)
    {
        try
        {
            var resp = await _httpClient.PutAsJsonAsync($"{BaseUrl}/topico/{topico.TopicID}", topico);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeletarTopicoAsync(int topicId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/topico/{topicId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
    #endregion

    #region ARTIGOS DE IMAGEM (NOVO)
    public async Task<bool> CriarImagemTopicoAsync(TopicImage topicImage)
    {
        try
        {
            // Rota: api/Content/topico/imagem
            var resp = await _httpClient.PostAsJsonAsync($"{BaseUrl}/topico/imagem", topicImage);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> AtualizarImagemTopicoAsync(TopicImage topicImage)
    {
        try
        {
            // Rota: api/Content/topico/imagem/{id}
            var resp = await _httpClient.PutAsJsonAsync($"{BaseUrl}/topico/imagem/{topicImage.ImageId}", topicImage);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeletarImagemTopicoAsync(int imageId)
    {
        try
        {
            // Rota: api/Content/topico/imagem/{id}
            var resp = await _httpClient.DeleteAsync($"{BaseUrl}/topico/imagem/{imageId}");
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }
    #endregion
}