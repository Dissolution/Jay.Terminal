using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Jay.Terminalis.Colors;

namespace Jay.Terminalis.Console;

public interface ITerminalOutput
{
    TerminalColor DefaultForeColor { get; }
    TerminalColor DefaultBackColor { get; }
    
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> used for output.
    /// </summary>
    TextWriter Writer { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to output.
    /// </summary>
    Encoding Encoding { get; set; }
    /// <summary>
    /// Has the output stream been redirected from standard?
    /// </summary>
    bool IsRedirected { get; }

    /// <summary>
    /// Gets or sets the foreground <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    TerminalColor ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    TerminalColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IPalette"/> in use for this <see cref="Terminal"/>.
    /// </summary>
    IPalette Palette { get; set; }

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s colors back to what they were when the <see cref="ColorLock"/> was taken.
    /// </summary>
    IDisposable ColorLock { get; }
    
    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream OpenStream();
    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream OpenStream(int bufferSize);

    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Color"/>s, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Color"/>s.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    void TempColor(Action<ITerminalInstance> tempAction);
    
    /// <summary>
    /// Resets the <see cref="ForegroundColor"/> and <see cref="BackgroundColor"/> to their default <see cref="System.Drawing.Color"/> values.
    /// </summary>
    /// <returns></returns>
    void ResetColor();

    /// <summary>
    /// Sets the foreground <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <returns></returns>
    void SetForeColor(TerminalColor foreColor);

    /// <summary>
    /// Sets the background <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    void SetBackColor(TerminalColor backColor);
    
    /// <summary>
    /// Sets the foreground and background <see cref="System.Drawing.Color"/>s for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <returns></returns>
    void SetColors(TerminalColor? foreColor = null, TerminalColor? backColor = null);
    
    /// <summary>
    /// Writes the text representation of the given <paramref name="value"/> to the standard <see cref="Output"/> stream.
    /// </summary>
    void Write<T>([AllowNull] T value);

    /// <summary>
    /// Writes the specified array of Unicode characters to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    void Write(ReadOnlySpan<char> text);

    /// <summary>
    /// Writes the specified <see cref="FormattableString"/> value to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
    /// </summary>
    /// <param name="interpolatedText"></param>
    /// <returns></returns>
    void Write(ref DefaultInterpolatedStringHandler interpolatedText);
    
    /// <summary>
    /// Writes the current line terminator (<see cref="Environment.NewLine"/>) to the standard output stream.
    /// </summary>
    /// <returns></returns>
    void WriteLine();

    /// <summary>
    /// Writes the text representation of the given <paramref name="value"/> followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    void WriteLine<T>([AllowNull] T value);

    /// <summary>
    /// Writes the specified array of Unicode characters, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    void WriteLine(ReadOnlySpan<char> text);

    void WriteLine(ref DefaultInterpolatedStringHandler interpolatedText);

}