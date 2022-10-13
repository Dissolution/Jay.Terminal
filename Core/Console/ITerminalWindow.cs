using System.Drawing;

namespace Jay.Terminalis.Console;

public interface ITerminalWindow
{
    /// <summary>
    /// Gets or sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    string Title { get; set; }

    Point Location { get; set; }

    Size Size { get; set; }

    /// <summary>
    /// Gets or sets the bounds of the <see cref="Terminal"/> window.
    /// </summary>
    Rectangle Bounds { get; set; }
}