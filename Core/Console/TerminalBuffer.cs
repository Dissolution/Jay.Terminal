using System.Drawing;

namespace Jay.Terminalis.Console;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s buffer area.
/// </summary>
internal sealed class TerminalBuffer : TerminalInstance, ITerminalBuffer
{
    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    public int Width
    {
        get => GetValue(() => SysCons.BufferWidth);
        set => SetValue(v => SysCons.BufferWidth = v, value);
    }

    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    public int Height
    {
        get => GetValue(() => SysCons.BufferHeight);
        set => SetValue(v => SysCons.BufferHeight = v, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    public Size Size
    {
        get => new Size(Width, Height);
        set => SetValue(v => SysCons.SetBufferSize(v.Width, v.Height), value);
    }

    internal TerminalBuffer(ReaderWriterLockSlim slimLock)
        : base(slimLock)
    {

    }

    /// <summary>
    /// Copies a specified source <see cref="Rectangle"/> of the screen buffer to a specified destination <see cref="Point"/>.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public void Copy(Rectangle area, Point position)
    {
        Interact(() =>
        {
            SysCons.MoveBufferArea(area.Left, area.Top, area.Width, area.Height, position.X, position.Y);
        });
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
    public void Copy(int left, int top, int width, int height, int xPos, int yPos)
    {
        Interact(() =>
        {
            SysCons.MoveBufferArea(left, top, width, height, xPos, yPos);
        });
    }
}