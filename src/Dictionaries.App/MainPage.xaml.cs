using Dictionaries.App.Views;
using Dictionaries.Core.Interfaces;

namespace Dictionaries.App;

public partial class MainPage : ContentPage
{
    private readonly IDictionaryService _dictionaryService;

    public MainPage(IDictionaryService dictionaryService)
    {
        InitializeComponent();
        _dictionaryService = dictionaryService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        int count = _dictionaryService.GetDictionaries().Count;
        StatsLabel.Text = $"Доступно словників: {count}";
    }

    private async void OnOpenDictionariesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(DictionariesPage));
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }
}