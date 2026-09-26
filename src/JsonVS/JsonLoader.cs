using System.Text.Json;

namespace JsonVS;

public static class JsonLoader
{
    /// <summary>
    /// Loads the JSON file lazily.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>The root JsonObject of the file</returns>
    public static JsonObject LoadFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return Load(content);
    }

    /// <summary>
    /// Loads the JSON text lazily.
    /// </summary>
    /// <param name="jsonContent">The content of the JSON.</param>
    /// <returns>The root object of the JSON</returns>
    public static JsonObject Load(string jsonContent)
    {
        var doc = JsonDocument.Parse(jsonContent);
        
        var jFile = new JsonObject(doc.RootElement);
        return jFile;
    }
}