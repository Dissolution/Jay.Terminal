using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace Jay.Terminalis;

/// <summary>
/// An instance of a <see cref="Terminal"/> for fluent method chaining.
/// </summary>
public sealed class TerminalInstance
{
    #region Fields
    /// <summary>
    /// For thread-safe <see cref="Terminal"/> usage.
    /// </summary>
    internal readonly object _lock = new object();

    private IPalette _palette;
    private readonly ColorMapper _colorMapper = new ColorMapper();
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the foreground <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    public Color ForegroundColor
    {
        get => _palette[Console.ForegroundColor];
        set => Console.ForegroundColor = _palette[value];
    }

    /// <summary>
    /// Gets or sets the background <see cref="System.Drawing.Color"/> of the <see cref="Terminal"/>.
    /// </summary>
    public Color BackgroundColor
    {
        get => _palette[Console.BackgroundColor];
        set => Console.BackgroundColor = _palette[value];
    }

    /// <summary>
    /// Gets or sets the <see cref="IPalette"/> in use for this <see cref="Terminal"/>.
    /// </summary>
    public IPalette Palette
    {
        get => _palette;
        set
        {
            _palette = value;
            _colorMapper.SetBufferColors(_palette);
        }
    }

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s buffer area.
    /// </summary>
    public TerminalBuffer Buffer { get; }

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s cursor.
    /// </summary>
    public TerminalCursor Cursor { get; }

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s input.
    /// </summary>
    public TerminalInput Input { get; }
    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s output.
    /// </summary>
    public TerminalOutput Output { get; }
    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s error.
    /// </summary>
    public TerminalError Error { get; }

    /// <summary>
    /// Options related to the <see cref="Terminal"/>'s inner window.
    /// </summary>
    public TerminalDisplayWindow DisplayWindow { get; }

    /// <summary>
    /// Options related to the actual <see cref="Terminal"/> window.
    /// </summary>
    public TerminalWindow Window { get; }

    /// <summary>
    /// Gets a value indicating whether a key press is available in the input stream.
    /// </summary>
    public bool KeyAvailable => Console.KeyAvailable;

    /// <summary>
    /// Gets a value indicating whether CAPS LOCK is toggled on or off.
    /// </summary>
    public bool CapsLock => Console.CapsLock;

    /// <summary>
    /// Gets a value indicating whether NUMBER LOCK is toggled on or off.
    /// </summary>
    public bool NumberLock => Console.NumberLock;
    #endregion

    #region Events
    /// <summary>
    /// Occurs when the <see cref="ConsoleModifiers.Control"/> modifier key (ctrl) and
    /// either the <see cref="ConsoleKey.C"/> console key (C) or
    /// the Break key are pressed simultaneously (Ctrl+C or Ctrl+Break).
    /// </summary>
    public event ConsoleCancelEventHandler CancelKeyPress;
    #endregion

    #region Constructors
    internal TerminalInstance()
    {
        _palette = Palettes.Default;
        SetColors(_palette.DefaultForeColor, _palette.DefaultBackColor);

        Buffer = new TerminalBuffer(this);
        Cursor = new TerminalCursor(this);
        Input = new TerminalInput(this);
        Output = new TerminalOutput(this);
        Error = new TerminalError(this);
        DisplayWindow = new TerminalDisplayWindow(this);
        Window = new TerminalWindow(this);

        Console.CancelKeyPress += OnCancelKeyPress;

        Output.Encoding = Encoding.UTF8;
    }
    #endregion

