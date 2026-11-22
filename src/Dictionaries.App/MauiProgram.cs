using Dictionaries.App.Views;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Services;
using Dictionaries.Storage;
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

        string dataFolder = Path.Combine(FileSystem.AppDataDirectory, "Data");
        builder.Services.AddSingleton<IDictionaryRepository>(_ => new JsonDictionaryRepository(dataFolder));
        builder.Services.AddSingleton<IDictionaryService, DictionaryService>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DictionariesPage>();
        builder.Services.AddTransient<CreateDictionaryPage>();
        builder.Services.AddTransient<DictionaryDetailsPage>();

        return builder.Build();
    }
}
