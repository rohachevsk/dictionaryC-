# Словники (`dictionaryC-`)

Командний проєкт на **C# / .NET MAUI**: словники різними мовами з пошуком перекладу, збереженням у JSON та експортом словникових записів.

## Статус

Проєкт у розробці. Структура solution, моделі, інтерфейси та `DictionaryService` уже в `develop`[cite: 16]. Повний опис проєкту оформлюється в рамках задачі **#14** (`feature/readme`).

## Опис проєкту та можливості

Застосунок дозволяє працювати зі словниковими записами (`WordEntry`), шукати переклади та зберігати дані в локальному сховищі або вивантажувати їх у зовнішні файли.

- **Робота зі словниками та пошук перекладу:** додавання слів і перекладів, пошук перекладу та керування словниковими записами через `DictionaryService` (`Dictionaries.Core`).
- **Збереження даних:** читання та запис словників у JSON-файлах (`Dictionaries.Storage`).
- **Експорт даних:** експорт словникових записів у формати **TXT** (`TxtExportService`) та **JSON** (`JsonExportService`) у проєкті `Dictionaries.Export`.
- **Інтерфейс користувача (.NET MAUI):** сторінки та `ViewModel` у `Dictionaries.App` (зокрема сторінка експорту `ExportPage` та `ExportViewModel`).
- **Модульне тестування:** перевірка логіки моделей, сервісів зберігання та експорту за допомогою xUnit-тестів (`Dictionaries.Tests`).

## Структура solution

```text
Dictionaries.sln
├── src/
│   ├── Dictionaries.Core      моделі, інтерфейси, DictionaryService (бізнес-логіка)
│   ├── Dictionaries.Storage   зберігання словників у JSON-файлах (→ Core)
│   ├── Dictionaries.Export    експорт слова у TXT / JSON (→ Core)
│   └── Dictionaries.App       застосунок .NET MAUI (→ Core, Storage, Export)
└── tests/
    └── Dictionaries.Tests     xUnit-тести (→ Core, Storage, Export)