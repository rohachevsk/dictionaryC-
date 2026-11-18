namespace Dictionaries.Core.Models;

/// <summary>
/// Слово словника і всі його переклади.
/// У слова завжди є щонайменше один переклад.
/// </summary>
public class WordEntry
{
    /// <summary>Слово мовою оригіналу, наприклад «apple».</summary>
    public string Word { get; set; } = string.Empty;

    /// <summary>Переклади слова, наприклад «яблуко», «яблуня».</summary>
    public List<string> Translations { get; set; } = new();

    /// <summary>Копія слова з окремим списком перекладів.</summary>
    public WordEntry Clone() => new()
    {
        Word = Word,
        Translations = new List<string>(Translations),
    };

    public override string ToString() => $"{Word} — {string.Join(", ", Translations)}";
}
