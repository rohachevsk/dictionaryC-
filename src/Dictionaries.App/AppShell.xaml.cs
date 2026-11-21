using Dictionaries.App.Views;

namespace Dictionaries.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DictionariesPage), typeof(DictionariesPage));
    }
}
