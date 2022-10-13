using System.Text;

namespace Jay.Terminalis.Console;

public interface ITerminalInput
{
    /// <summary>
    /// Gets or sets the <see cref="TextReader"/> the input reads from.
    /// </summary>
    TextReader Reader { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to read input.
    /// </summary>
    Encoding Encoding { get; set; }
    /// <summary>
    /// Has the input stream been redirected from standard?
    /// </summary>
    bool IsRedirected { get; }
    /// <summary>
    /// Gets or sets whether Ctrl+C should be treated as input or as a break command.
    /// </summary>
    bool TreatCtrlCAsInput { get; set; }
    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream OpenStream();
    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream OpenStream(int bufferSize);
}