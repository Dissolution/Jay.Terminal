namespace Jay.Terminalis.Console;

public interface ITerminalBeep
{
    /// <summary>
    /// Plays a beep through the console speaker.
    /// </summary>
    void Beep();

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    void Beep(int frequency, int duration);

    /// <summary>
    /// Plays a beep of specified frequency and duration through the console speaker.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    void Beep(int frequency, TimeSpan duration);
}