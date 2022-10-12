namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s display (not actual) window.
/// </summary>
public sealed class TerminalDisplayWindow
{
    private readonly TerminalInstance _terminal;

    /// <summary>
    /// Gets or sets the leftmost position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    public int Left
    {
        get => Console.WindowLeft;
        set => Console.WindowLeft = value;
    }

    /// <summary>
    /// Gets or sets the top position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    public int Top
    {
        get => Console.WindowTop;
        set => Console.WindowTop = value;
    }

    /// <summary>
    /// Gets or sets the position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    public Point Position
    {
        get => new Point(Console.WindowLeft, Console.WindowTop);
        set => Console.SetWindowPosition(value.X, value.Y);
    }

    /// <summary>
    /// Gets or sets the width of the <see cref="Terminal"/> display window.
    /// </summary>
    public int Width
    {
        get => Console.WindowWidth;
        set => Console.WindowWidth = value;
    }

    /// <summary>
    /// Gets or sets the height of the <see cref="Terminal"/> display window.
    /// </summary>
    public int Height
    {
        get => Console.WindowHeight;
        set => Console.WindowHeight = value;
    }

    /// <summary>
    /// Gets or sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    public Size Size
    {
        get => new Size(Console.WindowWidth, Console.WindowHeight);
        set => Console.SetWindowSize(value.Width, value.Height);
    }

    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window rows, based on the current font, screen resolution, and window size.
    /// </summary>
    public int LargestHeight => Console.LargestWindowHeight;

    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window columns, based on the current font, screen resolution, and window size.
    /// </summary>
    public int LargestWidth => Console.LargestWindowWidth;

    internal TerminalDisplayWindow(TerminalInstance terminal)
    {
        _terminal = terminal;
    }

    /// <summary>
    /// Sets the position of the <see cref="Terminal"/> display window relative to the screen buffer.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TerminalInstance SetPosition(Point position)
    {
        Console.SetWindowPosition(position.X, position.Y);
        return _terminal;
    }

    /// <summary>
    /// Sets the position of the <see cref="Terminal"/> display window relative to the screen buffer.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    public TerminalInstance SetPosition(int left, int top)
    {
        Console.SetWindowPosition(left, top);
        return _terminal;
    }

    /// <summary>
    /// Sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public TerminalInstance SetSize(Size size)
    {
        Console.SetWindowSize(size.Width, size.Height);
        return _terminal;
    }

    /// <summary>
    /// Sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public TerminalInstance SetSize(int width, int height)
    {
        Console.SetWindowSize(width, height);
        return _terminal;
    }
}