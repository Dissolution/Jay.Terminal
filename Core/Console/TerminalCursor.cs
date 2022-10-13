using System.Drawing;

namespace Jay.Terminalis.Console;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s cursor.
/// </summary>
internal sealed class TerminalCursor : TerminalInstance
{

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

    internal TerminalCursor(ReaderWriterLockSlim slimLock)
        : base(slimLock)
    {

    }
}