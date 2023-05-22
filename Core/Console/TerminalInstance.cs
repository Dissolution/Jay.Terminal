using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Jay.Terminalis.Colors;
using Jay.Terminalis.Native;
using Jay.Terminalis.Threading;

namespace Jay.Terminalis.Console;

internal class TerminalInstance : ITerminalInstance,
                                  ITerminalInput,
                                  ITerminalOutput,
                                  ITerminalErrorOutput,
                                  ITerminalBuffer,
                                  ITerminalCursor,
                                  ITerminalDisplay,
                                  ITerminalWindow
{
    private readonly IntPtr _consoleWindowHandle;
    private readonly ReaderWriterLockSlim _slimLock;
    private ConsoleCancelEventHandler? _cancelEventHandler;
    
    public ITerminalInput Input => this;

    public ITerminalOutput Output => this;

    public ITerminalErrorOutput Error  => this;

    public ITerminalBuffer Buffer => this;

    public ITerminalCursor Cursor => this;

    public ITerminalDisplay Display => this;

    public ITerminalWindow Window => this;

    public TerminalInstance()
    {
        _consoleWindowHandle = NativeMethods.GetConsoleHandle();
        _slimLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        _cancelEventHandler = null;
        SystemConsole.CancelKeyPress += ConsoleCancelKeyPress;
    }
    private void ConsoleCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
    {
        _cancelEventHandler?.Invoke(sender, args);
    }

    private TValue GetValue<TValue>(Func<TValue> getConsoleValue)
    {
        _slimLock.TryEnterReadLock(-1);
        TValue value;
        try
        {
            value = getConsoleValue();
        }
        finally
        {
            _slimLock.ExitReadLock();
        }
        return value;
    }

    private void SetValue<TValue>(Action<TValue> setConsoleValue, TValue value)
    {
        _slimLock.TryEnterWriteLock(-1);
        try
        {
            setConsoleValue(value);
        }
        finally
        {
            _slimLock.ExitWriteLock();
        }
    }

    private void Interact(Action consoleAction)
    {
        _slimLock.TryEnterWriteLock(-1);
        try
        {
            consoleAction();
        }
        finally
        {
            _slimLock.ExitWriteLock();
        }
    }
    
    #region Input
    /// <summary>
    /// Gets or sets the <see cref="TextReader"/> the input reads from.
    /// </summary>
    TextReader ITerminalInput.Reader
    {
        get => GetValue(() => SystemConsole.In);
        set => SetValue(SystemConsole.SetIn, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to read input.
    /// </summary>
    Encoding ITerminalInput.Encoding
    {
        get => GetValue(() => SystemConsole.InputEncoding);
        set => SetValue(v => SystemConsole.InputEncoding = v, value);
    }

    /// <summary>
    /// Has the input stream been redirected from standard?
    /// </summary>
    bool ITerminalInput.IsRedirected => GetValue(() => SystemConsole.IsInputRedirected);

    /// <summary>
    /// Gets or sets whether Ctrl+C should be treated as input or as a break command.
    /// </summary>
    bool ITerminalInput.TreatCtrlCAsInput
    {
        get => GetValue(() => SystemConsole.TreatControlCAsInput);
        set => SetValue(v => SystemConsole.TreatControlCAsInput = v, value);
    }
    
    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalInput.OpenStream() => GetValue(() => SystemConsole.OpenStandardInput());

    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalInput.OpenStream(int bufferSize) => GetValue(() => SystemConsole.OpenStandardInput(bufferSize));

    bool ITerminalInput.KeyAvailable => GetValue(() => SystemConsole.KeyAvailable);

    bool ITerminalInput.CapsLock => GetValue(() => SystemConsole.CapsLock);

    bool ITerminalInput.NumberLock => GetValue(() => SystemConsole.NumberLock);

    event ConsoleCancelEventHandler? ITerminalInput.CancelKeyPress
    {
        add => _cancelEventHandler = Delegate.Combine(_cancelEventHandler, value) as ConsoleCancelEventHandler;
        remove => _cancelEventHandler = Delegate.Remove(_cancelEventHandler, value) as ConsoleCancelEventHandler;
    }
    char ITerminalInput.ReadChar()
    {
        int unicodeChar = GetValue(SystemConsole.Read);
        return Convert.ToChar(unicodeChar);
    }
    ConsoleKeyInfo ITerminalInput.ReadKey() => GetValue(SystemConsole.ReadKey);
    ConsoleKeyInfo ITerminalInput.ReadKey(bool intercept) => GetValue(() => SystemConsole.ReadKey(intercept));
    string? ITerminalInput.ReadLine() => GetValue(SystemConsole.ReadLine);
  
    SecureString ITerminalInput.ReadPassword()
    {
        return GetValue(() =>
        {
            var secureString = new SecureString();
            while (SystemConsole.KeyAvailable)
            {
                var key = SystemConsole.ReadKey();
                if (char.IsWhiteSpace(key.KeyChar)) break;
                secureString.AppendChar(key.KeyChar);
            }
            return secureString;
        });
    }
    #endregion
    
    #region Output
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> used for output.
    /// </summary>
    TextWriter ITerminalOutput.Writer
    {
        get => GetValue(() => SystemConsole.Out);
        set => SetValue(SystemConsole.SetOut, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to output.
    /// </summary>
    Encoding ITerminalOutput.Encoding
    {
        get => GetValue(() => SystemConsole.OutputEncoding);
        set => SetValue(v => SystemConsole.OutputEncoding = v, value);
    }

    /// <summary>
    /// Has the output stream been redirected from standard?
    /// </summary>
    bool ITerminalOutput.IsRedirected => GetValue(() => SystemConsole.IsOutputRedirected);
    
    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalOutput.OpenStream() => GetValue(SystemConsole.OpenStandardOutput);

    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalOutput.OpenStream(int bufferSize) => GetValue(() => SystemConsole.OpenStandardOutput(bufferSize));

    TerminalColor ITerminalOutput.DefaultForeColor => TerminalColors.Default.Foreground;

    TerminalColor ITerminalOutput.DefaultBackColor => TerminalColors.Default.Background;

    TerminalColor ITerminalOutput.ForegroundColor
    {
        get => GetValue(() => (TerminalColor)SystemConsole.ForegroundColor);
        set => SetValue(color => SystemConsole.ForegroundColor = (ConsoleColor)color, value);
    }

    TerminalColor ITerminalOutput.BackgroundColor
    {
        get => GetValue(() => (TerminalColor)SystemConsole.BackgroundColor);
        set => SetValue(color => SystemConsole.BackgroundColor = (ConsoleColor)color, value);
    }

    IPalette ITerminalOutput.Palette
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    IDisposable ITerminalOutput.ColorLock
    {
        get
        {
            var (fore, back) = (SystemConsole.ForegroundColor, SystemConsole.BackgroundColor);
            return new ActionDisposable(() => Interact(() =>
            {
                SystemConsole.ForegroundColor = fore;
                SystemConsole.BackgroundColor = back;
            }));
        }
    }

    void ITerminalOutput.TempColor(Action<ITerminalInstance> tempAction)
    {
        var foreColor = this.Output.ForegroundColor;
        var backColor = this.Output.BackgroundColor;
        tempAction(this);
        this.Output.ForegroundColor = foreColor;
        this.Output.BackgroundColor = backColor;
    }
    void ITerminalOutput.ResetColor()
    {
        this.Output.ForegroundColor = Output.DefaultForeColor;
        this.Output.BackgroundColor = Output.DefaultBackColor;
    }
    void ITerminalOutput.SetForeColor(TerminalColor foreColor)
    {
        this.Output.ForegroundColor = foreColor;
    }
    void ITerminalOutput.SetBackColor(TerminalColor backColor)
    {
        this.Output.BackgroundColor = backColor;
    }
    void ITerminalOutput.SetColors(TerminalColor? foreColor, TerminalColor? backColor)
    {
        if (foreColor.HasValue)
            this.Output.ForegroundColor = foreColor.Value;
        if (backColor.HasValue)
            this.Output.BackgroundColor = backColor.Value;
    }
    void ITerminalOutput.Write<T>([AllowNull] T value)
    {
        using (_slimLock.GetWriteLock())
        {
            if (value is ISpanFormattable)
            {
                Span<char> buffer = stackalloc char[1024];
                if (((ISpanFormattable)value).TryFormat(buffer, out int charsWritten, default, default))
                {
                    SystemConsole.Out.Write(buffer[..charsWritten]);
                }
                else
                {
                    SystemConsole.Out.Write(((IFormattable)value).ToString(default, default));
                }
            }
            else if (value is IFormattable)
            {
                SystemConsole.Out.Write(((IFormattable)value).ToString(default, default));
            }
            else
            {
                SystemConsole.Out.Write(value?.ToString());
            }
        }
    }
    void ITerminalOutput.Write(ReadOnlySpan<char> text)
    {
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.Out.Write(text);
        }
    }
    void ITerminalOutput.Write(ref DefaultInterpolatedStringHandler interpolatedText)
    {
        // Eventually the interpolated handler will do some cool stuff with colors
        string text = interpolatedText.ToStringAndClear();
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.Out.Write(text);
        }
    }
    void ITerminalOutput.WriteLine()
    {
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.Out.WriteLine();
        }
    }
    void ITerminalOutput.WriteLine<T>([AllowNull] T value)
    {
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.Out.WriteLine(value?.ToString());
        }
    }
    void ITerminalOutput.WriteLine(ReadOnlySpan<char> text)
    {
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.Out.WriteLine(text);
        }
    }
    void ITerminalOutput.WriteLine(ref DefaultInterpolatedStringHandler interpolatedText)
    {
        // ToDo special someday!
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.Out.WriteLine(interpolatedText.ToStringAndClear());
        }
    }
    #endregion
    
    #region Error
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> the Error outputs to.
    /// </summary>
    TextWriter ITerminalErrorOutput.Writer
    {
        get => GetValue(() => SystemConsole.Error);
        set => SetValue(SystemConsole.SetError, value);
    }

    /// <summary>
    /// Has the error stream been redirected from standard?
    /// </summary>
    bool ITerminalErrorOutput.IsRedirected => GetValue(() => SystemConsole.IsErrorRedirected);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalErrorOutput.OpenStream() => GetValue(SystemConsole.OpenStandardError);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalErrorOutput.OpenStream(int bufferSize) => GetValue(() => SystemConsole.OpenStandardError(bufferSize));
    #endregion
    
    #region Buffer
    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    int ITerminalBuffer.Width
    {
        get => GetValue(() => SystemConsole.BufferWidth);
        set => SetValue(v => SystemConsole.BufferWidth = v, value);
    }

    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    int ITerminalBuffer.Height
    {
        get => GetValue(() => SystemConsole.BufferHeight);
        set => SetValue(v => SystemConsole.BufferHeight = v, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    Size ITerminalBuffer.Size
    {
        get => GetValue(() => new Size(SystemConsole.BufferWidth, SystemConsole.BufferHeight));
        set => SetValue(v => SystemConsole.SetBufferSize(v.Width, v.Height), value);
    }

    /// <summary>
    /// Copies a specified source <see cref="Rectangle"/> of the screen buffer to a specified destination <see cref="Point"/>.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    void ITerminalBuffer.Copy(Rectangle area, Point position)
    {
        Interact(() =>
        {
            SystemConsole.MoveBufferArea(area.Left, area.Top, area.Width, area.Height, position.X, position.Y);
        });
    }

    /// <summary>
    /// Copies a specified source area of the screen buffer to a specified destination area.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <returns></returns>
    void ITerminalBuffer.Copy(int left, int top, int width, int height, int xPos, int yPos)
    {
        Interact(() =>
        {
            SystemConsole.MoveBufferArea(left, top, width, height, xPos, yPos);
        });
    }

    void ITerminalBuffer.Clear() => Interact(SystemConsole.Clear);
    #endregion
    
    #region Cursor
    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.
    /// </summary>
    int ITerminalCursor.Left
    {
        get => GetValue(() => SystemConsole.CursorLeft);
        set => SetValue(left => SystemConsole.CursorLeft = left, value);
    }

    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area.
    /// </summary>
    int ITerminalCursor.Top
    {
        get => GetValue(() => SystemConsole.CursorTop);
        set => SetValue(top => SystemConsole.CursorTop = top, value);
    }

    /// <summary>
    /// Gets or sets the cursor position within the buffer area.
    /// </summary>
    Point ITerminalCursor.Position
    {
        get => GetValue(() => new Point(SystemConsole.CursorLeft, SystemConsole.CursorTop));
        set => SetValue(point => SystemConsole.SetCursorPosition(point.X, point.Y), value);
    }

    /// <summary>
    /// Gets or sets the height of the cursor within a cell.
    /// </summary>
    int ITerminalCursor.Height
    {
        get => GetValue(() => SystemConsole.CursorSize);
        set => SetValue(height => SystemConsole.CursorSize = height, value);
    }

    /// <summary>
    /// Gets or sets whether the cursor is visible.
    /// </summary>
    bool ITerminalCursor.Visible
    {
        get => GetValue(() => SystemConsole.CursorVisible);
        set => SetValue(visible => SystemConsole.CursorVisible = visible, value);
    }

    public IDisposable CursorLock
    {
        get
        {
            var (left, top, visible) = (SystemConsole.CursorLeft, SystemConsole.CursorTop, SystemConsole.CursorVisible);
            return new ActionDisposable(() =>
            {
                _slimLock.TryEnterWriteLock(-1);
                try
                {
                    SystemConsole.CursorLeft = left;
                    SystemConsole.CursorTop = top;
                    SystemConsole.CursorVisible = visible;
                }
                finally
                {
                    _slimLock.ExitWriteLock();
                }
            });
        }
    }

    public void TempPosition(Action<ITerminalInstance> tempAction)
    {
        var oldPos = this.Cursor.Position;
        tempAction(this);
        this.Cursor.Position = oldPos;
    }
    #endregion
    
    #region Display
    /// <summary>
    /// Gets or sets the leftmost position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    int ITerminalDisplay.Left
    {
        get => GetValue(() => SystemConsole.WindowLeft);
        set => SetValue(left => SystemConsole.WindowLeft = left, value);
    }

    /// <summary>
    /// Gets or sets the top position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    int ITerminalDisplay.Top
    {
        get => GetValue(() => SystemConsole.WindowTop);
        set => SetValue(top => SystemConsole.WindowTop = top, value);
    }

    /// <summary>
    /// Gets or sets the position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    Point ITerminalDisplay.Position
    {
        get => GetValue(() => new Point(SystemConsole.WindowLeft, SystemConsole.WindowTop));
        set => SetValue(point => SystemConsole.SetWindowPosition(point.X, point.Y), value);
    }

    /// <summary>
    /// Gets or sets the width of the <see cref="Terminal"/> display window.
    /// </summary>
    int ITerminalDisplay.Width
    {
        get => GetValue(() => SystemConsole.WindowWidth);
        set => SetValue(width => SystemConsole.WindowWidth = width, value);
    }

    /// <summary>
    /// Gets or sets the height of the <see cref="Terminal"/> display window.
    /// </summary>
    int ITerminalDisplay.Height
    {
        get => GetValue(() => SystemConsole.WindowHeight);
        set => SetValue(height => SystemConsole.WindowHeight = height, value);
    }

    /// <summary>
    /// Gets or sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    Size ITerminalDisplay.Size
    {
        get => GetValue(() => new Size(SystemConsole.WindowWidth, SystemConsole.WindowHeight));
        set => SetValue(size => SystemConsole.SetWindowSize(size.Width, size.Height), value);
    }

    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window rows, based on the current font, screen resolution, and window size.
    /// </summary>
    int ITerminalDisplay.LargestHeight => GetValue(() => SystemConsole.LargestWindowHeight);

    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window columns, based on the current font, screen resolution, and window size.
    /// </summary>
    int ITerminalDisplay.LargestWidth => GetValue(() => SystemConsole.LargestWindowWidth);

    void ITerminalDisplay.Clear() => Interact(SystemConsole.Clear);
#endregion
    
    #region Window
    /// <summary>
    /// Gets or sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    string ITerminalWindow.Title
    {
        get => GetValue(() => SystemConsole.Title);
        set => SetValue(title => SystemConsole.Title = title, value);
    }

    Point ITerminalWindow.Location
    {
        get => GetValue(() => new Point(SystemConsole.WindowLeft, SystemConsole.WindowTop));
        set
        {
            SetValue(location =>
            {
                var newBounds = new Rectangle(location.X, location.Y, SystemConsole.WindowWidth, SystemConsole.WindowHeight);
                NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, newBounds);
            }, value);
        }
    }

    Size ITerminalWindow.Size
    {
        get => GetValue(() => new Size(SystemConsole.WindowWidth, SystemConsole.WindowHeight));
        set
        {
            SetValue(size =>
            {
                var newBounds = new Rectangle(SystemConsole.WindowLeft, SystemConsole.WindowTop, size.Width, size.Height);
                NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, newBounds);
            }, value);
        }
    }
    
    /// <summary>
    /// Gets or sets the bounds of the <see cref="Terminal"/> window.
    /// </summary>
    Rectangle ITerminalWindow.Bounds
    {
        get
        {
            return GetValue(() =>
            {
                if (NativeMethods.GetWindowRect(_consoleWindowHandle, out var bounds))
                    return bounds;
                return Rectangle.Empty;
            });
        }
        set
        {
            Interact(() =>
            {
                NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, value, true);
            });
        }
    }
    #endregion

    void ITerminalInstance.Temp(Action<ITerminalInstance> terminalAction)
    {
        // TODO: Better watch syntax?
        throw new NotImplementedException();
    }

    void IDisposable.Dispose()
    {
        using (_slimLock.GetWriteLock())
        {
            SystemConsole.CancelKeyPress -= ConsoleCancelKeyPress;
            _cancelEventHandler = null;
        }
        _slimLock.Dispose();
    }
}