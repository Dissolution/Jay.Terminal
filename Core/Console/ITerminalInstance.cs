global using SysCons = System.Console;


namespace Jay.Terminalis.Console;

public interface ITerminalInstance : IDisposable
{
    ITerminalInput Input { get; }
    ITerminalOutput Output { get; }
    ITerminalError Error { get; }
    
    ITerminalBuffer Buffer { get; }
    ITerminalCursor Cursor { get; }
    ITerminalDisplay Display { get; }
    ITerminalWindow Window { get; }
    
    /// <summary>
    /// Stores the current information in <see cref="Input"/>, <see cref="Output"/>, <see cref="Buffer"/>,
    /// <see cref="Cursor"/>, <see cref="Display"/>, and <see cref="Window"/> and restores those values once
    /// the <paramref name="terminalAction"/> has completed
    /// </summary>
    /// <param name="terminalAction"></param>
    void Temp(Action<ITerminalInstance> terminalAction);
}