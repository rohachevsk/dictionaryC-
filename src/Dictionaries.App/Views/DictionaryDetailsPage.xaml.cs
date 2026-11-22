using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;

namespace Dictionaries.App.Views;

[QueryProperty(nameof(DictionaryIdString), "dictionaryId")]
public partial class DictionaryDetailsPage : ContentPage
{
    private readonly IDictionaryService _dictionaryService;
    private Guid _dictionaryId;

    public string DictionaryIdString
    {
        set
        {
            if (Guid.TryParse(value, out var parsed))
            {
                _dictionaryId = parsed;
                RefreshPage();
            }
            else
            {
                DictionaryTitleLabel.Text = "Некоректний ідентифікатор словника.";
            }
        }
    }

    public DictionaryDetailsPage(IDictionaryService dictionaryService)
    {
        InitializeComponent();
        _dictionaryService = dictionaryService;
    }

    private void OnNewWordInputChanged(object sender, TextChangedEventArgs e)
    {
        if (ErrorLabel.IsVisible)
            ErrorLabel.IsVisible = false;
    }

    private void RefreshPage()
    {
        try
        {
            var dict = _dictionaryService.GetDictionary(_dictionaryId);
            Title = dict.Name;
            DictionaryTitleLabel.Text = dict.Name;
            LanguagesLabel.Text = $"{dict.SourceLanguage} → {dict.TargetLanguage} • Всього слів: {dict.Words.Count}";
            ApplySearchFilter(WordSearchBar.Text);
        }
        catch (DictionaryException ex)
        {
            DictionaryTitleLabel.Text = ex.Message;
            WordsCollection.ItemsSource = null;
        }
        catch (Exception)
        {
            DictionaryTitleLabel.Text = "Не вдалося завантажити дані словника.";
            WordsCollection.ItemsSource = null;
        }
    }

    private void ApplySearchFilter(string? query)
    {
        try
        {
            var allWords = _dictionaryService.GetWords(_dictionaryId);
            var trimmed = query?.Trim() ?? string.Empty;

            IEnumerable<WordEntry> filtered = allWords;
            if (!string.IsNullOrEmpty(trimmed))
            {
                var exactMatch = _dictionaryService.FindWord(_dictionaryId, trimmed);
                filtered = allWords.Where(w =>
                    (exactMatch != null && string.Equals(w.Word, exactMatch.Word, StringComparison.OrdinalIgnoreCase)) ||
                    w.Word.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ||
                    w.Translations.Any(t => t.Contains(trimmed, StringComparison.OrdinalIgnoreCase)));
            }

            var items = filtered.Select(w => new WordItemViewModel
            {
                Word = w.Word,
                TranslationsText = string.Join(", ", w.Translations)
            }).ToList();

            WordsCollection.ItemsSource = items;

            if (items.Count == 0)
            {
                EmptyWordsLabel.Text = string.IsNullOrEmpty(trimmed)
                    ? "Слів поки немає. Додайте перше слово вище."
                    : $"За запитом «{trimmed}» нічого не знайдено.";
                EmptyWordsLabel.IsVisible = true;
            }
            else
            {
                EmptyWordsLabel.IsVisible = false;
            }
        }
        catch (DictionaryException ex)
        {
            WordsCollection.ItemsSource = null;
            EmptyWordsLabel.Text = ex.Message;
            EmptyWordsLabel.IsVisible = true;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplySearchFilter(e.NewTextValue);
    }

    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        ApplySearchFilter(WordSearchBar.Text);
    }

    private void OnAddWordClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        try
        {
            var word = NewWordEntry.Text ?? string.Empty;
            var translations = (NewTranslationsEntry.Text ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            _dictionaryService.AddWord(_dictionaryId, word, translations);

            NewWordEntry.Text = string.Empty;
            NewTranslationsEntry.Text = string.Empty;
            RefreshPage();
        }
        catch (DictionaryException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception)
        {
            ErrorLabel.Text = "Не вдалося додати слово. Перевірте введені дані.";
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnAddTranslationClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string word })
            return;

