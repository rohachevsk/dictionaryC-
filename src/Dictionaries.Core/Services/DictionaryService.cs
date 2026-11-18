using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;

namespace Dictionaries.Core.Services;

/// <summary>
/// Реалізація <see cref="IDictionaryService"/>. Правила описано в коментарях до інтерфейсу.
/// </summary>
/// <remarks>
/// Словники завантажуються з <see cref="IDictionaryRepository"/> під час першого звернення
/// і тримаються в пам'яті. Кожна зміна виконується над копією словника: копія зберігається
/// у сховище і лише після успішного збереження замінює словник у пам'яті.
/// Тож якщо збереження не вдалося, дані в пам'яті залишаються такими, як були.
/// </remarks>
public class DictionaryService : IDictionaryService
{
    /// <summary>Порівняння слів, перекладів і назв: без урахування регістру.</summary>
    private static readonly StringComparer TextComparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>Сортування для показу користувачеві (за алфавітом поточної мови).</summary>
    private static readonly StringComparer DisplayComparer = StringComparer.CurrentCultureIgnoreCase;

    private readonly IDictionaryRepository _repository;
    private readonly object _sync = new();
    private Dictionary<Guid, WordDictionary>? _dictionaries;

    public DictionaryService(IDictionaryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // ---------------------------------------------------------------- словники

    public IReadOnlyList<WordDictionary> GetDictionaries()
    {
        lock (_sync)
        {
            return Dictionaries.Values
                .OrderBy(d => d.Name, DisplayComparer)
                .Select(d => d.Clone())
                .ToList();
        }
    }

    public WordDictionary GetDictionary(Guid dictionaryId)
    {
        lock (_sync)
        {
            return FindDictionary(dictionaryId).Clone();
        }
    }

    public WordDictionary CreateDictionary(string name, string sourceLanguage, string targetLanguage)
    {
        var cleanName = Require(name, "Назва словника не може бути порожньою.");
        var cleanSource = Require(sourceLanguage, "Мова, з якої перекладаємо, не може бути порожньою.");
        var cleanTarget = Require(targetLanguage, "Мова, на яку перекладаємо, не може бути порожньою.");

        lock (_sync)
        {
            if (Dictionaries.Values.Any(d => TextComparer.Equals(d.Name, cleanName)))
                throw new DictionaryException($"Словник «{cleanName}» уже існує.");

            var dictionary = new WordDictionary
            {
                Id = Guid.NewGuid(),
                Name = cleanName,
                SourceLanguage = cleanSource,
                TargetLanguage = cleanTarget,
            };

            SaveToRepository(dictionary);
            Dictionaries[dictionary.Id] = dictionary;
            return dictionary.Clone();
        }
    }

    public void DeleteDictionary(Guid dictionaryId)
    {
        lock (_sync)
        {
            var dictionary = FindDictionary(dictionaryId);

            try
            {
                _repository.Delete(dictionaryId);
            }
            catch (Exception ex) when (ex is not DictionaryException)
            {
                throw new DictionaryException($"Не вдалося видалити словник «{dictionary.Name}». {ex.Message}", ex);
            }

            Dictionaries.Remove(dictionaryId);
        }
    }

    // ---------------------------------------------------------------- слова

    public IReadOnlyList<WordEntry> GetWords(Guid dictionaryId)
    {
        lock (_sync)
        {
            return FindDictionary(dictionaryId).Words
                .OrderBy(w => w.Word, DisplayComparer)
                .Select(w => w.Clone())
                .ToList();
        }
    }

    public WordEntry AddWord(Guid dictionaryId, string word, IEnumerable<string> translations)
    {
        var cleanWord = RequireWord(word);
        var cleanTranslations = RequireTranslations(translations);

        return Modify(dictionaryId, dictionary =>
        {
            if (FindEntry(dictionary, cleanWord) is not null)
                throw new DictionaryException($"Слово «{cleanWord}» уже є у словнику.");

            var entry = new WordEntry { Word = cleanWord, Translations = cleanTranslations };
            dictionary.Words.Add(entry);
            return entry.Clone();
        });
    }

    public void AddTranslation(Guid dictionaryId, string word, string translation)
    {
        var cleanWord = RequireWord(word);
        var cleanTranslation = RequireTranslation(translation);

        Modify(dictionaryId, dictionary =>
        {
            var entry = GetEntry(dictionary, cleanWord);

            if (entry.Translations.Contains(cleanTranslation, TextComparer))
                throw new DictionaryException($"Слово «{entry.Word}» уже має переклад «{cleanTranslation}».");

            entry.Translations.Add(cleanTranslation);
        });
    }

    public void ReplaceWord(Guid dictionaryId, string oldWord, string newWord)
    {
        var cleanOld = RequireWord(oldWord);
        var cleanNew = Require(newWord, "Нове слово не може бути порожнім.");

        Modify(dictionaryId, dictionary =>
        {
            var entry = GetEntry(dictionary, cleanOld);

            // Дозволяємо змінити регістр того самого слова (apple → Apple),
            // але не дозволяємо перейменувати слово на інше, яке вже є у словнику.
            var other = FindEntry(dictionary, cleanNew);
            if (other is not null && !ReferenceEquals(other, entry))
                throw new DictionaryException($"Слово «{other.Word}» уже є у словнику.");

            entry.Word = cleanNew;
        });
    }

    public void ReplaceTranslation(Guid dictionaryId, string word, string oldTranslation, string newTranslation)
    {
        var cleanWord = RequireWord(word);
        var cleanOld = RequireTranslation(oldTranslation);
        var cleanNew = Require(newTranslation, "Новий переклад не може бути порожнім.");

        Modify(dictionaryId, dictionary =>
        {
            var entry = GetEntry(dictionary, cleanWord);
            var index = GetTranslationIndex(entry, cleanOld);

            var duplicateIndex = entry.Translations.FindIndex(t => TextComparer.Equals(t, cleanNew));
            if (duplicateIndex >= 0 && duplicateIndex != index)
                throw new DictionaryException($"Слово «{entry.Word}» уже має переклад «{entry.Translations[duplicateIndex]}».");

            entry.Translations[index] = cleanNew;
        });
    }

    public void RemoveWord(Guid dictionaryId, string word)
    {
        var cleanWord = RequireWord(word);

        Modify(dictionaryId, dictionary =>
        {
            var entry = GetEntry(dictionary, cleanWord);
            dictionary.Words.Remove(entry);
        });
    }

    public void RemoveTranslation(Guid dictionaryId, string word, string translation)
    {
        var cleanWord = RequireWord(word);
        var cleanTranslation = RequireTranslation(translation);

        Modify(dictionaryId, dictionary =>
        {
            var entry = GetEntry(dictionary, cleanWord);
            var index = GetTranslationIndex(entry, cleanTranslation);

            if (entry.Translations.Count == 1)
                throw new DictionaryException(
                    $"Не можна видалити останній переклад слова «{entry.Word}». " +
                    "Спочатку додайте інший переклад або видаліть слово повністю.");

            entry.Translations.RemoveAt(index);
        });
    }

    public WordEntry? FindWord(Guid dictionaryId, string word)
    {
        var cleanWord = Require(word, "Введіть слово для пошуку.");

        lock (_sync)
        {
            return FindEntry(FindDictionary(dictionaryId), cleanWord)?.Clone();
        }
    }

    // ---------------------------------------------------------------- допоміжне

    /// <summary>Словники в пам'яті; завантажуються зі сховища під час першого звернення.</summary>
    private Dictionary<Guid, WordDictionary> Dictionaries
    {
        get
        {
            if (_dictionaries is null)
            {
                IReadOnlyList<WordDictionary> loaded;
                try
                {
                    loaded = _repository.LoadAll();
                }
                catch (Exception ex) when (ex is not DictionaryException)
                {
                    throw new DictionaryException($"Не вдалося завантажити словники. {ex.Message}", ex);
                }

                var result = new Dictionary<Guid, WordDictionary>();
                foreach (var dictionary in loaded)
                {
                    var copy = dictionary.Clone();
                    copy.Words ??= new List<WordEntry>();
                    foreach (var entry in copy.Words)
                        entry.Translations ??= new List<string>();
                    result[copy.Id] = copy;
                }

                _dictionaries = result;
            }

            return _dictionaries;
        }
    }

    /// <summary>
    /// Виконує зміну над копією словника, зберігає копію і лише тоді замінює нею словник у пам'яті.
    /// </summary>
    private T Modify<T>(Guid dictionaryId, Func<WordDictionary, T> change)
    {
        lock (_sync)
        {
            var copy = FindDictionary(dictionaryId).Clone();
            var result = change(copy);
            SaveToRepository(copy);
            Dictionaries[dictionaryId] = copy;
            return result;
        }
    }

    private void Modify(Guid dictionaryId, Action<WordDictionary> change) =>
        Modify(dictionaryId, dictionary =>
        {
            change(dictionary);
            return true;
        });

    private void SaveToRepository(WordDictionary dictionary)
    {
        try
        {
            _repository.Save(dictionary.Clone());
        }
        catch (Exception ex) when (ex is not DictionaryException)
        {
            throw new DictionaryException($"Не вдалося зберегти словник «{dictionary.Name}». {ex.Message}", ex);
        }
    }

    private WordDictionary FindDictionary(Guid dictionaryId) =>
        Dictionaries.TryGetValue(dictionaryId, out var dictionary)
            ? dictionary
            : throw new DictionaryException("Словник не знайдено. Можливо, його вже видалено.");

    private static WordEntry? FindEntry(WordDictionary dictionary, string word) =>
        dictionary.Words.FirstOrDefault(w => TextComparer.Equals(w.Word, word));

    private static WordEntry GetEntry(WordDictionary dictionary, string word) =>
        FindEntry(dictionary, word)
        ?? throw new DictionaryException($"Слово «{word}» не знайдено у словнику «{dictionary.Name}».");

    private static int GetTranslationIndex(WordEntry entry, string translation)
    {
        var index = entry.Translations.FindIndex(t => TextComparer.Equals(t, translation));
        return index >= 0
            ? index
            : throw new DictionaryException($"У слова «{entry.Word}» немає перекладу «{translation}».");
    }

    private static string RequireWord(string? word) =>
        Require(word, "Слово не може бути порожнім.");

    private static string RequireTranslation(string? translation) =>
        Require(translation, "Переклад не може бути порожнім.");

    private static List<string> RequireTranslations(IEnumerable<string>? translations)
    {
        var result = new List<string>();

        foreach (var translation in translations ?? Enumerable.Empty<string>())
        {
            var clean = RequireTranslation(translation);
            if (result.Contains(clean, TextComparer))
                throw new DictionaryException($"Переклад «{clean}» указано кілька разів.");
            result.Add(clean);
        }

        if (result.Count == 0)
            throw new DictionaryException("Потрібно вказати щонайменше один переклад.");

        return result;
    }

    /// <summary>Обрізає пробіли на краях; порожній або <c>null</c> рядок — помилка з текстом <paramref name="errorMessage"/>.</summary>
    private static string Require(string? value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new DictionaryException(errorMessage)
            : value.Trim();
}
