namespace Dictionaries.Core.Models;

/// <summary>
/// Словник одного типу, наприклад англо-український.
/// Один словник зберігається в одному файлі <c>Data/{Id}.json</c>.
/// </summary>
public class WordDictionary
{
    /// <summary>Унікальний ідентифікатор словника. Використовується як ім'я файлу.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Назва словника, яку бачить користувач, наприклад «Англо-український».</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Мова, з якої перекладаємо, наприклад «Англійська».</summary>
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>Мова, на яку перекладаємо, наприклад «Українська».</summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>Слова словника разом з їхніми перекладами.</summary>
    public List<WordEntry> Words { get; set; } = new();

    /// <summary>Повна (глибока) копія словника разом з усіма словами.</summary>
    public WordDictionary Clone() => new()
    {
        Id = Id,
        Name = Name,
        SourceLanguage = SourceLanguage,
        TargetLanguage = TargetLanguage,
        Words = Words.Select(w => w.Clone()).ToList(),
    };

    public override string ToString() => $"{Name} ({SourceLanguage} → {TargetLanguage})";
}
