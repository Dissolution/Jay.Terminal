using System.Text;

namespace Jay.Terminalis.Console;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s Input.
/// </summary>
internal sealed class TerminalInput : TerminalInstance, ITerminalInput
{
    /// <summary>
    /// Gets or sets the <see cref="TextReader"/> the input reads from.
    /// </summary>
    public TextReader Reader
    {
        get => GetValue(() => SysCons.In);
        set => SetValue(SysCons.SetIn, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to read input.
    /// </summary>
    public Encoding Encoding
    {
        get => GetValue(() => SysCons.InputEncoding);
        set => SetValue(v => SysCons.InputEncoding = v, value);
    }

    /// <summary>
    /// Has the input stream been redirected from standard?
    /// </summary>
    public bool IsRedirected => GetValue(() => SysCons.IsInputRedirected);

    /// <summary>
    /// Gets or sets whether Ctrl+C should be treated as input or as a break command.
    /// </summary>
    public bool TreatCtrlCAsInput
    {
        get => GetValue(() => SysCons.TreatControlCAsInput);
        set => SetValue(v => SysCons.TreatControlCAsInput = v, value);
    }

    internal TerminalInput(ReaderWriterLockSlim slimLock)
        : base(slimLock)
    {

    }

    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    public Stream OpenStream() => GetValue(() => SysCons.OpenStandardInput());

    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public Stream OpenStream(int bufferSize) => GetValue(() => SysCons.OpenStandardInput(bufferSize));
}