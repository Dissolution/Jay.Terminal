using System.Drawing;

namespace Jay.Terminalis.Console;

public interface ITerminalDisplay
{
    /// <summary>
    /// Gets or sets the leftmost position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    int Left { get; set; }
    /// <summary>
    /// Gets or sets the top position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    int Top { get; set; }
    /// <summary>
    /// Gets or sets the position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    Point Position { get; set; }
    /// <summary>
    /// Gets or sets the width of the <see cref="Terminal"/> display window.
    /// </summary>
    int Width { get; set; }
    /// <summary>
    /// Gets or sets the height of the <see cref="Terminal"/> display window.
    /// </summary>
    int Height { get; set; }
    /// <summary>
    /// Gets or sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    Size Size { get; set; }
    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window rows, based on the current font, screen resolution, and window size.
    /// </summary>
    int LargestHeight { get; }
    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window columns, based on the current font, screen resolution, and window size.
    /// </summary>
    int LargestWidth { get; }
    

    /// <summary>
    /// Clears the <see cref="Terminal"/> buffer and corresponding <see cref="DisplayWindow"/> of display information.
    /// </summary>
    void Clear();
}