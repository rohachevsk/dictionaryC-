using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;

namespace Dictionaries.Export;

public class JsonExportService : IExportService
{
    public string FormatName => "JSON";

    public string FileExtension => ".json";

    public void Export(WordEntry entry, string filePath)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Налаштування для красивих відступів і нормального відображення кирилиці
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        string json = JsonSerializer.Serialize(entry, options);
        File.WriteAllText(filePath, json, Encoding.UTF8);
    }
}
