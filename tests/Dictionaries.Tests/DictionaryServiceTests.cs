using Dictionaries.Core.Exceptions;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;
using Dictionaries.Core.Repositories;
using Dictionaries.Core.Services;
using Xunit;

namespace Dictionaries.Tests;

public class DictionaryServiceTests
{
    private readonly IDictionaryService _service;
    private readonly WordDictionary _testDict;

    public DictionaryServiceTests()
    {
        IDictionaryRepository repo = new InMemoryDictionaryRepository();
        _service = new DictionaryService(repo);
        _testDict = _service.CreateDictionary("Англо-український", "Англійська", "Українська");
    }

    [Fact]
    public void RemoveTranslation_LastTranslation_ShouldThrowDictionaryException()
    {
        _service.AddWord(_testDict.Id, "cat", new[] { "кіт" });

        Assert.Throws<DictionaryException>(() =>
            _service.RemoveTranslation(_testDict.Id, "cat", "кіт"));
    }

    [Fact]
    public void RemoveWord_ShouldRemoveWordAndAllTranslations()
    {
        _service.AddWord(_testDict.Id, "run", new[] { "бігти", "керувати" });

        _service.RemoveWord(_testDict.Id, "run");

        var found = _service.FindWord(_testDict.Id, "run");
        Assert.Null(found);
    }

    [Fact]
    public void AddWord_DuplicateWord_ShouldThrowDictionaryException()
    {
        _service.AddWord(_testDict.Id, "book", new[] { "книга" });

        Assert.Throws<DictionaryException>(() =>
            _service.AddWord(_testDict.Id, "BOOK", new[] { "бронювати" }));
    }

    [Fact]
    public void FindWord_ShouldBeCaseInsensitive()
    {
        _service.AddWord(_testDict.Id, "House", new[] { "будинок", "дім" });

        var found = _service.FindWord(_testDict.Id, "hOuSe");

        Assert.NotNull(found);
        Assert.Equal("House", found.Word);
        Assert.Equal(2, found.Translations.Count);
    }

    [Fact]
    public void ReplaceWordAndTranslation_ShouldUpdateValues()
    {
        _service.AddWord(_testDict.Id, "aple", new[] { "яблуко" });

        _service.ReplaceWord(_testDict.Id, "aple", "apple");
        _service.ReplaceTranslation(_testDict.Id, "apple", "яблуко", "яблуня");

        var found = _service.FindWord(_testDict.Id, "apple");
        Assert.NotNull(found);
        Assert.Contains("яблуня", found.Translations);
        Assert.DoesNotContain("яблуко", found.Translations);
    }
}
