using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace JsonVS;

public sealed record JsonObject : JsonValue
{
    internal JsonObject(JsonElement element)
    {
        _element = element;
    }
    private readonly JsonElement _element;
    
    public bool IsArray => _element.ValueKind == JsonValueKind.Array;
    public IEnumerable<(string Name, JsonValue Value)> GetProperties() => _element.EnumerateObject().Select(p => (p.Name, ToValue(p.Value)));

    public IEnumerable<(string Name, T Value)> GetProperties<T>() where T : JsonValue
    {
        List<(string, T)> result = new ();
        foreach (var prop in _element.EnumerateObject())
        {
            var val = ToValue(prop.Value);
            if (val is T t) result.Add((prop.Name, t));
        }

        return result.ToArray();
    }
    
    
    #region Property Check
    public bool Exists(string pathString) => Exists(PathParser.Parse(pathString));
    public bool Exists(JsonPath path)
    {
        var element = _element;
        JsonPath? activePath = path;
        
        while (activePath is not null)
        {
            // If its a scalarPath, try to get the property and save it
            if (activePath is ScalarPath sp)
            {
                if (element.TryGetProperty(sp.Name, out var newElement)) element = newElement;
                else return false;
            }
            // Otherwise if its an indexPath, make sure the element is an array and that the index is ok
            else if (activePath is IndexPath ip)
            {
                if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > ip.Index) element = element.EnumerateArray().ElementAt(ip.Index);
                else return false;
            }

            activePath = activePath.Next;
        }

        // No failed attempts until the path runs out = Success
        return true;
    }
    #endregion
    
    #region Unsafe Get
    public JsonValue Get(string pathString) => Get(PathParser.Parse(pathString));
    public JsonValue Get(JsonPath path)
    {
        var element = _element;
        JsonPath? activePath = path;
        
        while (activePath is not null)
        {
            // If its a scalarPath, try to get the property and save it
            if (activePath is ScalarPath sp)
            {
                if (element.TryGetProperty(sp.Name, out var newElement)) element = newElement;
                else throw new JsonException($"Property {sp.Name} does not exist");
            }
            // Otherwise if its an indexPath, make sure the element is an array and that the index is ok
            else if (activePath is IndexPath ip)
            {
                if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > ip.Index) element = element.EnumerateArray().ElementAt(ip.Index);
                else throw new JsonException($"Property is either not an array, or the index is more than the array length.");
            }

            activePath = activePath.Next;
        }

        return ToValue(element);
    }

    public JsonValue this[string pathString] => Get(PathParser.Parse(pathString));
    public JsonValue this[JsonPath path] => Get(path);
    
    public T Get<T>(string pathString) where T : JsonValue => Get<T>(PathParser.Parse(pathString));
    public T Get<T>(JsonPath path) where T : JsonValue
    {
        var val = Get(path);
        if (val is not T) throw new JsonException($"Expected '{typeof(T).FullName}', but got '{val.GetType().FullName}'.");
        return (T)val; // Safe cast as we made sure it is of type T before
    }
    #endregion
    
    #region Safe Get
    public bool TryGet(string pathString, [NotNullWhen(true)] out JsonValue? value) => TryGet(PathParser.Parse(pathString), out value);
    public bool TryGet(JsonPath path, [NotNullWhen(true)] out JsonValue? value)
    {
        var element = _element;
        JsonPath? activePath = path;
        value = null;
        
        while (activePath is not null)
        {
            // If its a scalarPath, try to get the property and save it
            if (activePath is ScalarPath sp)
            {
                if (element.TryGetProperty(sp.Name, out var newElement)) element = newElement;
                else return false;
            }
            // Otherwise if its an indexPath, make sure the element is an array and that the index is ok
            else if (activePath is IndexPath ip)
            {
                if (element.ValueKind == JsonValueKind.Array && element.GetArrayLength() > ip.Index) element = element.EnumerateArray().ElementAt(ip.Index);
                else return false;
            }

            activePath = activePath.Next;
        }

        value = element.ValueKind switch
        {
            JsonValueKind.Number => element.TryGetInt32(out var integer) 
                ? new JsonInteger(integer) 
                : new JsonDecimal(element.GetDouble()),
            JsonValueKind.String => new JsonString(element.GetString()),
            JsonValueKind.True or JsonValueKind.False => new JsonBoolean(element.ValueKind is JsonValueKind.True),
            JsonValueKind.Object => new JsonObject(element),
            JsonValueKind.Array => new JsonArray(element),
            JsonValueKind.Null or JsonValueKind.Undefined=> new JsonNull(),
            _ => throw new JsonException("Something went wrong")
        };
        
        return true;
    }

    public bool TryGet<T>(string pathString, [NotNullWhen(true)] out T? value) where T : JsonValue => TryGet<T>(PathParser.Parse(pathString), out value);
    public bool TryGet<T>(JsonPath path, [NotNullWhen(true)] out T? value) where T : JsonValue
    {
        var result = TryGet(path, out var val);
        value = null;
        
        if (val is not T actualVal) return false;
        value = actualVal;
        
        return result; // Safe cast as we made sure it is of type T before
    }
    #endregion
    
    private JsonValue ToValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.TryGetInt32(out var integer) 
            ? new JsonInteger(integer) 
            : new JsonDecimal(element.GetDouble()),
        JsonValueKind.String => new JsonString(element.GetString()),
        JsonValueKind.True or JsonValueKind.False => new JsonBoolean(element.ValueKind is JsonValueKind.True),
        JsonValueKind.Object => new JsonObject(element),
        JsonValueKind.Array => new JsonArray(element),
        JsonValueKind.Null or JsonValueKind.Undefined=> new JsonNull(),
        _ => throw new JsonException("Something went wrong")
    };
}