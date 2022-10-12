namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s buffer area.
/// </summary>
public sealed class TerminalBuffer
{
    private readonly TerminalInstance _terminal;

    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    public int Width
    {
        get => Console.BufferWidth;
        set => Console.BufferWidth = value;
    }

    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    public int Height
    {
        get => Console.BufferHeight;
        set => Console.BufferHeight = value;
    }

    /// <summary>
    /// Gets or sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    public Size Size
    {
        get => new Size(Console.BufferWidth, Console.BufferHeight);
        set => Console.SetBufferSize(value.Width, value.Height);
    }

    internal TerminalBuffer(TerminalInstance terminal)
    {
        _terminal = terminal;
    }

    /// <summary>
    /// Sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public TerminalInstance SetSize(Size size)
    {
        Console.SetBufferSize(size.Width, size.Height);
        return _terminal;
    }

    /// <summary>
    /// Sets the width and height of the buffer area.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public TerminalInstance SetSize(int width, int height)
    {
        Console.SetBufferSize(width, height);
        return _terminal;
    }

    /// <summary>
    /// Copies a specified source <see cref="Rectangle"/> of the screen buffer to a specified destination <see cref="Point"/>.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public TerminalInstance Copy(Rectangle area, Point position)
    {
        Console.MoveBufferArea(area.Left, area.Top, area.Width, area.Height, position.X, position.Y);
        return _terminal;
    }

    /// <summary>
    /// Copies a specified source area of the screen buffer to a specified destination area.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <returns></returns>
    public TerminalInstance Copy(int left, int top, int width, int height, int xPos, int yPos)
    {
        Console.MoveBufferArea(left, top, width, height, xPos, yPos);
        return _terminal;
    }
}