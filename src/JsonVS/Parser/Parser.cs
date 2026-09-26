using JsonVS.Lexer;
using JsonVS.Models;

namespace JsonVS.Parser;

public class JsonParser : SafeIterator<Token>
{
    private record Property(string Name, JsonValue Value);
    
    public JsonObject Parse(Token[] rawTokens)
    {
        var filteredTokens = rawTokens.Where(t => t.Type is not TokenType.Unknown and not TokenType.Whitespace).ToArray();
        Start(filteredTokens);

        var obj = PopObject();
        return obj;
    }

    private Property PopProperty()
    {
        var startSpan = Peek().Span;
        
        var name = ExpectType(TokenType.String, "Expected property name.");
        ExpectType(TokenType.Colon, "Expected ':'");
        var value = PopValue();
        
        return new Property(name.Value, value);
    }
    private JsonValue PopValue()
    {
        return Peek().Type switch
        {
            TokenType.BracketOpen => PopObject(),
            TokenType.SquareOpen => PopArray(),
            _ => PopLiteral()
        };
    }

    #region Values
    private JsonObject PopObject()
    {
        List<Property> properties = [];
        ExpectType(TokenType.BracketOpen, "Expected '{'");
        while (Peek().Type is not TokenType.Eof and not TokenType.BracketClose)
        {
            properties.Add(PopProperty());

            if (Peek().Type is not TokenType.BracketClose)
            {
                ExpectType(TokenType.Comma, "Expected ','");
            }
        }
        ExpectType(TokenType.BracketClose, "Expected '}'");
        
        return new JsonObject(properties.ToDictionary(p => p.Name, p => p.Value));
    }
    private JsonArray PopArray()
    {
        List<JsonValue> values = [];
        ExpectType(TokenType.SquareOpen, "Expected '['");
        while (Peek().Type is not TokenType.Eof and not TokenType.SquareClose)
        {
            values.Add(PopValue());

            if (Peek().Type is not TokenType.SquareClose)
            {
                ExpectType(TokenType.Comma, "Expected ','");
            }
        }
        ExpectType(TokenType.SquareClose, "Expected ']'");
        
        return new JsonArray(values.ToArray());
    }
    private JsonValue PopLiteral()
    {
        var token = Pop();
        return token.Type switch
        {
            TokenType.String => new JsonString(token.Value),
            TokenType.Integer => new JsonInteger(token.Value),
            TokenType.Decimal => new JsonDecimal(token.Value),
            TokenType.Boolean => new JsonBoolean(token.Value),
            _ => new JsonNull()
        };
    }
    #endregion
    
    #region Errors
    private Token ExpectType(TokenType type, string message) => Expect(t => t.Type == type, message, Peek().Span);
    #endregion
}