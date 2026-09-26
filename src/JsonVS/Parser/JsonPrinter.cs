using JsonVS.Models;

namespace JsonVS.Parser;

public class JsonPrinter(int indentAmount = 2) : JsonVisitor<string>
{
    private int _indentLevel = 0;
    private string Indent(string text) => " ".Repeat(indentAmount).Repeat(_indentLevel) + text;

    public void Print(JsonObject obj) => Console.WriteLine(Run(obj));
    public string Run(JsonObject obj) => Visit(obj);

    protected override string Default(JsonNode _) => "<JsonNode>";
    protected override string VisitProperty(JsonProperty node) => $"\"{node.Name}\": {Visit(node.Value)}";

    private string Container<TNode>(TNode[] nodes, char startChar, char endChar) where TNode : JsonNode
    {
        var result = startChar + "\n";
        
        _indentLevel++;
        for (int i = 0; i < nodes.Length; i++)
        {
            result += Indent(Visit(nodes[i]));

            // Not the last element in the container
            if (i < nodes.Length - 1)
            {
                result += ",\n";
            }
        }
        _indentLevel--;
        result += "\n" + Indent(endChar.ToString());
        
        return result;
    }

    protected override string VisitObject(JsonObject node) => Container(node.Properties, '{', '}');
    protected override string VisitArray(JsonArray node) => Container(node.Elements, '[', ']');
    protected override string VisitLiteral(JsonLiteral node)
    {
        if (node is JsonString str) return $"\"{str.Value}\"";
        return node.Value;
    }
}