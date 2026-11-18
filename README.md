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

Збірка і запуск застосунку для Windows (не потрібен Android SDK):

```
dotnet build src/Dictionaries.App/Dictionaries.App.csproj -f net10.0-windows10.0.19041.0
dotnet run --project src/Dictionaries.App/Dictionaries.App.csproj -f net10.0-windows10.0.19041.0
```

Збірка всього `Dictionaries.sln` без `-f` збирає ще й Android, iOS і Mac Catalyst,
тому для неї потрібен Android SDK. Для роботи над проєктом це не обов'язково.

Бібліотеки і тести без MAUI (наприклад, для роботи над Storage / Export / тестами):

```
dotnet build Dictionaries.NoApp.slnf
dotnet test Dictionaries.NoApp.slnf
```
