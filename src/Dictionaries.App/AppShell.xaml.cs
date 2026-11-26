using Dictionaries.App.Views;

namespace Dictionaries.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DictionariesPage), typeof(DictionariesPage));
        Routing.RegisterRoute(nameof(CreateDictionaryPage), typeof(CreateDictionaryPage));
        Routing.RegisterRoute(nameof(DictionaryDetailsPage), typeof(DictionaryDetailsPage));
    }
}
