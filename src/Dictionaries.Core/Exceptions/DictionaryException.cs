namespace Dictionaries.Core.Exceptions;

/// <summary>
/// Помилка роботи зі словником: порушено правило (дублікат, останній переклад, порожнє поле)
/// або не знайдено словник / слово / переклад.
/// <see cref="Exception.Message"/> містить зрозумілий текст, який можна одразу показати користувачеві.
/// </summary>
public class DictionaryException : Exception
{
    public DictionaryException(string message) : base(message)
    {
    }

    public DictionaryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
