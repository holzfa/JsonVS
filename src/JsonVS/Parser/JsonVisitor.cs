using JsonVS.Models;

namespace JsonVS.Parser;

public abstract class JsonVisitor<T>
{
    protected T Visit(JsonNode val) => val switch
    {
        // Values
        JsonObject n => VisitObject(n),
        JsonArray n => VisitArray(n),
        JsonLiteral n => VisitLiteral(n),
        JsonProperty n => VisitProperty(n),
        
        // Default
        _ => Default(val)
    };

    protected abstract T Default(JsonNode node);

    protected virtual T VisitProperty(JsonProperty node) => Default(node);
    
    protected virtual T VisitObject(JsonObject node) => Default(node);
    protected virtual T VisitArray(JsonArray node) => Default(node);
    protected virtual T VisitLiteral(JsonLiteral node) => Default(node);
}