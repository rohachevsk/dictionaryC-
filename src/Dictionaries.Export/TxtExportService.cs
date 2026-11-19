using System.Text;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;

namespace Dictionaries.Export;

public class TxtExportService : IExportService
{
    public string FormatName => "TXT";

    public string FileExtension => ".txt";

    public void Export(WordEntry entry, string filePath)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));

        // Перевіряємо, чи існує папка, в яку зберігаємо файл. Якщо ні — створюємо її
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Формуємо текст згідно з прикладом із завдання
        var sb = new StringBuilder();
        sb.AppendLine(entry.Word);

        foreach (var translation in entry.Translations)
        {
            sb.AppendLine($"- {translation}");
        }

        // Записуємо у файл у кодуванні UTF-8
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }
}