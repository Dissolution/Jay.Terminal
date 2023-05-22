namespace Jay.Terminalis.Console;

public interface ITerminalErrorOutput
{
    TextWriter Writer { get; set; }
    bool IsRedirected { get; }

    Stream OpenStream();
    Stream OpenStream(int bufferSize);
}