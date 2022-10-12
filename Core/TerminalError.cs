namespace Jay.Terminalis;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s Error output.
/// </summary>
public sealed class TerminalError
{
    private readonly TerminalInstance _terminal;

    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> the Error outputs to.
    /// </summary>
    public TextWriter Writer
    {
        get => Console.Error;
        set => Console.SetError(value);
    }

    /// <summary>
    /// Has the error stream been redirected from standard?
    /// </summary>
    public bool Redirected => Console.IsErrorRedirected;

    internal TerminalError(TerminalInstance terminal)
    {
        _terminal = terminal;
    }

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    public Stream Open() => Console.OpenStandardError();

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public Stream Open(int bufferSize) => Console.OpenStandardError(bufferSize);

    /// <summary>
    /// Sets the error output to the specified <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    public TerminalInstance SetWriter(TextWriter writer)
    {
        Console.SetError(writer);
        return _terminal;
    }
}