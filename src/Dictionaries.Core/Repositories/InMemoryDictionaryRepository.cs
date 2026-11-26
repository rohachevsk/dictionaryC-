using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;

namespace Dictionaries.Core.Repositories;

/// <summary>
/// Сховище в пам'яті: нічого не пише на диск, дані зникають після закриття застосунку.
/// </summary>
/// <remarks>
/// Потрібне для unit-тестів <see cref="Services.DictionaryService"/> і для роботи над інтерфейсом,
/// поки не готове справжнє сховище у файлах (<c>JsonDictionaryRepository</c>).
/// </remarks>
public class InMemoryDictionaryRepository : IDictionaryRepository
{
    private readonly Dictionary<Guid, WordDictionary> _storage = new();

    public InMemoryDictionaryRepository()
    {
    }

    /// <summary>Створює сховище з уже наявними словниками (наприклад, для тестів або демо).</summary>
    public InMemoryDictionaryRepository(IEnumerable<WordDictionary> dictionaries)
    {
        foreach (var dictionary in dictionaries)
            _storage[dictionary.Id] = dictionary.Clone();
    }

    /// <summary>Скільки разів викликали <see cref="Save"/>. Зручно перевіряти в тестах.</summary>
    public int SaveCount { get; private set; }

    public IReadOnlyList<WordDictionary> LoadAll() =>
        _storage.Values.Select(d => d.Clone()).ToList();

    public void Save(WordDictionary dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        _storage[dictionary.Id] = dictionary.Clone();
        SaveCount++;
    }

    public void Delete(Guid dictionaryId) => _storage.Remove(dictionaryId);
}
