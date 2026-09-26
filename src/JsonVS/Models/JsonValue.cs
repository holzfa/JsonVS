using System.Text.Json;

namespace JsonVS;

public abstract record JsonValue;
public abstract record JsonLiteral<T> : JsonValue
{
    internal JsonLiteral(T val)
    {
        Value = val;
    }
    
    public T Value { get; }
}



public sealed record JsonInteger : JsonLiteral<int>
{
    internal JsonInteger(int val) : base(val) { }
}

public sealed record JsonDecimal : JsonLiteral<double>
{
    internal JsonDecimal(double val) : base(val) { }
}

public sealed record JsonString : JsonLiteral<string>
{
    internal JsonString(string val) : base(val) { }
}

public sealed record JsonBoolean : JsonLiteral<bool>
{
    internal JsonBoolean(bool val) : base(val) { }
    
    public static readonly JsonBoolean True = new(true);
    public static readonly JsonBoolean False = new(false);
}

public sealed record JsonNull : JsonValue
{
    public static readonly JsonNull Instance = new();
}

public sealed record JsonArray : JsonValue
{
    internal JsonArray(JsonElement element) { _element = element; }
    
    private JsonElement _element;

    public static readonly JsonArray Empty = new(new JsonElement());
}