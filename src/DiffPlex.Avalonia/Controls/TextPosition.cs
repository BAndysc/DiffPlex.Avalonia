namespace DiffPlex.Avalonia.Controls;

internal readonly struct TextPosition : IComparable<TextPosition>
{
    public int CompareTo(TextPosition other)
    {
        var lineComparison = line.CompareTo(other.line);
        if (lineComparison != 0) return lineComparison;
        return character.CompareTo(other.character);
    }

    public static bool operator <(TextPosition left, TextPosition right) => left.CompareTo(right) < 0;

    public static bool operator >(TextPosition left, TextPosition right) => left.CompareTo(right) > 0;

    public static bool operator <=(TextPosition left, TextPosition right) => left.CompareTo(right) <= 0;

    public static bool operator >=(TextPosition left, TextPosition right) => left.CompareTo(right) >= 0;

    private readonly int line;

    private readonly int character;

    public TextPosition(int line, int character)
    {
        this.line = line + 1;
        this.character = character + 1;
    }

    public int Line => line - 1;

    public int Character => character - 1;

    public override string ToString()
    {
        return $"Line: {line}, Character: {character}";
    }
}