using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using GoyIA.Services;
using GoyIA.ViewModels;
using GoyIA.Pages;

namespace GoyIA
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

            // Register HTTP client
            builder.Services.AddHttpClient<OpenAIService>();

            // Register services
            builder.Services.AddSingleton<OpenAIService>();
            builder.Services.AddSingleton<GalleryService>();

            // Register ViewModels
            builder.Services.AddTransient<ImageGenerationViewModel>();
            builder.Services.AddTransient<ImageEditingViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<GalleryViewModel>();

            // Register Pages
            builder.Services.AddTransient<ImageGenerationPage>();
            builder.Services.AddTransient<ImageEditingPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<GalleryPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
