using Dictionaries.App.Views;
using Dictionaries.Core;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Repositories;
using Dictionaries.Core.Services;
using Microsoft.Extensions.Logging;

namespace Dictionaries.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IDictionaryRepository, InMemoryDictionaryRepository>();
        builder.Services.AddSingleton<IDictionaryService, DictionaryService>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DictionariesPage>();

        return builder.Build();
    }
}