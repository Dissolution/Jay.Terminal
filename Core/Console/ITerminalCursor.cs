using System.Drawing;

namespace Jay.Terminalis.Console;

public interface ITerminalCursor
{
    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.
    /// </summary>
    int Left { get; set; }
    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area.
    /// </summary>
    int Top { get; set; }
    /// <summary>
    /// Gets or sets the cursor position within the buffer area.
    /// </summary>
    Point Position { get; set; }
    /// <summary>
    /// Gets or sets the height of the cursor within a cell.
    /// </summary>
    int Height { get; set; }
    /// <summary>
    /// Gets or sets whether the cursor is visible.
    /// </summary>
    bool Visible { get; set; }
    
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s cursor back to where it was when the <see cref="ColorLock"/> was taken.
    /// </summary>
    IDisposable CursorLock { get; }
    
    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Cursor"/> position, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Cursor"/> position.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    void TempPosition(Action<ITerminalInstance> tempAction);
}