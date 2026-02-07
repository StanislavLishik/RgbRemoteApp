using Microsoft.Extensions.Logging;
using RgbRemoteApp.Services;
using RgbRemoteApp.ViewModels;
using System.ComponentModel.Design;


namespace RgbRemoteApp;

public static class MauiProgram
    {
    public static MauiApp CreateMauiApp()
        {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf","OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf","OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<IEspService,EspService>();
        builder.Services.AddSingleton<ISettingsService,SettingsService>();

        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();

        // Register Pages
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
        }
    }
