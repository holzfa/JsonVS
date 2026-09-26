using JsonVS.Lexer;
using JsonVS.Models;
using JsonVS.Parser;

namespace JsonVS;

public static class JsonLoader
{
    /// <summary>
    /// Loads the JSON text lazily.
    /// </summary>
    /// <param name="jsonContent">The content of the JSON.</param>
    /// <returns>The root object of the JSON</returns>
    public static JsonObject Load(string jsonContent)
    {
        var tokens = new Tokenizer().Tokenize(jsonContent);
        var obj = new JsonParser().Parse(tokens);

        return obj;
    }

    /// <summary>
    /// Loads the JSON file lazily.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>The root JsonObject of the file</returns>
    public static JsonObject LoadFromFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return Load(content);
    }
    
    /// <summary>
    /// Constructs and returns an IJsonMeta from a given JSON string.
    /// </summary>
    /// <param name="jsonContent">The content of the JSON.</param>
    /// <typeparam name="TMeta">The IJsonMeta to construct.</typeparam>
    /// <returns>The constructed IJsonMeta.</returns>
    public static TMeta LoadMeta<TMeta>(string jsonContent) where TMeta : IJsonMeta<TMeta>
    {
        var obj = Load(jsonContent);
        return TMeta.Load(obj);
    }
    
    /// <summary>
    /// Constructs and returns an IJsonMeta from a given JSON string.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <typeparam name="TMeta">The IJsonMeta to construct.</typeparam>
    /// <returns>The constructed IJsonMeta.</returns>
    public static TMeta LoadMetaFromFile<TMeta>(string filePath) where TMeta : IJsonMeta<TMeta>
    {
        var obj = LoadFromFile(filePath);
        return TMeta.Load(obj);
    }
}