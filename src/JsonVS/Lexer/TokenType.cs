namespace JsonVS.Lexer;

public enum TokenType
{
    // Literals
    String,
    Integer,
    Decimal,
    Boolean,
    Null,
    
    // Punctuation
    BracketOpen,
    BracketClose,
    SquareOpen,
    SquareClose,
    Colon,
    Comma,
    
    // Special
    Whitespace,
    Unknown,
    Eof,
}