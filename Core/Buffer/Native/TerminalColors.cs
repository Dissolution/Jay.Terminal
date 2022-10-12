using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Terminalis.Buff.Native;

[StructLayout(LayoutKind.Explicit, Size = sizeof(byte))]
public struct TerminalColors : IEquatable<TerminalColors>
{
    public static implicit operator TerminalColors((TerminalColor ForeColor, TerminalColor BackColor) colors)
        => new TerminalColors(colors.ForeColor, colors.BackColor);

    public static bool operator ==(TerminalColors x, TerminalColors y) => x.Value == y.Value;
    public static bool operator !=(TerminalColors x, TerminalColors y) => x.Value != y.Value;
        
    public static readonly TerminalColors None = new TerminalColors();

    [FieldOffset(0)] 
    internal byte Value;

    public TerminalColor Foreground
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TerminalColor) (Value & 0b00001111);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Value = (byte) ((Value & 0b11110000) | (byte) value);
    }
        
    public TerminalColor Background
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TerminalColor) ((Value & 0b11110000) >> 4);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Value = (byte) ((Value & 0b00001111) | ((int) value << 4));
    }

    public TerminalColors(TerminalColor foreColor, TerminalColor backColor)
    {
        Value = (byte) ((int) foreColor | ((int) backColor << 4));
    }

    /// <inheritdoc />
    public bool Equals(TerminalColors colors)
    {
        return colors.Value == this.Value;
    }
        
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is TerminalColors colors && colors.Value == this.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Foreground}/{Background}";
    }
}