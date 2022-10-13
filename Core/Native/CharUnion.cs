﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jay.Terminalis.Native;

[StructLayout(LayoutKind.Explicit, Size = 2, CharSet = CharSet.Unicode)]
public struct CharUnion : IEquatable<CharUnion>, IEquatable<char>
{
    public static implicit operator CharUnion(char ch) => new CharUnion(ch);
    public static implicit operator char(CharUnion charUnion) => charUnion.UnicodeChar;
        
    public static bool operator ==(CharUnion x, CharUnion y) => x.UnicodeChar == y.UnicodeChar;
    public static bool operator !=(CharUnion x, CharUnion y) => x.UnicodeChar != y.UnicodeChar;
    public static bool operator ==(CharUnion x, char y) => x.UnicodeChar == y;
    public static bool operator !=(CharUnion x, char y) => x.UnicodeChar != y;
        
    [FieldOffset(0)] 
    public byte AsciiChar;
    [FieldOffset(0)] 
    public char UnicodeChar;

    [SkipLocalsInit]
    public CharUnion(char unicodeChar)
    {
        this.UnicodeChar = unicodeChar;
    }

    /// <inheritdoc />
    public bool Equals(CharUnion charUnion)
    {
        return charUnion.UnicodeChar == this.UnicodeChar;
    }

    /// <inheritdoc />
    public bool Equals(char ch)
    {
        return ch == this.UnicodeChar;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is CharUnion charUnion) return charUnion.UnicodeChar == this.UnicodeChar;
        if (obj is char c) return c == this.UnicodeChar;
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode() => throw new InvalidOperationException();
    
    public override string ToString()
    {
        return this.UnicodeChar.ToString();
    }
}