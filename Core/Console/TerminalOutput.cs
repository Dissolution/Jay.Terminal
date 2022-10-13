using System.Text;

namespace Jay.Terminalis.Console;

/// <summary>
/// Options related to a <see cref="Terminal"/>'s Output.
/// </summary>
internal sealed class TerminalOutput : TerminalInstance, ITerminalOutput
{
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> used for output.
    /// </summary>
    public TextWriter Writer
    {
        get => GetValue(() => SysCons.Out);
        set => SetValue(SysCons.SetOut, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to output.
    /// </summary>
    public Encoding Encoding
    {
        get => GetValue(() => SysCons.OutputEncoding);
        set => SetValue(v => SysCons.OutputEncoding = v, value);
    }

    /// <summary>
    /// Has the output stream been redirected from standard?
    /// </summary>
    public bool IsRedirected => GetValue(() => SysCons.IsOutputRedirected);

    internal TerminalOutput(ReaderWriterLockSlim slimLock)
         : base(slimLock)
    {

    }

    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    public Stream OpenStream() => GetValue(SysCons.OpenStandardOutput);

    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public Stream OpenStream(int bufferSize) => GetValue(() => SysCons.OpenStandardOutput(bufferSize));


}