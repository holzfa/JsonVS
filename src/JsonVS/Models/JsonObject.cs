using System.Text.Json;

namespace JsonVS;

public sealed partial record JsonObject : JsonValue
{
    internal JsonObject(JsonElement element) { Element = element; }
    internal readonly JsonElement Element;
    
    public bool IsArray => Element.ValueKind == JsonValueKind.Array;
    
    public IEnumerable<(string Name, JsonValue Value)> GetProperties() => Element.EnumerateObject().Select(p => (p.Name, ToValue(p.Value)));
    public IEnumerable<(string Name, T Value)> GetProperties<T>() where T : JsonValue
    {
        List<(string, T)> result = new ();
        foreach (var prop in Element.EnumerateObject())
        {
            var val = ToValue(prop.Value);
            if (val is T t) result.Add((prop.Name, t));
        }

        return result.ToArray();
    }
    
    private JsonValue ToValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.TryGetInt32(out var integer) 
            ? new JsonInteger(integer) 
            : new JsonDecimal(element.GetDouble()),
        JsonValueKind.String => new JsonString(element.GetString()!),
        JsonValueKind.True or JsonValueKind.False => new JsonBoolean(element.ValueKind is JsonValueKind.True),
        JsonValueKind.Object => new JsonObject(element),
        JsonValueKind.Array => new JsonArray(element),
        JsonValueKind.Null or JsonValueKind.Undefined=> new JsonNull(),
        _ => throw new JsonException("Something went wrong")
    };
}