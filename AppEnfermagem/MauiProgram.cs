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

            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
