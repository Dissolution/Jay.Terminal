using System.Drawing;

namespace Jay.Terminalis;

/// <summary>
/// Manages <see cref="TerminalColor"/> to <see cref="Color"/> conversion
/// </summary>
public static class TerminalColors
{
    private static readonly Color[] _terminalColorToColorMap;
    private const double _matchEpsilon = 0.001d;

    static TerminalColors()
    {
        _terminalColorToColorMap = new Color[16]
        {
            /* Black       */ Color.FromArgb(0, 0, 0),
            /* DarkBlue    */ Color.FromArgb(0, 0, 128),
            /* DarkGreen   */ Color.FromArgb(0, 128, 0),
            /* DarkCyan    */ Color.FromArgb(0, 128, 128),
            /* DarkRed     */ Color.FromArgb(128, 0, 0),
            /* DarkMagenta */ Color.FromArgb(128, 0, 128),
            /* DarkYellow  */ Color.FromArgb(128, 128, 0),
            /* Gray        */ Color.FromArgb(192, 192, 192),
            /* DarkGray    */ Color.FromArgb(128, 128, 128),
            /* Blue        */ Color.FromArgb(0, 0, 255),
            /* Green       */ Color.FromArgb(0, 255, 0),
            /* Cyan        */ Color.FromArgb(0, 255, 255),
            /* Red         */ Color.FromArgb(255, 0, 0),
            /* Magenta     */ Color.FromArgb(255, 0, 255),
            /* Yellow      */ Color.FromArgb(255, 255, 0),
            /* White       */ Color.FromArgb(255, 255, 255),
        };
    }

    /// <summary>
    /// Gets the closest <see cref="TerminalColor"/> match for the given <paramref name="color"/>
    /// </summary>
    public static TerminalColor GetClosestTerminalColor(Color color)
    {
        // Simple dist check
        TerminalColor closestColor = default;
        double minDelta = double.MaxValue;

        for (int i = 0; i < _terminalColorToColorMap.Length; i++)
        {
            Color checkColor = _terminalColorToColorMap[i];
            var diff = Math.Pow(checkColor.R - color.R, 2d) +
                Math.Pow(checkColor.G - color.G, 2d) +
                Math.Pow(checkColor.B - color.B, 2d);
            if (diff < minDelta)
            {
                closestColor = (TerminalColor)i;
                minDelta = diff;
            }
        }
        return closestColor;
    }

    /// <summary>
    /// Gets the <see cref="Color"/> representation of this <paramref name="terminalColor"/>
    /// </summary>
    public static Color ToColor(this TerminalColor terminalColor)
    {
        return _terminalColorToColorMap[(int)terminalColor];
    }
}