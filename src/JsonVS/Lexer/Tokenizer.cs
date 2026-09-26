using System.Runtime.CompilerServices;

namespace JsonVS.Lexer;

public sealed class Tokenizer : SafeIterator<char>
{
    private int Line { get; set; } = 1;
    private int Column { get; set; } = 1;

    private List<Token> Tokens { get; set; } = [];

    public Tokenizer()
    {
        PopTask = amount =>
        {
            var skipped = Items[Index..(Index + amount)];

            foreach (var c in skipped)
            {
                switch (c)
                {
                    case '\n': Line++; Column = 1; break;
                    case '\r': Column = 1; break;
                    default: Column++; break;
                }
            }
        };
    }
    
    public Token[] Tokenize(string jsonContent)
    {
        var source = jsonContent.ToCharArray();
        Start(source);
        
        Tokens = new List<Token>(source.Length / 5);
        

        while (Index < source.Length)
        {
            char c = Peek();

            LexPass(c);
        }

        var fileSpan = new SourceSpan(TextLocation.Zero, new TextLocation(source.Count('\n') + 1, source.Length - source.LastIndexOf('\n') - 1));
        Tokens.Add(new Token(TokenType.Eof, "", fileSpan));
        return Tokens.ToArray();
    }
    
    void LexPass(char c)
    {
        // Whitespace
        if (char.IsWhiteSpace(c))
        {
            PopWhitespace();
            return;
        }

        // Number Literals (Float vs Int)
        if (char.IsAsciiDigit(c) || (c == '.' && char.IsAsciiDigit(Peek(1))))
        {
            PopNumber();
            return;
        }
        
        // String & Character Literals
        if (c is '"')
        {
            PopString();
            return;
        }

        // Operators & Punctuation
        var opToken = PopPunctuation();
        if (opToken.Type != TokenType.Unknown)
        {
            Tokens.Add(opToken);
            return;
        }

        // Unknown / Fallback
        var span = new SourceSpan(new TextLocation(Line, Column), new TextLocation(Line, Column + 1));
        Tokens.Add(new Token(TokenType.Unknown, Peek().ToString(), span));
        Pop();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ReadOnlySpan<char> PeekRange(int start, int target) => (uint)target < (uint)Items.Length ? Items[start..target] : "\0";

    void PopWhitespace()
    {
        var initialLoc = new TextLocation(Line, Column);
        
        int start = Index;
        char c = Peek();
        while (Index < Items.Length && char.IsWhiteSpace(c))
        {
            Pop();
            c = Peek();
        }
        var span = new SourceSpan(initialLoc, new TextLocation(Line, Column));
        Tokens.Add(new Token(TokenType.Whitespace, PeekRange(start, Index).ToString(), span));
    }
    void PopNumber()
    {
        var initialLoc = new TextLocation(Line, Column);
        
        int start = Index;
        bool isFloat = false;

        while (Index < Items.Length)
        {
            char current = Peek();
            
            if (char.IsAsciiDigit(current)) // handle int exclusively
            {
                Pop();
            }
            else if (current == '.' && !isFloat && char.IsAsciiDigit(Peek(1))) // handle float
            {
                isFloat = true;
                Pop();
            }
            else
            {
                break;
            }
        }

        TokenType type = isFloat ? TokenType.Decimal : TokenType.Integer;
        var span = new SourceSpan(initialLoc, new TextLocation(Line, Column));
        Tokens.Add(new Token(type, PeekRange(start, Index).ToString(), span));
    }
    void PopString()
    {
        var initialLoc = new TextLocation(Line, Column);
        
        // Pop the opening quote
        Pop();
        int start = Index;

        while (Index < Items.Length)
        {
            if (Peek() == '"')
            {
                string value = new string(Items[start..Index]);
                
                Pop(); // Pop closing quote
                
                var span = new SourceSpan(initialLoc, new TextLocation(Line, Column));
                Tokens.Add(new Token(TokenType.String, value, span));
                return;
            }
            else
            {
                Pop(); // Advance past regular characters (and newlines)
            }
        }

        // Fallback for unterminated literals at EOF
        var errSpan = new SourceSpan(initialLoc, new TextLocation(Line, Column));
        Tokens.Add(new Token(TokenType.Unknown, new string(Items[start..Index]), errSpan));
    }
    Token PopPunctuation()
    {
        var initialLoc = new TextLocation(Line, Column);
        char c = Peek();
        var span = new SourceSpan(initialLoc, new TextLocation(Line, Column));
        
        // Punctuation
        TokenType punctuation = c switch
        {
            '{' => TokenType.BracketOpen,
            '}' => TokenType.BracketClose,
            '[' => TokenType.SquareOpen,
            ']' => TokenType.SquareClose,
            ',' => TokenType.Comma,
            ':' => TokenType.Colon,
            _ => TokenType.Unknown
        };

        if (punctuation != TokenType.Unknown)
        {
            Pop();
            span = new SourceSpan(initialLoc, new TextLocation(Line, Column));
            return new Token(punctuation, c.ToString(), span);
        }

        return new Token(TokenType.Unknown, "", span);
    }
}