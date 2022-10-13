using System.Text;

namespace Jay.Terminalis.Console;

public interface ITerminalOutput
{
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
}