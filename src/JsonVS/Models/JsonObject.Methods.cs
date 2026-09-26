using System.Diagnostics.CodeAnalysis;

namespace JsonVS.Models;

public sealed partial record JsonObject
{
    #region Property Check
    public bool Exists(string pathString) => Exists(PathParser.Parse(pathString));
    public bool Exists(JsonPath path)
    {
        JsonValue val = this;
        JsonPath? activePath = path;
        
        while (activePath is not null)
        {
            if (activePath is ScalarPath sp && val is JsonObject on)
            {
                if (on.FastProperties.TryGetValue(sp.Name, out var obj)) val = obj;
                else return false;
            }
            else if (activePath is IndexPath ip && val is JsonArray an)
            {
                if (an.Elements.Length > ip.Index) val = an.Elements[ip.Index];
                else return false;
            }
            else return false;

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
        JsonValue val = this;
        JsonPath? activePath = path;
        
        while (activePath is not null)
        {
            if (activePath is ScalarPath sp && val is JsonObject on)
            {
                if (on.FastProperties.TryGetValue(sp.Name, out var obj)) val = obj;
                else throw new JsonException("Property not found.");
            }
            else if (activePath is IndexPath ip && val is JsonArray an)
            {
                if (an.Elements.Length > ip.Index) val = an.Elements[ip.Index];
                else throw new JsonException("Index out of range.");
            }
            else throw new JsonException("Path not found.");

            activePath = activePath.Next;
        }

        // No failed attempts until the path runs out = Success
        return val;
    }

    public JsonValue this[string pathString] => Get(PathParser.Parse(pathString));
    public JsonValue this[JsonPath path] => Get(path);
    
    public T Get<T>(string pathString) where T : JsonValue => Get<T>(PathParser.Parse(pathString));
    public T Get<T>(JsonPath path) where T : JsonValue
    {
        var val = Get(path);
        if (val is not T) throw new JsonException($"Expected type '{typeof(T).FullName}', but got '{val.GetType().FullName}'.");
        return (T)val; // Safe cast as we made sure it is of type T before
    }
    #endregion
    
    #region Safe Get
    public bool TryGet(string pathString, [NotNullWhen(true)] out JsonValue? value) => TryGet(PathParser.Parse(pathString), out value);
    public bool TryGet(JsonPath path, [NotNullWhen(true)] out JsonValue? value)
    {
        JsonValue val = this;
        JsonPath? activePath = path;
        value = null;
        
        while (activePath is not null)
        {
            if (activePath is ScalarPath sp && val is JsonObject on)
            {
                if (on.FastProperties.TryGetValue(sp.Name, out var obj)) val = obj;
                else return false;
            }
            else if (activePath is IndexPath ip && val is JsonArray an)
            {
                if (an.Elements.Length > ip.Index) val = an.Elements[ip.Index];
                else return false;
            }
            else return false;

            activePath = activePath.Next;
        }

        // No failed attempts until the path runs out = Success
        value = val;
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
}