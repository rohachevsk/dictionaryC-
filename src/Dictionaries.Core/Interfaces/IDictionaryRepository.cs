using Dictionaries.Core.Models;

namespace Dictionaries.Core.Interfaces;

/// <summary>
/// Сховище словників. Відповідає лише за збереження і завантаження,
/// усі правила роботи зі словами перевіряє <see cref="IDictionaryService"/>.
/// </summary>
public interface IDictionaryRepository
{
    /// <summary>
    /// Завантажує всі збережені словники.
    /// Пошкоджені записи пропускаються, метод не кидає виняток через один поганий файл.
    /// </summary>
    /// <returns>Список словників; порожній, якщо збережених словників немає.</returns>
    IReadOnlyList<WordDictionary> LoadAll();

    /// <summary>
    /// Зберігає словник. Якщо словник з таким <see cref="WordDictionary.Id"/> уже є, він перезаписується.
    /// </summary>
    /// <param name="dictionary">Словник для збереження.</param>
    void Save(WordDictionary dictionary);

    /// <summary>
    /// Видаляє словник зі сховища. Якщо словника з таким ідентифікатором немає, нічого не відбувається.
    /// </summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    void Delete(Guid dictionaryId);
}
