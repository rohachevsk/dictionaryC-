using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Interfaces;

namespace Dictionaries.App.Views;

public partial class DictionariesPage : ContentPage
{
    private readonly IDictionaryService _dictionaryService;

    public DictionariesPage(IDictionaryService dictionaryService)
    {
        InitializeComponent();
        _dictionaryService = dictionaryService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDictionariesAsync();
    }

    private async Task LoadDictionariesAsync()
    {
        try
        {
            var items = _dictionaryService.GetDictionaries()
                .Select(d => new DictionaryItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Subtitle = $"{d.SourceLanguage} → {d.TargetLanguage} • Слів: {d.Words.Count}"
                })
                .ToList();

            DictionariesCollection.ItemsSource = items;
            EmptyLabel.Text = "Словників поки немає. Натисніть «+ Створити словник», щоб додати перший.";
            EmptyLabel.IsVisible = items.Count == 0;
        }
        catch (DictionaryException ex)
        {
            EmptyLabel.Text = ex.Message;
            EmptyLabel.IsVisible = true;
        }
        catch (Exception)
        {
            await DisplayAlert("Помилка", "Не вдалося завантажити список словників.", "ОК");
        }
    }

    private async void OnCreateDictionaryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateDictionaryPage));
    }

    private async void OnOpenDictionaryClicked(object sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: Guid id })
        {
            await Shell.Current.GoToAsync($"{nameof(DictionaryDetailsPage)}?dictionaryId={id}");
        }
    }

    private async void OnDeleteDictionaryClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: Guid id })
            return;

        try
        {
            var dict = _dictionaryService.GetDictionary(id);
            bool confirm = await DisplayAlert(
                "Видалення словника",
                $"Ви дійсно хочете видалити словник «{dict.Name}» разом з усіма словами?",
                "Так, видалити",
                "Скасувати");

            if (!confirm)
                return;

            _dictionaryService.DeleteDictionary(id);
            await LoadDictionariesAsync();
        }
        catch (DictionaryException ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
        catch (Exception)
        {
            await DisplayAlert("Помилка", "Не вдалося видалити словник.", "ОК");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private sealed class DictionaryItemViewModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Subtitle { get; init; } = string.Empty;
    }
}
