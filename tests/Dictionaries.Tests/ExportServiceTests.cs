using System.Text;
using Dictionaries.Core.Models;
using Dictionaries.Export;
using Xunit;

namespace Dictionaries.Tests;

public class ExportServiceTests : IDisposable
{
    private readonly string _tempFolder;

    public ExportServiceTests()
    {
        // Створюємо окрему тимчасову папку для кожного запуску тестів,
        // щоб не засмічувати проєкт і не конфліктувати між тестами
        _tempFolder = Path.Combine(Path.GetTempPath(), $"ExportTests_{Guid.NewGuid()}");
    }

    [Fact]
    public void TxtExportService_Export_CreatesFileWithWordTranslationsAndCyrillic()
    {
        // Arrange (підготовка даних)
        var service = new TxtExportService();
        var entry = new WordEntry
        {
            Word = "apple",
            Translations = new List<string> { "яблуко", "яблуня" }
        };

        // Вказуємо шлях у вкладеній неіснуючій підпапці, щоб перевірити створення папки
        string targetDirectory = Path.Combine(_tempFolder, "nested_folder");
        string filePath = Path.Combine(targetDirectory, "apple.txt");

        // Act (виконання дії)
        service.Export(entry, filePath);

        // Assert (перевірка результату)
        Assert.True(Directory.Exists(targetDirectory));
        Assert.True(File.Exists(filePath));

        string content = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.Contains("apple", content);
        Assert.Contains("- яблуко", content);
        Assert.Contains("- яблуня", content);
        Assert.DoesNotContain("?", content);
    }

    [Fact]
    public void JsonExportService_Export_CreatesFileWithWordTranslationsAndCyrillic()
    {
        // Arrange
        var service = new JsonExportService();
        var entry = new WordEntry
        {
            Word = "book",
            Translations = new List<string> { "книга", "бронювати" }
        };

        string targetDirectory = Path.Combine(_tempFolder, "json_folder");
        string filePath = Path.Combine(targetDirectory, "book.json");

        // Act
        service.Export(entry, filePath);

        // Assert
        Assert.True(Directory.Exists(targetDirectory));
        Assert.True(File.Exists(filePath));

        string content = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.Contains("book", content);
        Assert.Contains("книга", content);
        Assert.Contains("бронювати", content);
    }

    public void Dispose()
    {
        // Прибираємо тимчасову папку після завершення тестів
        if (Directory.Exists(_tempFolder))
        {
            Directory.Delete(_tempFolder, recursive: true);
        }
    }
}
