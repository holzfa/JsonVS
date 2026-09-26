namespace JsonVS.Models;

public abstract record JsonNode;
public abstract record JsonValue : JsonNode;
public abstract record JsonLiteral : JsonValue
{
    internal JsonLiteral(string val)
    {
        Value = val;
    }
    
    internal string Value { get; }
}



public sealed record JsonInteger : JsonLiteral
{
    internal JsonInteger(string val) : base(val) { }
    
    public short Get16() => short.Parse(Value);
    public int Get32() => int.Parse(Value);
    public long Get64() => long.Parse(Value);
}

public sealed record JsonDecimal : JsonLiteral
{
    internal JsonDecimal(string val) : base(val) { }
    
    public float Get32() => float.Parse(Value);
    public double Get64() => double.Parse(Value);
}

public sealed record JsonString : JsonLiteral
{
    internal JsonString(string val) : base(val) { }

    public new string Value => base.Value;
}

public sealed record JsonBoolean : JsonLiteral
{
    internal JsonBoolean(string val) : base(val) { }
    
    public bool Get() => bool.Parse(Value);
    
    public static readonly JsonBoolean True = new("true");
    public static readonly JsonBoolean False = new("false");
}

public sealed record JsonNull() : JsonLiteral("null")
{
    public static readonly JsonNull Instance = new();
}

public sealed record JsonArray : JsonValue
{
    internal JsonArray(JsonValue[] elements) { Elements = elements; }
    
    public JsonValue[] Elements;

    public static readonly JsonArray Empty = new([]);
}