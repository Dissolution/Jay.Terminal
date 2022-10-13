using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Terminalis.Native;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct Size16 : IEquatable<Size16>
{
    public static bool operator ==(Size16 a, Size16 b) => a.Equals(b);
    public static bool operator !=(Size16 a, Size16 b) => !a.Equals(b);

    public static readonly Size16 Empty = default;

    public static Size16 From32(Size size)  => new Size16(Validate.IsShort(size.Width), Validate.IsShort(size.Height));

    public static Size16 From32(int width, int height) => new Size16(Validate.IsShort(width), Validate.IsShort(height));
    
    [FieldOffset(0)] 
    public short Width;
    [FieldOffset(2)] 
    public short Height;

    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Width > 0 && Height > 0;
    }
    
    public Size16(short width, short height)
    {
        this.Width = width;
        this.Height = height;
    }
        
    /// <inheritdoc />
    public bool Equals(Size16 size)
    {
        return this.Width == size.Width &&
            this.Height == size.Height;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Size16 size && Equals(size);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (int)((int)Width | (int)Height << 16);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Width}x{Height}]";
    }
}