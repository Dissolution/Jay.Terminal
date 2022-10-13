using System.Drawing;
using System.Runtime.InteropServices;

namespace Jay.Terminalis.Native;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct Point16 : IEquatable<Point16>
{
    public static bool operator ==(Point16 a, Point16 b) => a.Equals(b);
    public static bool operator !=(Point16 a, Point16 b) => !a.Equals(b);

    public static readonly Point16 Empty = default;
    
    public static Point16 From32(Point point) => new Point16(Validate.IsShort(point.X), Validate.IsShort(point.Y));
    
    public static Point16 From32(int x, int y) => new Point16(Validate.IsShort(x), Validate.IsShort(y));
    
    [FieldOffset(0)] 
    public short X;
    [FieldOffset(2)] 
    public short Y;

    public Point16(short x, short y)
    {
        this.X = x;
        this.Y = y;
    }
        
    /// <inheritdoc />
    public bool Equals(Point16 point16)
    {
        return X == point16.X && Y == point16.Y;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Point16 point && Equals(point);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (int)((int)X | (int)Y << 16);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"({X},{Y})";
    }
}