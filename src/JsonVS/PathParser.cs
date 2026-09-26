using JsonVS.Models;

namespace JsonVS;

public static class PathParser
{
    public static JsonPath Parse(string path)
{
    if (string.IsNullOrEmpty(path))
        throw new ArgumentException("Path cannot be null or empty.", nameof(path));

    JsonPath? head = null;
    JsonPath? tail = null;

    var parts = path.Split('/');

    foreach (var part in parts)
    {
        if (part.Length == 0) continue; // Skip empty segments if path starts with '/'

        ReadOnlySpan<char> span = part.AsSpan();
        int bracketIndex = span.IndexOf('[');

        if (bracketIndex != 0)
        {
            ReadOnlySpan<char> scalarSpan = bracketIndex < 0 ? span : span[..bracketIndex];
            if (scalarSpan.IsEmpty)
            {
                throw new FormatException("Empty scalar property name.");
            }

            AppendNode(new ScalarPath(scalarSpan.ToString()), ref head, ref tail);

            if (bracketIndex < 0) continue; // Pure scalar segment
        }

        ReadOnlySpan<char> indexSpan = span[bracketIndex..];

        while (!indexSpan.IsEmpty)
        {
            if (indexSpan[0] != '[')
            {
                throw new FormatException("Unexpected character sequence in index segment.");
            }

            int closeIndex = indexSpan.IndexOf(']');
            if (closeIndex <= 1)
            {
                throw new FormatException("Malformed bracketed index.");
            }

            ReadOnlySpan<char> numberSpan = indexSpan[1..closeIndex];

            if (!int.TryParse(numberSpan, out int ind))
            {
                throw new FormatException("Invalid index integer.");
            }

            AppendNode(new IndexPath(ind), ref head, ref tail);

            indexSpan = indexSpan[(closeIndex + 1)..];
        }
    }

    return head ?? throw new ArgumentException("Path contained no valid segments.", nameof(path));
}

private static void AppendNode(JsonPath node, ref JsonPath? head, ref JsonPath? tail)
{
    if (head is null)
    {
        head = node;
        tail = node;
    }
    else
    {
        tail!.Next = node;
        tail = node;
    }
}
}