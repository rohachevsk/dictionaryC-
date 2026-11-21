using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Interfaces;

namespace Dictionaries.App.Views;

public partial class CreateDictionaryPage : ContentPage
{
    private readonly IDictionaryService _dictionaryService;

    public CreateDictionaryPage(IDictionaryService dictionaryService)
    {
        InitializeComponent();
        _dictionaryService = dictionaryService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        try
        {
            _dictionaryService.CreateDictionary(
                NameEntry.Text ?? string.Empty,
                SourceLangEntry.Text ?? string.Empty,
                TargetLangEntry.Text ?? string.Empty);

            await Shell.Current.GoToAsync("..");
        }
        catch (DictionaryException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
