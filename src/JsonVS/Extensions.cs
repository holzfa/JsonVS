using JsonVS.Lexer;

namespace JsonVS;

public static class Extensions
{
    internal static SourceSpan From(this SourceSpan span, TextLocation target) => span with { Start = target };
    internal static SourceSpan From(this SourceSpan span, SourceSpan target) => span.From(target.Start);
    internal static SourceSpan To(this SourceSpan span, TextLocation target) => span with { End = target };
    internal static SourceSpan To(this SourceSpan span, SourceSpan target) => span.To(target.End);

    internal static string Repeat(this string text, int amount)
    {
        string result = "";
        for (int i = 0; i < amount; i++)
        {
            result += text;
        }
        return result;
    }
}