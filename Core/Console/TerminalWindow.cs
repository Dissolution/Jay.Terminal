/*using System.Drawing;
using Jay.Terminalis.Buff.Native;

namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s actual window.
/// </summary>
public sealed class TerminalWindow
{
    private readonly TerminalInstance _terminal;
    private readonly IntPtr _handle;

    /// <summary>
    /// Gets or sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    public string Title
    {
        get => Console.Title;
        set => Console.Title = value;
    }

    /// <summary>
    /// Gets or sets the bounds of the <see cref="Terminal"/> window.
    /// </summary>
    public Rectangle Bounds
    {
        get
        {
            // RECT rect = default;
            // if (NativeMethods.GetWindowRect(_handle, ref rect))
            //     return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
            // return Rectangle.Empty;
            throw new NotImplementedException();
        }
        set
        {
            //NativeMethods.MoveWindow(_handle, value.X, value.Y, value.Width, value.Height, true);
            throw new NotImplementedException();
        }
    }

    internal TerminalWindow(TerminalInstance terminal)
    {
        _terminal = terminal;
        _handle = NativeMethods.GetConsoleHandle();
    }

    /// <summary>
    /// Sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public TerminalInstance SetTitle(string title)
    {
        Console.Title = title;
        return _terminal;
    }

    /// <summary>
    /// Manipules the <see cref="Terminal"/> window with the specified <see cref="Jay.Show"/> command.
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    // public TerminalInstance Show(Show show)
    // {
    //     NativeMethods.ShowWindow(_handle, (int)show);
    //     return _terminal;
    // }

    /// <summary>
    /// Moves the <see cref="Terminal"/> window to the specified <see cref="Point"/>.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TerminalInstance Move(Point position) => Move(position.X, position.Y);

    /// <summary>
    /// Moves the <see cref="Terminal"/> window to the specified X and Y coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TerminalInstance Move(int x, int y)
    {
        // var bounds = this.Bounds;
        // NativeMethods.MoveWindow(_handle, x, y, bounds.Width, bounds.Height, true);
        // return _terminal;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Resize the <see cref="Terminal"/> window to the specified <see cref="Size"/>.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public TerminalInstance Resize(Size size) => Resize(size.Width, size.Height);

    /// <summary>
    /// Resize the <see cref="Terminal"/> window to the specified width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public TerminalInstance Resize(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
        var bounds = this.Bounds;
        //NativeMethods.MoveWindow(_handle, bounds.X, bounds.Y, width, height, true);
        throw new NotImplementedException();
        return _terminal;
    }

    /// <summary>
    /// Moves and/or resizes the <see cref="Terminal"/> widow to the specified <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public TerminalInstance SetBounds(Rectangle bounds)
    {
        //NativeMethods.MoveWindow(_handle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);
        throw new NotImplementedException();
        return _terminal;
    }
}*/