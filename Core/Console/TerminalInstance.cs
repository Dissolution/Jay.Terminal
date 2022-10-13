using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Jay.Terminalis.Colors;
using Jay.Terminalis.Native;

namespace Jay.Terminalis.Console;

internal class TerminalInstance : ITerminalInstance,
                                  ITerminalInput,
                                  ITerminalOutput,
                                  ITerminalError,
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

    public ITerminalError Error  => this;

    public ITerminalBuffer Buffer => this;

    public ITerminalCursor Cursor => this;

    public ITerminalDisplay Display => this;

    public ITerminalWindow Window => this;

    public TerminalInstance()
    {
        _consoleWindowHandle = NativeMethods.GetConsoleHandle();
        _slimLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        _cancelEventHandler = null;
        SysCons.CancelKeyPress += ConsoleCancelKeyPress;
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
        get => GetValue(() => SysCons.In);
        set => SetValue(SysCons.SetIn, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to read input.
    /// </summary>
    Encoding ITerminalInput.Encoding
    {
        get => GetValue(() => SysCons.InputEncoding);
        set => SetValue(v => SysCons.InputEncoding = v, value);
    }

    /// <summary>
    /// Has the input stream been redirected from standard?
    /// </summary>
    bool ITerminalInput.IsRedirected => GetValue(() => SysCons.IsInputRedirected);

    /// <summary>
    /// Gets or sets whether Ctrl+C should be treated as input or as a break command.
    /// </summary>
    bool ITerminalInput.TreatCtrlCAsInput
    {
        get => GetValue(() => SysCons.TreatControlCAsInput);
        set => SetValue(v => SysCons.TreatControlCAsInput = v, value);
    }
    
    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalInput.OpenStream() => GetValue(() => SysCons.OpenStandardInput());

    /// <summary>
    /// Acquires the standard input <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalInput.OpenStream(int bufferSize) => GetValue(() => SysCons.OpenStandardInput(bufferSize));

    bool ITerminalInput.KeyAvailable => GetValue(() => SysCons.KeyAvailable);

    bool ITerminalInput.CapsLock => GetValue(() => SysCons.CapsLock);

    bool ITerminalInput.NumberLock => GetValue(() => SysCons.NumberLock);

    event ConsoleCancelEventHandler? ITerminalInput.CancelKeyPress
    {
        add => _cancelEventHandler = Delegate.Combine(_cancelEventHandler, value) as ConsoleCancelEventHandler;
        remove => _cancelEventHandler = Delegate.Remove(_cancelEventHandler, value) as ConsoleCancelEventHandler;
    }
    char ITerminalInput.ReadChar()
    {
        int unicodeChar = GetValue(SysCons.Read);
        return Convert.ToChar(unicodeChar);
    }
    ConsoleKeyInfo ITerminalInput.ReadKey() => GetValue(SysCons.ReadKey);
    ConsoleKeyInfo ITerminalInput.ReadKey(bool intercept) => GetValue(() => SysCons.ReadKey(intercept));
    string? ITerminalInput.ReadLine() => GetValue(SysCons.ReadLine);
  
    SecureString ITerminalInput.ReadPassword()
    {
        return GetValue(() =>
        {
            var secureString = new SecureString();
            while (SysCons.KeyAvailable)
            {
                var key = SysCons.ReadKey();
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
        get => GetValue(() => SysCons.Out);
        set => SetValue(SysCons.SetOut, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="System.Text.Encoding"/> the <see cref="Terminal"/> uses to output.
    /// </summary>
    Encoding ITerminalOutput.Encoding
    {
        get => GetValue(() => SysCons.OutputEncoding);
        set => SetValue(v => SysCons.OutputEncoding = v, value);
    }

    /// <summary>
    /// Has the output stream been redirected from standard?
    /// </summary>
    bool ITerminalOutput.IsRedirected => GetValue(() => SysCons.IsOutputRedirected);
    
    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalOutput.OpenStream() => GetValue(SysCons.OpenStandardOutput);

    /// <summary>
    /// Acquires the standard output <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalOutput.OpenStream(int bufferSize) => GetValue(() => SysCons.OpenStandardOutput(bufferSize));

    TerminalColor ITerminalOutput.DefaultForeColor => TerminalColors.Default.Foreground;

    TerminalColor ITerminalOutput.DefaultBackColor => TerminalColors.Default.Background;

    TerminalColor ITerminalOutput.ForegroundColor
    {
        get => GetValue(() => (TerminalColor)SysCons.ForegroundColor);
        set => SetValue(color => SysCons.ForegroundColor = (ConsoleColor)color, value);
    }

    TerminalColor ITerminalOutput.BackgroundColor
    {
        get => GetValue(() => (TerminalColor)SysCons.BackgroundColor);
        set => SetValue(color => SysCons.BackgroundColor = (ConsoleColor)color, value);
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
            return new TerminalReset(this)
                .Watch(t => t.Output.ForegroundColor)
                .Watch(t => t.Output.BackgroundColor);
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
                    SysCons.Out.Write(buffer[..charsWritten]);
                }
                else
                {
                    SysCons.Out.Write(((IFormattable)value).ToString(default, default));
                }
            }
            else if (value is IFormattable)
            {
                SysCons.Out.Write(((IFormattable)value).ToString(default, default));
            }
            else
            {
                SysCons.Out.Write(value?.ToString());
            }
        }
    }
    void ITerminalOutput.Write(ReadOnlySpan<char> text)
    {
        using (_slimLock.GetWriteLock())
        {
            SysCons.Out.Write(text);
        }
    }
    void ITerminalOutput.Write(ref DefaultInterpolatedStringHandler interpolatedText)
    {
        // Eventually the interpolated handler will do some cool stuff with colors
        string text = interpolatedText.ToStringAndClear();
        using (_slimLock.GetWriteLock())
        {
            SysCons.Out.Write(text);
        }
    }
    void ITerminalOutput.WriteLine()
    {
        using (_slimLock.GetWriteLock())
        {
            SysCons.Out.WriteLine();
        }
    }
    void ITerminalOutput.WriteLine<T>([AllowNull] T value)
    {
        using (_slimLock.GetWriteLock())
        {
            SysCons.Out.WriteLine(value?.ToString());
        }
    }
    void ITerminalOutput.WriteLine(ReadOnlySpan<char> text)
    {
        using (_slimLock.GetWriteLock())
        {
            SysCons.Out.WriteLine(text);
        }
    }
    void ITerminalOutput.WriteLine(ref DefaultInterpolatedStringHandler interpolatedText)
    {
        // ToDo special someday!
        using (_slimLock.GetWriteLock())
        {
            SysCons.Out.WriteLine(interpolatedText.ToStringAndClear());
        }
    }
    #endregion
    
    #region Error
    /// <summary>
    /// Gets or sets the <see cref="TextWriter"/> the Error outputs to.
    /// </summary>
    TextWriter ITerminalError.Writer
    {
        get => GetValue(() => SysCons.Error);
        set => SetValue(SysCons.SetError, value);
    }

    /// <summary>
    /// Has the error stream been redirected from standard?
    /// </summary>
    bool ITerminalError.IsRedirected => GetValue(() => SysCons.IsErrorRedirected);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>.
    /// </summary>
    /// <returns></returns>
    Stream ITerminalError.OpenStream() => GetValue(SysCons.OpenStandardError);

    /// <summary>
    /// Acquires the standard error <see cref="Stream"/>, which is set to a specified buffer size.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    Stream ITerminalError.OpenStream(int bufferSize) => GetValue(() => SysCons.OpenStandardError(bufferSize));
    #endregion
    
    #region Buffer
    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    int ITerminalBuffer.Width
    {
        get => GetValue(() => SysCons.BufferWidth);
        set => SetValue(v => SysCons.BufferWidth = v, value);
    }

    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    int ITerminalBuffer.Height
    {
        get => GetValue(() => SysCons.BufferHeight);
        set => SetValue(v => SysCons.BufferHeight = v, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Size"/> of the buffer area.
    /// </summary>
    Size ITerminalBuffer.Size
    {
        get => GetValue(() => new Size(SysCons.BufferWidth, SysCons.BufferHeight));
        set => SetValue(v => SysCons.SetBufferSize(v.Width, v.Height), value);
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
            SysCons.MoveBufferArea(area.Left, area.Top, area.Width, area.Height, position.X, position.Y);
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
            SysCons.MoveBufferArea(left, top, width, height, xPos, yPos);
        });
    }

    void ITerminalBuffer.Clear() => Interact(SysCons.Clear);
    #endregion
    
    #region Cursor
    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.
    /// </summary>
    int ITerminalCursor.Left
    {
        get => GetValue(() => SysCons.CursorLeft);
        set => SetValue(left => SysCons.CursorLeft = left, value);
    }

    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area.
    /// </summary>
    int ITerminalCursor.Top
    {
        get => GetValue(() => SysCons.CursorTop);
        set => SetValue(top => SysCons.CursorTop = top, value);
    }

    /// <summary>
    /// Gets or sets the cursor position within the buffer area.
    /// </summary>
    Point ITerminalCursor.Position
    {
        get => GetValue(() => new Point(SysCons.CursorLeft, SysCons.CursorTop));
        set => SetValue(point => SysCons.SetCursorPosition(point.X, point.Y), value);
    }

    /// <summary>
    /// Gets or sets the height of the cursor within a cell.
    /// </summary>
    int ITerminalCursor.Height
    {
        get => GetValue(() => SysCons.CursorSize);
        set => SetValue(height => SysCons.CursorSize = height, value);
    }

    /// <summary>
    /// Gets or sets whether the cursor is visible.
    /// </summary>
    bool ITerminalCursor.Visible
    {
        get => GetValue(() => SysCons.CursorVisible);
        set => SetValue(visible => SysCons.CursorVisible = visible, value);
    }

    public IDisposable CursorLock => new TerminalReset(this)
        .Watch(t => t.Cursor.Left)
        .Watch(t => t.Cursor.Top)
        .Watch(t => t.Cursor.Visible);

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
        get => GetValue(() => SysCons.WindowLeft);
        set => SetValue(left => SysCons.WindowLeft = left, value);
    }

    /// <summary>
    /// Gets or sets the top position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    int ITerminalDisplay.Top
    {
        get => GetValue(() => SysCons.WindowTop);
        set => SetValue(top => SysCons.WindowTop = top, value);
    }

    /// <summary>
    /// Gets or sets the position of the <see cref="Terminal"/> display window area relative to the screen buffer.
    /// </summary>
    Point ITerminalDisplay.Position
    {
        get => GetValue(() => new Point(SysCons.WindowLeft, SysCons.WindowTop));
        set => SetValue(point => SysCons.SetWindowPosition(point.X, point.Y), value);
    }

    /// <summary>
    /// Gets or sets the width of the <see cref="Terminal"/> display window.
    /// </summary>
    int ITerminalDisplay.Width
    {
        get => GetValue(() => SysCons.WindowWidth);
        set => SetValue(width => SysCons.WindowWidth = width, value);
    }

    /// <summary>
    /// Gets or sets the height of the <see cref="Terminal"/> display window.
    /// </summary>
    int ITerminalDisplay.Height
    {
        get => GetValue(() => SysCons.WindowHeight);
        set => SetValue(height => SysCons.WindowHeight = height, value);
    }

    /// <summary>
    /// Gets or sets the width and height of the <see cref="Terminal"/> display window.
    /// </summary>
    Size ITerminalDisplay.Size
    {
        get => GetValue(() => new Size(SysCons.WindowWidth, SysCons.WindowHeight));
        set => SetValue(size => SysCons.SetWindowSize(size.Width, size.Height), value);
    }

    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window rows, based on the current font, screen resolution, and window size.
    /// </summary>
    int ITerminalDisplay.LargestHeight => GetValue(() => SysCons.LargestWindowHeight);

    /// <summary>
    /// Gets the largest possible number of <see cref="Terminal"/> display window columns, based on the current font, screen resolution, and window size.
    /// </summary>
    int ITerminalDisplay.LargestWidth => GetValue(() => SysCons.LargestWindowWidth);

    void ITerminalDisplay.Clear() => Interact(SysCons.Clear);
#endregion
    
    #region Window
    /// <summary>
    /// Gets or sets the title to display in the <see cref="Terminal"/> window.
    /// </summary>
    string ITerminalWindow.Title
    {
        get => GetValue(() => SysCons.Title);
        set => SetValue(title => SysCons.Title = title, value);
    }

    Point ITerminalWindow.Location
    {
        get => GetValue(() => new Point(SysCons.WindowLeft, SysCons.WindowTop));
        set
        {
            SetValue(location =>
            {
                var newBounds = new Rectangle(location.X, location.Y, SysCons.WindowWidth, SysCons.WindowHeight);
                NativeMethods.MoveAndResizeWindow(_consoleWindowHandle, newBounds);
            }, value);
        }
    }

    Size ITerminalWindow.Size
    {
        get => GetValue(() => new Size(SysCons.WindowWidth, SysCons.WindowHeight));
        set
        {
            SetValue(size =>
            {
                var newBounds = new Rectangle(SysCons.WindowLeft, SysCons.WindowTop, size.Width, size.Height);
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
            SysCons.CancelKeyPress -= ConsoleCancelKeyPress;
            _cancelEventHandler = null;
        }
        _slimLock.Dispose();
    }
}