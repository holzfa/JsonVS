namespace JsonVS;

public abstract record JsonPath
{
    public JsonPath? Next { get; set; }
}

public sealed record ScalarPath(string Name) : JsonPath;
public sealed record IndexPath(int Index) : JsonPath;