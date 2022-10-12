namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s cursor.
/// </summary>
public sealed class TerminalCursor
{
    private readonly TerminalInstance _terminal;

    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.
    /// </summary>
    public int Left
    {
        get => Console.CursorLeft;
        set => Console.CursorLeft = value;
    }

    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area.
    /// </summary>
    public int Top
    {
        get => Console.CursorTop;
        set => Console.CursorTop = value;
    }

    /// <summary>
    /// Gets or sets the cursor position within the buffer area.
    /// </summary>
    public Point Position
    {
        get => new Point(Console.CursorLeft, Console.CursorTop);
        set => Console.SetCursorPosition(value.X, value.Y);
    }

    /// <summary>
    /// Gets or sets the height of the cursor within a cell.
    /// </summary>
    public int Height
    {
        get => Console.CursorSize;
        set => Console.CursorSize = value;
    }

    /// <summary>
    /// Gets or sets whether the cursor is visible.
    /// </summary>
    public bool Visible
    {
        get => Console.CursorVisible;
        set => Console.CursorVisible = value;
    }

    internal TerminalCursor(TerminalInstance terminal)
    {
        _terminal = terminal;
    }

    /// <summary>
    /// Sets the position of the cursor.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TerminalInstance SetPosition(Point position)
    {
        Console.SetCursorPosition(position.X, position.Y);
        return _terminal;
    }

    /// <summary>
    /// Sets the position of the cursor.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    public TerminalInstance SetPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
        return _terminal;
    }

    /// <summary>
    /// Sets the height of the cursor.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public TerminalInstance SetHeight(int size)
    {
        Console.CursorSize = size;
        return _terminal;
    }

    /// <summary>
    /// Sets the cursor's visibility.
    /// </summary>
    /// <param name="visible"></param>
    /// <returns></returns>
    public TerminalInstance SetVisible(bool visible)
    {
        Console.CursorVisible = visible;
        return _terminal;
    }
}