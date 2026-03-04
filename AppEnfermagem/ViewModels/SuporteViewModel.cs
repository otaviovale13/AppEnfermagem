using AppEnfermagem.Models;
using AppEnfermagem.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppEnfermagem.ViewModels;

public partial class SuporteViewModel : ObservableObject
{
    private readonly IContentService _contentService;

    public ObservableCollection<ForumPost> ListaPosts { get; } = new();

    [ObservableProperty] private bool isLoading;

    // Propriedades para nova dúvida
    [ObservableProperty] private string novoTitulo;
    [ObservableProperty] private string novaDuvida;
    [ObservableProperty] private string nomeUsuario; // Opcional

    public SuporteViewModel(IContentService contentService)
    {
        _contentService = contentService;
    }

    [RelayCommand]
    public async Task CarregarPosts()
    {
        IsLoading = true;
        ListaPosts.Clear();

        var postsDb = await _contentService.ObterForumPostsAsync();

        foreach (var p in postsDb)
        {
            ListaPosts.Add(p);
        }

        IsLoading = false;
    }

    [RelayCommand]
    public async Task EnviarDuvida()
    {
        if (string.IsNullOrWhiteSpace(NovoTitulo) || string.IsNullOrWhiteSpace(NovaDuvida))
        {
            await Shell.Current.DisplayAlert("Atenção", "Preencha o título e a dúvida.", "OK");
            return;
        }

        IsLoading = true;

        var novoPost = new ForumPost
        {
            Title = NovoTitulo,
            ContentBody = NovaDuvida,
            AuthorName = string.IsNullOrWhiteSpace(NomeUsuario) ? "Anônimo" : NomeUsuario
        };

        var sucesso = await _contentService.CriarForumPostAsync(novoPost);

        if (sucesso)
        {
            // Limpa os campos
            NovoTitulo = string.Empty;
            NovaDuvida = string.Empty;
            NomeUsuario = string.Empty;

            await Shell.Current.DisplayAlert("Sucesso", "Sua dúvida foi enviada!", "OK");

            // Recarrega a lista para mostrar o novo post
            await CarregarPosts();
        }
        else
        {
            await Shell.Current.DisplayAlert("Erro", "Falha ao enviar, tente novamente.", "OK");
        }

        IsLoading = false;
    }

    [RelayCommand]
    public async Task Voltar()
    {
        // Volta para a página de Opções
        await Shell.Current.GoToAsync($"//OpcoesPage");
    }
}