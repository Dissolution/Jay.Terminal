using System.Drawing;

namespace Jay.Terminalis.Console;

public interface ITerminalBuffer
{
    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    int Width { get; set; }
    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    int Height { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    Size Size { get; set; }
    /// <summary>
    /// Copies a specified source <see cref="Rectangle"/> of the screen buffer to a specified destination <see cref="Point"/>.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    void Copy(Rectangle area, Point position);
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
    void Copy(int left, int top, int width, int height, int xPos, int yPos);
}