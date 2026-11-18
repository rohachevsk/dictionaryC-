using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Models;

namespace Dictionaries.Core.Interfaces;

/// <summary>
/// Бізнес-логіка роботи зі словниками. Через цей інтерфейс працює інтерфейс користувача.
/// </summary>
/// <remarks>
/// <para>Правила:</para>
/// <list type="bullet">
/// <item>Слова і переклади порівнюються без урахування регістру; пробіли на краях обрізаються.</item>
/// <item>У словнику не може бути двох однакових слів, у слова — двох однакових перекладів.</item>
/// <item>У слова завжди є щонайменше один переклад; останній переклад видалити не можна.</item>
/// <item>Порожні назви, слова і переклади не приймаються.</item>
/// </list>
/// <para>
/// Усі порушення правил повідомляються винятком <see cref="DictionaryException"/>,
/// текст якого можна показати користувачеві без змін.
/// </para>
/// <para>
/// Методи повертають копії даних: зміна повернених об'єктів не змінює словник.
/// Після кожної зміни словник одразу зберігається через <see cref="IDictionaryRepository"/>.
/// </para>
/// </remarks>
public interface IDictionaryService
{
    /// <summary>Повертає всі словники, відсортовані за назвою.</summary>
    IReadOnlyList<WordDictionary> GetDictionaries();

    /// <summary>Повертає словник разом з усіма словами.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <exception cref="DictionaryException">Словник не знайдено.</exception>
    WordDictionary GetDictionary(Guid dictionaryId);

    /// <summary>Створює новий порожній словник.</summary>
    /// <param name="name">Назва словника, наприклад «Англо-український».</param>
    /// <param name="sourceLanguage">Мова, з якої перекладаємо.</param>
    /// <param name="targetLanguage">Мова, на яку перекладаємо.</param>
    /// <returns>Створений словник.</returns>
    /// <exception cref="DictionaryException">Порожнє поле або словник з такою назвою вже існує.</exception>
    WordDictionary CreateDictionary(string name, string sourceLanguage, string targetLanguage);

    /// <summary>Видаляє словник разом з усіма словами.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <exception cref="DictionaryException">Словник не знайдено.</exception>
    void DeleteDictionary(Guid dictionaryId);

    /// <summary>Повертає всі слова словника, відсортовані за алфавітом.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <exception cref="DictionaryException">Словник не знайдено.</exception>
    IReadOnlyList<WordEntry> GetWords(Guid dictionaryId);

    /// <summary>Додає нове слово з одним або кількома перекладами.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="word">Слово.</param>
    /// <param name="translations">Переклади, щонайменше один.</param>
    /// <returns>Додане слово.</returns>
    /// <exception cref="DictionaryException">
    /// Словник не знайдено, порожнє слово або переклад, немає перекладів,
    /// слово вже є у словнику або переклади повторюються.
    /// </exception>
    WordEntry AddWord(Guid dictionaryId, string word, IEnumerable<string> translations);

    /// <summary>Додає ще один переклад до наявного слова.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="word">Слово, до якого додається переклад.</param>
    /// <param name="translation">Новий переклад.</param>
    /// <exception cref="DictionaryException">
    /// Словник або слово не знайдено, порожній переклад або такий переклад уже є.
    /// </exception>
    void AddTranslation(Guid dictionaryId, string word, string translation);

    /// <summary>Замінює написання слова, переклади залишаються.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="oldWord">Поточне слово.</param>
    /// <param name="newWord">Нове написання слова.</param>
    /// <exception cref="DictionaryException">
    /// Словник або слово не знайдено, нове слово порожнє або вже є у словнику.
    /// </exception>
    void ReplaceWord(Guid dictionaryId, string oldWord, string newWord);

    /// <summary>Замінює один переклад слова на інший.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="word">Слово.</param>
    /// <param name="oldTranslation">Поточний переклад.</param>
    /// <param name="newTranslation">Новий переклад.</param>
    /// <exception cref="DictionaryException">
    /// Словник, слово або переклад не знайдено, новий переклад порожній або вже є у слова.
    /// </exception>
    void ReplaceTranslation(Guid dictionaryId, string word, string oldTranslation, string newTranslation);

    /// <summary>Видаляє слово разом з усіма його перекладами.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="word">Слово.</param>
    /// <exception cref="DictionaryException">Словник або слово не знайдено.</exception>
    void RemoveWord(Guid dictionaryId, string word);

    /// <summary>Видаляє один переклад слова.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="word">Слово.</param>
    /// <param name="translation">Переклад для видалення.</param>
    /// <exception cref="DictionaryException">
    /// Словник, слово або переклад не знайдено, або це останній переклад слова.
    /// </exception>
    void RemoveTranslation(Guid dictionaryId, string word, string translation);

    /// <summary>Шукає слово у словнику без урахування регістру.</summary>
    /// <param name="dictionaryId">Ідентифікатор словника.</param>
    /// <param name="word">Слово для пошуку.</param>
    /// <returns>Знайдене слово з перекладами або <c>null</c>, якщо слова немає.</returns>
    /// <exception cref="DictionaryException">Словник не знайдено або слово для пошуку порожнє.</exception>
    WordEntry? FindWord(Guid dictionaryId, string word);
}
