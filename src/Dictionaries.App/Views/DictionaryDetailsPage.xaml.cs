using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Interfaces;

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
                LoadDictionaryInfo();
            }
        }
    }

    public DictionaryDetailsPage(IDictionaryService dictionaryService)
    {
        InitializeComponent();
        _dictionaryService = dictionaryService;
    }

    private void LoadDictionaryInfo()
    {
        try
        {
            var dict = _dictionaryService.GetDictionary(_dictionaryId);
            Title = dict.Name;
            DictionaryTitleLabel.Text = dict.Name;
            LanguagesLabel.Text = $"{dict.SourceLanguage} → {dict.TargetLanguage} • Слів: {dict.Words.Count}";
        }
        catch (DictionaryException ex)
        {
            DictionaryTitleLabel.Text = ex.Message;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
