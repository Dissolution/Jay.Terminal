using System.Runtime.CompilerServices;

namespace Jay.Terminalis;

internal static class Validate
{
    public static short IsShort(int value, [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        if (value is <= short.MaxValue and >= short.MinValue) return (short)value;
        throw new ArgumentOutOfRangeException(valueName,
            value,
            $"{valueName} must be within Int16 constraints [{short.MinValue}..{short.MaxValue}]");
    }
}