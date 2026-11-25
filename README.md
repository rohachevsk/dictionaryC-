# Словники (`dictionaryC-`)

Командний проєкт на **C# / .NET MAUI**: кросплатформний та настільний застосунок для роботи зі словниками різними мовами з пошуком перекладу, збереженням даних у JSON та експортом словникових записів у файли.

---

## Про проєкт і можливості

**Словники (`dictionaryC-`)** створено для зручного ведення мовних словників, швидкого пошуку слів і перекладів, збереження даних у локальному сховищі та вивантаження словникових записів у зовнішні формати.

### Основні можливості:
- **Робота зі словниками та пошук перекладу:** створення й перегляд словників, додавання нових слів із кількома варіантами перекладу, пошук та керування словниковими записами (`WordEntry`) через `DictionaryService` (`Dictionaries.Core`).
- **Локальне зберігання у JSON:** читання та запис словників у JSON-файлах через `JsonDictionaryRepository` (`Dictionaries.Storage`).
- **Експорт у TXT та JSON:** експорт словникових записів у формати **TXT** (`TxtExportService`) та **JSON** (`JsonExportService`) із підтримкою кодування UTF-8 та автоматичним створенням цільової директорії за потреби (`Dictionaries.Export`).
- **Інтерфейс користувача (.NET MAUI):** навігація між екранами, перегляд і створення словників, робота зі словами та окремий екран експорту (`ExportPage` і `ExportViewModel`) у `Dictionaries.App`.
- **Модульне тестування:** автоматична перевірка бізнес-логіки, сховища та сервісів експорту за допомогою xUnit-тестів (`Dictionaries.Tests`).

---

## Структура solution

Рішення побудоване за багатошаровою архітектурою з чітким розділенням відповідальності між бібліотеками, тестами та клієнтським застосунком:

```text
Dictionaries.sln
├── src/
│   ├── Dictionaries.Core      # Моделі, інтерфейси, DictionaryService (бізнес-логіка)
│   ├── Dictionaries.Storage   # Зберігання словників у JSON-файлах (→ Core)
│   ├── Dictionaries.Export    # Експорт слова у TXT / JSON (→ Core)
│   └── Dictionaries.App       # Застосунок .NET MAUI (→ Core, Storage, Export)
├── tests/
│   └── Dictionaries.Tests     # xUnit-тести (→ Core, Storage, Export)
├── Data/                      # Приклади словникових даних у форматі JSON
├── Dictionaries.sln           # Повне рішення (включно з .NET MAUI застосунком)
└── Dictionaries.NoApp.slnf    # Фільтр рішення для збірки та запуску тестів без MAUI

## Формати експорту
Застосунок підтримує експорт словникових записів у два формати у кодуванні UTF-8:

1. Текстовий формат (.txt) — TxtExportService
Слово записується в першому рядку, а кожен варіант перекладу — з нового рядка через дефіс:

Plaintext
apple
- яблуко
- яблуня

2. Формат JSON (.json) — JsonExportService
Словниковий запис серіалізується у структурований JSON зі збереженням кирилиці:

JSON
{
  "Word": "apple",
  "Translations": [
    "яблуко",
    "яблуня"
  ]
}

## Збірка і запуск
### Вимоги
Встановлений .NET SDK 10 (версію закріплено у global.json).

Для запуску застосунку .NET MAUI потрібне робоче навантаження MAUI (через Visual Studio Installer → «Розробка застосунків .NET MAUI» або команда dotnet workload install maui).

Збірка і запуск застосунку для Windows (не потрібен Android SDK)

dotnet build src/Dictionaries.App/Dictionaries.App.csproj -f net10.0-windows10.0.19041.0
dotnet run --project src/Dictionaries.App/Dictionaries.App.csproj -f net10.0-windows10.0.19041.0

Збірка всього Dictionaries.sln без параметра -f збирає додатково цільові платформи Android, iOS та Mac Catalyst, тому для неї потрібен Android SDK. Для щоденної роботи та запуску застосунку на Windows це не обов'язково.

## Бібліотеки і тести без MAUI

dotnet build Dictionaries.NoApp.slnf
dotnet test Dictionaries.NoApp.slnf

## Склад команди
Ваня — архітектура solution, базові моделі, інтерфейси, сховище даних та DictionaryService.

Аріна — навігація в застосунку .NET MAUI, сторінки інтерфейсу користувача та інтеграція екранів.

Тарас — сервіси експорту у TXT та JSON (TxtExportService, JsonExportService), інтерфейс і логіка сторінки експорту (ExportPage, ExportViewModel), тести, приклади даних та документація проєкту (README.md).