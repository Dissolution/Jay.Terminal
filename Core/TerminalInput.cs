using System.Text;

namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s Input.
/// </summary>
public sealed class TerminalInput
{
    private readonly TerminalInstance _terminal;

    /// <summary>
    /// Gets or sets the <see cref="TextReader"/> the input reads from.
    /// </summary>
    public TextReader Reader
    {
        get => Console.In;
        set => Console.SetIn(value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to read input.
    /// </summary>
    public Encoding Encoding
    {
        get => Console.InputEncoding;
        set => Console.InputEncoding = value;
    }

    /// <summary>
    /// Has the input stream been redirected from standard?
    /// </summary>
    public bool Redirected => Console.IsInputRedirected;

    /// <summary>
    /// Gets or sets whether Ctrl+C should be treated as input or as a break command.
    /// </summary>
    public bool TreatCtrlCAsInput
    {
        get => Console.TreatControlCAsInput;
        set => Console.TreatControlCAsInput = value;
    }

    internal TerminalInput(TerminalInstance terminal)
    {
        _terminal = terminal;
    }

    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    public Stream Open() => Console.OpenStandardInput();

    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public Stream Open(int bufferSize) => Console.OpenStandardInput(bufferSize);

    /// <summary>
    /// Sets the input to the specified <see cref="TextReader"/>.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public TerminalInstance SetReader(TextReader reader)
    {
        Console.SetIn(reader);
        return _terminal;
    }

    /// <summary>
    /// Sets the input <see cref="System.Text.Encoding"/>.
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public TerminalInstance SetEncoding(Encoding encoding)
    {
        Console.InputEncoding = encoding;
        return _terminal;
    }
}