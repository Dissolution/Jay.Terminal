using System.Security;
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
    /// Gets a value indicating whether a key press is available in the input stream.
    /// </summary>
    bool KeyAvailable { get; }

    /// <summary>
    /// Gets a value indicating whether CAPS LOCK is toggled on or off.
    /// </summary>
    bool CapsLock  { get; }

    /// <summary>
    /// Gets a value indicating whether NUMBER LOCK is toggled on or off.
    /// </summary>
    bool NumberLock  { get; }
    
    /// <summary>
    /// Occurs when the <see cref="ConsoleModifiers.Control"/> modifier key (ctrl) and
    /// either the <see cref="ConsoleKey.C"/> console key (C) or
    /// the Break key are pressed simultaneously
    /// (Ctrl+C or Ctrl+Break).
    /// </summary>
    event ConsoleCancelEventHandler? CancelKeyPress;
    
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
    

    /// <summary>
    /// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <returns></returns>
    char ReadChar();

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <returns></returns>
    ConsoleKeyInfo ReadKey();

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <returns></returns>
    ConsoleKeyInfo ReadKey(bool intercept);

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <returns></returns>
    string? ReadLine();

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream into a <see cref="SecureString"/>, displaying an asterisk (*) instead of the pressed characters.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    SecureString ReadPassword();
}