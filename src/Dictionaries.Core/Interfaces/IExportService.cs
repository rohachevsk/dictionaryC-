using Dictionaries.Core.Models;

namespace Dictionaries.Core.Interfaces;

/// <summary>
/// Експорт одного слова з перекладами у файл певного формату (TXT, JSON).
/// Кожен формат — окрема реалізація цього інтерфейсу.
/// </summary>
public interface IExportService
{
    /// <summary>Назва формату для показу користувачеві, наприклад «TXT».</summary>
    string FormatName { get; }

    /// <summary>Розширення файлу разом із крапкою, наприклад «.txt».</summary>
    string FileExtension { get; }

    /// <summary>
    /// Записує слово і всі його переклади у файл у кодуванні UTF-8.
    /// Якщо папки для файлу немає, вона створюється. Наявний файл перезаписується.
    /// </summary>
    /// <param name="entry">Слово для експорту.</param>
    /// <param name="filePath">Повний шлях до файлу, який треба створити.</param>
    void Export(WordEntry entry, string filePath);
}
