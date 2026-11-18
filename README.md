# Словники

Командний проєкт на C# / .NET MAUI: словники різними мовами з пошуком перекладу.

## Статус

Проєкт у розробці. Структура solution, моделі, інтерфейси і `DictionaryService` уже в `develop`.
Повний опис проєкту буде додано в Issue #14.

## Структура solution

```
Dictionaries.sln
├── src/
│   ├── Dictionaries.Core      моделі, інтерфейси, DictionaryService (бізнес-логіка)
│   ├── Dictionaries.Storage   зберігання словників у JSON-файлах (→ Core)
│   ├── Dictionaries.Export    експорт слова у TXT / JSON (→ Core)
│   └── Dictionaries.App       застосунок .NET MAUI (→ Core, Storage, Export)
└── tests/
    └── Dictionaries.Tests     xUnit-тести (→ Core, Storage, Export)
```

## Збірка

Потрібен .NET SDK 10 (версію закріплено у `global.json`).

Для застосунку MAUI потрібна робоча навантаження MAUI
(Visual Studio Installer → «Розробка застосунків .NET MAUI» або `dotnet workload install maui`):

```
dotnet build Dictionaries.sln
```

Бібліотеки і тести без MAUI (наприклад, для роботи над Storage / Export / тестами):

```
dotnet build Dictionaries.NoApp.slnf
dotnet test Dictionaries.NoApp.slnf
```
