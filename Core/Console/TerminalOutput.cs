/*using System.Text;

namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s Output.
/// </summary>
public sealed class TerminalOutput
{
    private readonly TerminalInstance _terminal;

    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> used for output.
    /// </summary>
    public TextWriter Writer
    {
        get => Console.Out;
        set => Console.SetOut(value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to output.
    /// </summary>
    public Encoding Encoding
    {
        get => Console.OutputEncoding;
        set => Console.OutputEncoding = value;
    }

    /// <summary>
    /// Has the output stream been redirected from standard?
    /// </summary>
    public bool Redirected => Console.IsOutputRedirected;

    internal TerminalOutput(TerminalInstance terminal)
    {
        _terminal = terminal;
    }

    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    public static Stream Open() => Console.OpenStandardOutput();

    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static Stream Open(int bufferSize) => Console.OpenStandardOutput(bufferSize);

    /// <summary>
    /// Sets the output to the specified <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    public TerminalInstance SetWriter(TextWriter writer)
    {
        Console.SetOut(writer);
        return _terminal;
    }

    /// <summary>
    /// Sets the output <see cref="System.Text.Encoding"/>.
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public TerminalInstance SetEncoding(Encoding encoding)
    {
        Console.OutputEncoding = encoding;
        return _terminal;
    }
}*/