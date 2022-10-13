using System.Drawing;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Jay.Terminalis.Native;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct Rect16 : IEquatable<Rect16>
{
    public static bool operator ==(Rect16 a, Rect16 b) => a.Equals(b);
    public static bool operator !=(Rect16 a, Rect16 b) => !a.Equals(b);
        
    public static Rect16 FromLTRB(short left, short top, short right, short bottom)
    {
        return new Rect16(left, top, right, bottom);
    }
    public static Rect16 FromLTWH(short left, short top, short width, short height)
    {
        return new Rect16(
            left, 
            top, 
            Validate.IsShort(left + width), 
            Validate.IsShort(top + height));
    }
    public static Rect16 FromPointSize(Point16 point, Size16 size)
    {
        return new Rect16(
            point.X, 
            point.Y, 
            Validate.IsShort(point.X + size.Width), 
            Validate.IsShort(point.Y + size.Height));
    }

    public static Rect16 FromLTRB32(int left, int top, int right, int bottom)
    {
        return new Rect16(
            Validate.IsShort(left),
            Validate.IsShort(top),
            Validate.IsShort(right),
            Validate.IsShort(bottom));
    }
    public static Rect16 FromLTWH32(int left, int top, int width, int height) => FromLTRB32(left, top, left + width, top + height);
    public static Rect16 FromPointSize32(Point point, Size size) => FromPointSize(Point16.From32(point), Size16.From32(size));


    [FieldOffset(0)]
    public short Left;
    [FieldOffset(2)] 
    public short Top;
    [FieldOffset(4)] 
    public short Right;
    [FieldOffset(6)] 
    public short Bottom;

    public Point16 Location => new Point16(Left, Top);
    public Size16 Size => new Size16(Width, Height);
    public short Width => (short)(Right - Left);
    public short Height => (short)(Bottom - Top);

    internal Rect16(short left, short top, short right, short bottom)
    {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }

    /// <inheritdoc />
    public bool Equals(Rect16 rect16)
    {
        return rect16.Left == this.Left &&
            rect16.Top == this.Top &&
            rect16.Right == this.Right &&
            rect16.Bottom == this.Bottom;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Rect16 smallRect && Equals(smallRect);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (((int)Left | (int)Top) | (((int)Right | (int)Bottom) << 16));
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({Left},{Top}) \\ ({Right},{Bottom})";
    }
}