        var newTranslation = await DisplayPromptAsync(
            "Новий переклад",
            $"Введіть додатковий переклад для слова «{word}»:",
            "Додати",
            "Скасувати",
            "Наприклад: яблуня");

        if (newTranslation is null)
            return;

        try
        {
            _dictionaryService.AddTranslation(_dictionaryId, word, newTranslation);
            RefreshPage();
        }
        catch (DictionaryException ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
        catch (Exception)
        {
            await DisplayAlert("Помилка", "Не вдалося додати переклад.", "ОК");
        }
    }

    private async void OnEditWordClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string word })
            return;

        var action = await DisplayActionSheet(
            $"Редагування «{word}»",
            "Скасувати",
            null,
            "Змінити написання слова",
            "Змінити переклад",
            "Видалити один переклад");

        if (string.IsNullOrEmpty(action) || action == "Скасувати")
            return;

        try
        {
            if (action == "Змінити написання слова")
            {
                var updatedWord = await DisplayPromptAsync(
                    "Редагування слова",
                    "Введіть нове написання слова:",
                    "Зберегти",
                    "Скасувати",
                    initialValue: word);

                if (updatedWord is null)
                    return;

                _dictionaryService.ReplaceWord(_dictionaryId, word, updatedWord);
                RefreshPage();
            }
            else if (action == "Змінити переклад")
            {
                var entry = _dictionaryService.FindWord(_dictionaryId, word);
                if (entry is null)
                    return;

                var oldTranslation = entry.Translations.Count == 1
                    ? entry.Translations[0]
                    : await DisplayActionSheet("Оберіть переклад для заміни", "Скасувати", null, entry.Translations.ToArray());

                if (string.IsNullOrEmpty(oldTranslation) || oldTranslation == "Скасувати")
                    return;

                var newTranslation = await DisplayPromptAsync(
                    "Редагування перекладу",
                    $"Замінити переклад «{oldTranslation}» на:",
                    "Зберегти",
                    "Скасувати",
                    initialValue: oldTranslation);

                if (newTranslation is null)
                    return;

                _dictionaryService.ReplaceTranslation(_dictionaryId, word, oldTranslation, newTranslation);
                RefreshPage();
            }
            else if (action == "Видалити один переклад")
            {
                var entry = _dictionaryService.FindWord(_dictionaryId, word);
                if (entry is null)
                    return;

                var targetTranslation = entry.Translations.Count == 1
                    ? entry.Translations[0]
                    : await DisplayActionSheet("Оберіть переклад для видалення", "Скасувати", null, entry.Translations.ToArray());

                if (string.IsNullOrEmpty(targetTranslation) || targetTranslation == "Скасувати")
                    return;

                _dictionaryService.RemoveTranslation(_dictionaryId, word, targetTranslation);
                RefreshPage();
            }
        }
        catch (DictionaryException ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
        catch (Exception)
        {
            await DisplayAlert("Помилка", "Не вдалося зберегти зміни.", "ОК");
        }
    }

    private async void OnExportWordClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string word })
            return;

        try
        {
            var entry = _dictionaryService.FindWord(_dictionaryId, word);
            if (entry is null)
            {
                await DisplayAlert("Помилка", "Слово не знайдено.", "ОК");
                return;
            }

            await Navigation.PushAsync(new ExportPage(entry));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося відкрити вікно експорту: {ex.Message}", "ОК");
        }
    }

    private async void OnDeleteWordClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string word })
            return;

        bool confirm = await DisplayAlert(
            "Видалення слова",
            $"Видалити слово «{word}» разом з усіма перекладами?",
            "Так, видалити",
            "Скасувати");

        if (!confirm)
            return;

        try
        {
            _dictionaryService.RemoveWord(_dictionaryId, word);
            RefreshPage();
        }
        catch (DictionaryException ex)
        {
            await DisplayAlert("Помилка", ex.Message, "ОК");
        }
        catch (Exception)
        {
            await DisplayAlert("Помилка", "Не вдалося видалити слово.", "ОК");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private sealed class WordItemViewModel
    {
        public string Word { get; init; } = string.Empty;
        public string TranslationsText { get; init; } = string.Empty;
    }
}
