using System.Diagnostics;
using Jay.Terminalis;
using Jay.Terminalis.Native;

var cells = TerminalCells.Instance;
cells.Clear();
var random = Random.Shared;
const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

int i = 0;
long deltaTime = 0L;
ulong frames = 0UL;
double frameRate = 30;  // Assume 30fps average at start
double averageFrameTimeMS = 33.3333d; // assume 1/30 of a second (at 30fps)

Stopwatch stopwatch = Stopwatch.StartNew();

while (!Console.KeyAvailable)
{
    long startTicks = stopwatch.ElapsedTicks;
    
    
    // for (var x = 0; x < cells.Width; x++)
    // for (var y = 0; y < cells.Height; y++)
    // {
    //     ref TerminalCell cell = ref cells[x, y];
    //     cell.Char = chars[random.Next(chars.Length)];
    //     // hack
    //     cell.Colors = (TerminalColors)(byte)random.Next(byte.MaxValue + 1);
    // }
    for (var c = 0; c < cells.Length; c++)
    {
        ref TerminalCell cell = ref cells[c];
        cell.Char = chars[random.Next(62)];
        // hack
        cell.Colors = (TerminalColors)(byte)random.Next(byte.MaxValue + 1);
    }
    cells.Flush();

    frames++;
    long endTicks = stopwatch.ElapsedTicks;
    deltaTime += (endTicks - startTicks);
    
    // Every second, 10k ticks / ms
    if (deltaTime >= TimeSpan.TicksPerSecond)
    {
        // (moving average)
        frameRate = ((double)frames * 0.5d) + (frameRate * 0.5d);
        frames = 0;
        deltaTime -= TimeSpan.TicksPerSecond;
        averageFrameTimeMS = (1000.0d / (frameRate == 0d ? 0.001d : frameRate));
        i++;
        if (i % 3 == 0)
        {
            Console.Title = $"{frameRate:#,##0.0} Frames Per Second  |  {averageFrameTimeMS:#,#00} Average Frame Time (ms)";
            i = 0;
        }
    }
}

