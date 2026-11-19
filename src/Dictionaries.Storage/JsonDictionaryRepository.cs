using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Dictionaries.Core.Interfaces;
using Dictionaries.Core.Models;

namespace Dictionaries.Storage;

public class JsonDictionaryRepository : IDictionaryRepository
{
    private readonly string _dataFolder;
    private readonly JsonSerializerOptions _options;

    public JsonDictionaryRepository(string dataFolder = "Data")
    {
        _dataFolder = dataFolder;
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }

        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
    }

    public IReadOnlyList<WordDictionary> LoadAll()
    {
        var result = new List<WordDictionary>();
        if (!Directory.Exists(_dataFolder))
            return result;

        foreach (var file in Directory.GetFiles(_dataFolder, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(file);
                var dict = JsonSerializer.Deserialize<WordDictionary>(json, _options);
                if (dict != null)
                {
                    result.Add(dict);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка читання файлу {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        return result;
    }

    public void Save(WordDictionary dictionary)
    {
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }

        string filePath = Path.Combine(_dataFolder, $"{dictionary.Id}.json");
        string json = JsonSerializer.Serialize(dictionary, _options);
        File.WriteAllText(filePath, json);
    }

    public void Delete(Guid id)
    {
        string filePath = Path.Combine(_dataFolder, $"{id}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
