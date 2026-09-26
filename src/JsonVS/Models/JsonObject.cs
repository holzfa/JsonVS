namespace JsonVS.Models;

public sealed partial record JsonObject : JsonValue
{
    internal JsonObject(Dictionary<string, JsonValue> properties) { FastProperties = properties; }
    
    internal readonly Dictionary<string, JsonValue> FastProperties;
    
    public JsonProperty[] Properties => FastProperties.Select(kv => new  JsonProperty(kv.Key, kv.Value)).ToArray();
    public (string Name, TValue Value)[] GetProperties<TValue>() where TValue : JsonValue
        => FastProperties
            .Where(kv => kv.Value is TValue)
            .Select(kv => (kv.Key, (TValue)kv.Value))
            .ToArray();
}

public sealed record JsonProperty(string Name, JsonValue Value) : JsonNode;