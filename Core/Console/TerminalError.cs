namespace Jay.Terminalis.Console;

internal class TerminalError : TerminalInstance, ITerminalError
{
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> the Error outputs to.
    /// </summary>
    public TextWriter Writer
    {
        get => GetValue(() => System.Console.Error);
        set => SetValue(System.Console.SetError, value);
    }

    /// <summary>
    /// Has the error stream been redirected from standard?
    /// </summary>
    public bool IsRedirected => GetValue(() => System.Console.IsErrorRedirected);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    public Stream OpenStream() => GetValue(System.Console.OpenStandardError);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public Stream OpenStream(int bufferSize) => GetValue(() => System.Console.OpenStandardError(bufferSize));

    
    protected TerminalError(ReaderWriterLockSlim slimLock)
        : base(slimLock) { }
}