namespace Dictionaries.App.Views;

public partial class DictionariesPage : ContentPage
{
    public DictionariesPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}