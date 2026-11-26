using Dictionaries.Core.Models;
using Dictionaries.Storage;
using Xunit;

namespace Dictionaries.Tests;

public class JsonDictionaryRepositoryTests : IDisposable
{
    private readonly string _tempFolder;
    private readonly JsonDictionaryRepository _repository;

    public JsonDictionaryRepositoryTests()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), $"DictTest_{Guid.NewGuid()}");
        _repository = new JsonDictionaryRepository(_tempFolder);
    }

    [Fact]
    public void SaveAndLoadAll_ShouldPersistDictionaryWithCyrillic()
    {
        var dict = new WordDictionary
        {
            Name = "Англо-український",
            SourceLanguage = "Англійська",
            TargetLanguage = "Українська",
            Words = new List<WordEntry>
            {
                new WordEntry { Word = "apple", Translations = new List<string> { "яблуко", "яблуня" } }
            }
        };

        _repository.Save(dict);
        var loaded = _repository.LoadAll();

        Assert.Single(loaded);
        Assert.Equal(dict.Id, loaded[0].Id);
        Assert.Equal("Англо-український", loaded[0].Name);
        Assert.Contains("яблуко", loaded[0].Words[0].Translations);
    }

    [Fact]
    public void Delete_ShouldRemoveDictionaryFile()
    {
        var dict = new WordDictionary { Name = "Тест", SourceLanguage = "EN", TargetLanguage = "UA" };
        _repository.Save(dict);

        _repository.Delete(dict.Id);
        var loaded = _repository.LoadAll();

        Assert.Empty(loaded);
    }

    [Fact]
    public void LoadAll_ShouldSkipCorruptedJsonWithoutCrashing()
    {
        var validDict = new WordDictionary { Name = "Валідний", SourceLanguage = "EN", TargetLanguage = "UA" };
        _repository.Save(validDict);

        File.WriteAllText(Path.Combine(_tempFolder, "corrupted.json"), "{ invalid json text ::: ");

        var loaded = _repository.LoadAll();

        Assert.Single(loaded);
        Assert.Equal("Валідний", loaded[0].Name);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempFolder))
            Directory.Delete(_tempFolder, true);
    }
}
