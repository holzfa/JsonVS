using JsonVS.Models;

namespace JsonVS;

public class JsonException : Exception
{
    public JsonException(string message) : base(message) { }
    public JsonException(string message, JsonNode value) : base(message) { Value = value; }
    public JsonException(string message, Exception inner) : base(message, inner) { }

    internal JsonNode? Value = null;
}