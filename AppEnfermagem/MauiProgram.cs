using AppEnfermagem.Services;
using AppEnfermagem.ViewModels;
using AppEnfermagem.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using System.Buffers.Text;

namespace AppEnfermagem
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            string baseUrl = "https://apienfermagem.runasp.net/";

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            builder.Services.AddHttpClient<IContentService, ContentService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddHttpClient<ILoginService, LoginService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddTransient<LoadingPage>();

            builder.Services.AddTransient<AdmViewModel>();
            builder.Services.AddTransient<AdmPage>();

            builder.Services.AddTransient<FormularioArtigoPage>();
            builder.Services.AddTransient<FormularioArtigoViewModel>();

            builder.Services.AddTransient<FormularioTopicoPage>();
            builder.Services.AddTransient<FormularioTopicoViewModel>();

            builder.Services.AddTransient<FormularioImagemPage>();
            builder.Services.AddTransient<FormularioImagemViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
