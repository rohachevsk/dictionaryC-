using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;
using Dictionaries.Export;

namespace Dictionaries.App;

public class ExportViewModel : INotifyPropertyChanged
{
    private readonly IExportService _txtExportService;
    private readonly IExportService _jsonExportService;

    private WordEntry _wordEntry = new();
    private string _selectedFormat = "TXT";
    private string _filePath = string.Empty;
    private string _statusMessage = string.Empty;

    public ExportViewModel()
    {
        _txtExportService = new TxtExportService();
        _jsonExportService = new JsonExportService();

        ExportCommand = new Command(ExecuteExport);
        ResetDefaultPathCommand = new Command(UpdateDefaultFilePath);
    }

    public WordEntry WordEntry
    {
        get => _wordEntry;
        set
        {
            _wordEntry = value ?? new WordEntry();
            OnPropertyChanged();
            OnPropertyChanged(nameof(TranslationsText));
            UpdateDefaultFilePath();
        }
    }

    public string TranslationsText =>
        WordEntry.Translations.Count > 0
            ? string.Join(", ", WordEntry.Translations)
            : "Переклади відсутні";

    public List<string> Formats { get; } = new() { "TXT", "JSON" };

    public string SelectedFormat
    {
        get => _selectedFormat;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            _selectedFormat = value;
            OnPropertyChanged();
            UpdateDefaultFilePath();
        }
    }

    public string FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand ExportCommand { get; }
    public ICommand ResetDefaultPathCommand { get; }

    private void UpdateDefaultFilePath()
    {
        if (string.IsNullOrWhiteSpace(WordEntry.Word))
            return;

        string safeWord = string.Join("_", WordEntry.Word.Trim().Split(Path.GetInvalidFileNameChars()));
        string extension = SelectedFormat.Equals("JSON", StringComparison.OrdinalIgnoreCase) ? ".json" : ".txt";
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        FilePath = Path.Combine(desktopPath, $"{safeWord}_export{extension}");
    }

    private void ExecuteExport()
    {
        StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(WordEntry.Word))
        {
            StatusMessage = "Помилка: не вказано слово для експорту.";
            return;
        }

        if (string.IsNullOrWhiteSpace(FilePath))
        {
            StatusMessage = "Помилка: вкажіть шлях для збереження файлу.";
            return;
        }

        try
        {
            IExportService service = SelectedFormat.Equals("JSON", StringComparison.OrdinalIgnoreCase)
                ? _jsonExportService
                : _txtExportService;

            service.Export(WordEntry, FilePath.Trim());
            StatusMessage = $"Успішно експортовано ({SelectedFormat}) у файл:\n{FilePath.Trim()}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка експорту: {ex.Message}";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