    #region Private Methods
    private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        CancelKeyPress?.Invoke(sender, e);
    }

    private Color? Resolve(object arg)
    {
        switch (arg)
        {
            case null:
                return null;
            case Color color:
                return color;
            case ConsoleColor consoleColor:
                return _palette[consoleColor];
        }
        return null;
    }

    private void Write(FormattableString text, out int length)
    {
        if (text is null)
        {
            length = 0;
            return;
        }

        lock (_lock)
        {
            var format = text.Format;
            var formatLength = format.Length;
            var args = text.GetArguments();
            var argCount = text.ArgumentCount;

            var startFore = Console.ForegroundColor;
            var startBack = Console.BackgroundColor;

            var segment = new StringBuilder(formatLength);
            length = 0;
            //Parse
            for (var i = 0; i < formatLength; i++)
            {
                char c = format[i];
                //Check for start of placeholder
                if (c == '{')
                {
                    //New segment
                    segment.Clear();
                    //Find the closing bracket
                    for (var j = i + 1; j < formatLength; j++)
                    {
                        char d = format[j];
                        if (d == '}')
                        {
                            //Should be a number specifying an index
                            if (!Converter.TryConvertTo<int>(segment.ToString(), out int argIndex) ||
                                argIndex < 0 || argIndex >= argCount)
                                throw new FormatException("Format string is invalid (arguments)");

                            var arg = args[argIndex];
                            if (arg is Color color)
                            {
                                SetForeColor(color, startFore);
                            }
                            else if (arg is ConsoleColor consoleColor)
                            {
                                SetForeColor(consoleColor);
                            }
                            else if (arg is ITuple tuple)
                            {
                                if (tuple.Length >= 1)
                                {
                                    var fore = Resolve(tuple[0]);
                                    SetForeColor(fore, startFore);
                                }

                                if (tuple.Length >= 2)
                                {
                                    var back = Resolve(tuple[1]);
                                    SetBackColor(back, startBack);
                                }
                            }
                            else
                            {
                                //Just write it, it's a format object
                                var argString = arg?.ToString() ?? string.Empty;
                                Console.Write(argString);
                                length += argString.Length;
                            }

                            //Move ahead
                            i = j;
                            break;
                        }
                        else
                        {
                            //Add to segment
                            segment.Append(d);
                        }
                    }
                }
                else
                {
                    //Write this char
                    Console.Write(c);
                    length++;
                }
            }
        }//End lock
    }

    private void SetForeColor(Color? color, ConsoleColor defaultColor)
    {
        if (color is null)
            return;
        if (color.Value == Color.Empty)
        {
            Console.ForegroundColor = defaultColor;
        }
        else if (color.Value != Color.Transparent)
        {
            Console.ForegroundColor = _palette[color.Value];
        }
    }
    private void SetBackColor(Color? color, ConsoleColor defaultColor)
    {
        if (color is null)
            return;
        if (color.Value == Color.Empty)
        {
            Console.BackgroundColor = defaultColor;
        }
        else if (color.Value != Color.Transparent)
        {
            Console.BackgroundColor = _palette[color.Value];
        }
    }
    #endregion

    #region Public Methods
    #region Beep
    /// <summary>
    /// Plays a beep through the console speaker.
    /// </summary>
    /// <returns></returns>
    public TerminalInstance Beep()
    {
        Console.Beep();
        return this;
    }

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public TerminalInstance Beep(int frequency, int duration)
    {
        Console.Beep(frequency, duration);
        return this;
    }

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public TerminalInstance Beep(int frequency, TimeSpan duration)
    {
        Console.Beep(frequency, (int)duration.TotalMilliseconds);
        return this;
    }
    #endregion

    #region Clear
    /// <summary>
    /// Clears the <see cref="Terminal"/> buffer and corresponding <see cref="DisplayWindow"/> of display information.
    /// </summary>
    /// <returns></returns>
    public TerminalInstance Clear()
    {
        Console.Clear();
        return this;
    }

    /// <summary>
    /// Clears the current line the <see cref="Terminal"/> <see cref="Cursor"/> is on.
    /// </summary>
    /// <returns></returns>
    public TerminalInstance ClearLine()
    {
        lock (_lock)
        {
            var y = Console.CursorTop;
            var x = Console.CursorLeft;
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(x, y);
            return this;
        }
    }
    #endregion

    #region Read
    /// <summary>
    /// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <returns></returns>
    public char Read()
    {
        return (char)Console.Read();
    }

    /// <summary>
    /// Reads the next <see cref="char"/>acter from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public TerminalInstance Read(out char c)
    {
        c = (char)Console.Read();
        return this;
    }

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <returns></returns>
    public ConsoleKeyInfo ReadKey() => Console.ReadKey();

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="consoleKeyInfo"></param>
    /// <returns></returns>
    public TerminalInstance ReadKey(out ConsoleKeyInfo consoleKeyInfo)
    {
        consoleKeyInfo = Console.ReadKey();
        return this;
    }

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <returns></returns>
    public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

    /// <summary>
    /// Reads the next <see cref="char"/>acter or function key pressed by the user.
    /// The pressed key is optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="consoleKeyInfo"></param>
    /// <returns></returns>
    public TerminalInstance ReadKey(bool intercept, out ConsoleKeyInfo consoleKeyInfo)
    {
        consoleKeyInfo = Console.ReadKey(intercept);
        return this;
    }

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <returns></returns>
    public string ReadLine() => Console.ReadLine();

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public TerminalInstance ReadLine(out string line)
    {
        line = Console.ReadLine();
        return this;
    }

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream.
    /// The pressed keys are optionally displayed in the <see cref="Terminal"/> window.
    /// </summary>
    /// <param name="intercept"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public TerminalInstance ReadLine(bool intercept, out string line)
    {
        if (!intercept)
            return ReadLine(out line);
        lock (_lock)
        {
            var buffer = new StringBuilder();
            while (true)
            {
                var cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                    break;
                buffer.Append(cki.KeyChar);
            }

            line = buffer.ToString();
        }
        return this;
    }

    /// <summary>
    /// Reads the next line of characters from the standard <see cref="Input"/> stream into a <see cref="SecureString"/>, displaying an asterisk (*) instead of the pressed characters.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public TerminalInstance ReadPassword(out SecureString password)
    {
        password = new SecureString();
        lock(_lock)
            while (true)
            {
                var cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Enter)
                    break;

                if (cki.Key == ConsoleKey.Backspace)
                {
                    password.RemoveAt(password.Length - 1);
                    Console.CursorLeft -= 1;
                    Console.Write(' ');
                    Console.CursorLeft -= 1;
                }
                else
                {
                    password.AppendChar(cki.KeyChar);
                    Console.Write('*');
                }
            }
        //Done
        return this;
    }
    #endregion

    #region Color
    /// <summary>
    /// Resets the <see cref="ForegroundColor"/> and <see cref="BackgroundColor"/> to their default <see cref="System.Drawing.Color"/> values.
    /// </summary>
    /// <returns></returns>
    public TerminalInstance ResetColor()
    {
        lock (_lock)
        {
            Console.ForegroundColor = _palette[_palette.DefaultForeColor];
            Console.BackgroundColor = _palette[_palette.DefaultBackColor];
        }
        return this;
    }

    /// <summary>
    /// Sets the foreground <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <returns></returns>
    public TerminalInstance SetForeColor(Color foreColor)
    {
        if (foreColor == Color.Empty)
            Console.ForegroundColor = _palette[_palette.DefaultForeColor];
        else if (foreColor == Color.Transparent)
            return this;
        else
            Console.ForegroundColor = _palette[foreColor];
        return this;
    }

    /// <summary>
    /// Sets the background <see cref="System.Drawing.Color"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public TerminalInstance SetBackColor(Color backColor)
    {
        if (backColor == Color.Empty)
            Console.BackgroundColor = _palette[_palette.DefaultBackColor];
        else if (backColor == Color.Transparent)
            return this;
        else
            Console.BackgroundColor = _palette[backColor];
        return this;
    }

    /// <summary>
    /// Sets the foreground <see cref="ConsoleColor"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <returns></returns>
    public TerminalInstance SetForeColor(ConsoleColor foreColor)
    {
        Console.ForegroundColor = _palette[_palette[foreColor]];
        return this;
    }

    /// <summary>
    /// Sets the background <see cref="ConsoleColor"/> for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public TerminalInstance SetBackColor(ConsoleColor backColor)
    {
        Console.BackgroundColor = _palette[_palette[backColor]];
        return this;
    }

    /// <summary>
    /// Sets the foreground and background <see cref="System.Drawing.Color"/>s for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public TerminalInstance SetColors(Color? foreColor = null, Color? backColor = null)
    {
        lock (_lock)
        {
            if (foreColor != null)
                SetForeColor(foreColor.Value);
            if (backColor != null)
                SetBackColor(backColor.Value);
        }
        return this;
    }

    /// <summary>
    /// Sets the foreground and background colors for the <see cref="Terminal"/>.
    /// </summary>
    /// <param name="foreColor"></param>
    /// <param name="backColor"></param>
    /// <returns></returns>
    public TerminalInstance SetColors(ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
    {
        lock (_lock)
        {
            if (foreColor != null)
                SetForeColor(foreColor.Value);
            if (backColor != null)
                SetBackColor(backColor.Value);
        }
        return this;
    }
    #endregion

    #region Write
    /// <summary>
    /// Writes the text representation of the specified <see cref="bool"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(bool value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the specified <see cref="Encoding.Unicode"/> <see cref="char"/> to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(char value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the specified array of Unicode characters to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public TerminalInstance Write(char[] buffer)
    {
        Console.Write(buffer);
        return this;
    }

    /// <summary>
    /// Writes the a segment of an array of Unicode characters to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public TerminalInstance Write(char[] buffer, int? startIndex = null, int? length = null, int? endIndex = null)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));
        var check = SliceExtensions.Validate(buffer.Length, ref startIndex, ref length, ref endIndex);
        if (!check)
            throw check.Exception;
        Console.Write(buffer, startIndex.Value, length.Value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="object"/> to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(object value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="int"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(int value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="uint"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(uint value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="long"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(long value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="ulong"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(ulong value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="float"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(float value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="double"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(double value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="decimal"/> value to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(decimal value)
    {
        Console.Write(value);
        return this;
    }

    /// <summary>
    /// Writes the specified <see cref="string"/> value to the standard output stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance Write(NonFormattableString value)
    {
        Console.Write(value?.Value);
        return this;
    }

    /// <summary>
    /// Writes the specified <see cref="FormattableString"/> value to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public TerminalInstance Write(FormattableString text)
    {
        Write(text, out _);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    [StringFormatMethod("format")]
    public TerminalInstance Write(string format, object arg0)
    {
        Console.Write(format, arg0);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    [StringFormatMethod("format")]
    public TerminalInstance Write(string format, object arg0, object arg1)
    {
        Console.Write(format, arg0, arg1);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    [StringFormatMethod("format")]
    public TerminalInstance Write(string format, object arg0, object arg1, object arg2)
    {
        Console.Write(format, arg0, arg1, arg2);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    [StringFormatMethod("format")]
    public TerminalInstance Write(string format, object arg0, object arg1, object arg2, object arg3)
    {
        Console.Write(format, arg0, arg1, arg2, arg3);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/> to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [StringFormatMethod("format")]
    public TerminalInstance Write(NonFormattableString format, params object[] args)
    {
        Console.Write((string)format, args);
        return this;
    }
    #endregion

    #region WriteLine
    /// <summary>
    /// Writes the current line terminator (<see cref="Environment.NewLine"/>) to the standard output stream.
    /// </summary>
    /// <returns></returns>
    public TerminalInstance WriteLine()
    {
        Console.WriteLine();
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="object"/>, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(object value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="bool"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(bool value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the specified <see cref="Encoding.Unicode"/> <see cref="char"/>, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(char value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the specified array of Unicode characters, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(char[] buffer)
    {
        Console.WriteLine(buffer);
        return this;
    }

    /// <summary>
    /// Writes the a segement of an array of Unicode characters, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="startIndex"></param>
    /// <param name="length"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(char[] buffer, int? startIndex = null, int? length = null, int? endIndex = null)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));
        var check = SliceExtensions.Validate(buffer.Length, ref startIndex, ref length, ref endIndex);
        if (!check)
            throw check.Exception;
        Console.WriteLine(buffer, startIndex.Value, length.Value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="int"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(int value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="uint"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(uint value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="long"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(long value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="ulong"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(ulong value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="float"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(float value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="double"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(double value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the text representation of the specified <see cref="decimal"/> value, followed by the current line terminator, to the standard <see cref="Output"/> stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(decimal value)
    {
        Console.WriteLine(value);
        return this;
    }

    /// <summary>
    /// Writes the specified <see cref="string"/> value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(NonFormattableString value)
    {
        Console.WriteLine(value?.Value);
        return this;
    }

    /// <summary>
    /// Writes the specified <see cref="FormattableString"/> value, followed by the current line terminator, to the standard output stream, using embedded <see cref="System.Drawing.Color"/> values to change the foreground color during the writing process.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(FormattableString text)
    {
        Write(text, out _);
        Console.WriteLine();
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(string format, object arg0)
    {
        Console.WriteLine(format, arg0);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(string format, object arg0, object arg1)
    {
        Console.WriteLine(format, arg0, arg1);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(string format, object arg0, object arg1, object arg2)
    {
        Console.WriteLine(format, arg0, arg1, arg2);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
    {
        Console.WriteLine(format, arg0, arg1, arg2, arg3);
        return this;
    }

    /// <summary>
    /// Writes a formatted <see cref="string"/>, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public TerminalInstance WriteLine(NonFormattableString format, params object[] args)
    {
        Console.WriteLine((string)format, args);
        return this;
    }
    #endregion

    #region Temporary Actions
    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Color"/>s and <see cref="Cursor"/> positions, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Color"/>s and <see cref="Cursor"/> position.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    public TerminalInstance Temp(Action<TerminalInstance> tempAction)
    {
        if (tempAction is null)
            return this;
        lock (_lock)
        {
            //Store temp vars
            var oldForeColor = Console.ForegroundColor;
            var oldBackColor = Console.BackgroundColor;
            var oldCursorLeft = Console.CursorLeft;
            var oldCursorTop = Console.CursorTop;
            //Do the temp action
            tempAction(this);
            //Reset temp vars
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
            Console.CursorLeft = oldCursorLeft;
            Console.CursorTop = oldCursorTop;
        }
        //Fin
        return this;
    }

    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Color"/>s, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Color"/>s.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    public TerminalInstance TempColor(Action<TerminalInstance> tempAction)
    {
        if (tempAction is null)
            return this;
        lock (_lock)
        {
            //Store temp vars
            var oldForeColor = Console.ForegroundColor;
            var oldBackColor = Console.BackgroundColor;
            //Do the temp action
            tempAction(this);
            //Reset temp vars
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
        }
        //Fin
        return this;
    }

    /// <summary>
    /// Stores the current <see cref="Terminal"/> <see cref="Cursor"/> position, performs an <see cref="Action"/> on this <see cref="Terminal"/>, then restores the <see cref="Cursor"/> position.
    /// </summary>
    /// <param name="tempAction"></param>
    /// <returns></returns>
    public TerminalInstance TempPosition(Action<TerminalInstance> tempAction)
    {
        if (tempAction is null)
            return this;
        lock (_lock)
        {
            //Store temp vars
            var oldCursorLeft = Console.CursorLeft;
            var oldCursorTop = Console.CursorTop;
            //Do the temp action
            tempAction(this);
            //Reset temp vars
            Console.CursorLeft = oldCursorLeft;
            Console.CursorTop = oldCursorTop;
        }
        //Fin
        return this;
    }

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s colors back to what they were when the <see cref="ColorLock"/> was taken.
    /// </summary>
    public IDisposable ColorLock
    {
        get
        {
            //Store temp vars
            lock (_lock)
            {
                var oldForeColor = Console.ForegroundColor;
                var oldBackColor = Console.BackgroundColor;
                return new Disposer(() =>
                {
                    //Reset temp vars
                    lock (_lock)
                    {
                        Console.ForegroundColor = oldForeColor;
                        Console.BackgroundColor = oldBackColor;
                    }
                });
            }
        }
    }

    /// <summary>
    /// Gets an <see cref="IDisposable"/> that, when disposed, resets the <see cref="Terminal"/>'s cursor back to where it was when the <see cref="ColorLock"/> was taken.
    /// </summary>
    public IDisposable CursorLock
    {
        get
        {
            lock (_lock)
            {
                //Store temp vars
                var oldCursorLeft = Console.CursorLeft;
                var oldCursorTop = Console.CursorTop;
                return new Disposer(() =>
                {
                    //Reset temp vars
                    lock (_lock)
                    {
                        Console.CursorLeft = oldCursorLeft;
                        Console.CursorTop = oldCursorTop;
                    }
                });
            }
        }
    }
    #endregion
    #endregion
}