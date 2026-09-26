namespace JsonVS;

public struct SourceSpan(TextLocation start, TextLocation end);
public struct TextLocation(int line, int column);