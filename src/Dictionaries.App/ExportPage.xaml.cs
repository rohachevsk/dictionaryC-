using Dictionaries.Core.Models;

namespace Dictionaries.App;

public partial class ExportPage : ContentPage
{
    private readonly ExportViewModel _viewModel;

    public ExportPage()
    {
        InitializeComponent();
        _viewModel = new ExportViewModel();
        BindingContext = _viewModel;
    }

    public ExportPage(WordEntry wordEntry) : this()
    {
        _viewModel.WordEntry = wordEntry;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}