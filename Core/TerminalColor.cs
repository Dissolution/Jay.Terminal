using System.Drawing;

namespace Jay.Terminalis;

[Flags]
public enum TerminalColor : byte
{
    Black = 0b00000000, //  0  Black
    DarkBlue = 0b00000001, //  1  Blue
    DarkGreen = 0b00000010, //  2  Green
    DarkCyan = 0b00000011, //  3  Cyan = Green | Blue
    DarkRed = 0b00000100, //  4  Red
    DarkMagenta = 0b00000101, //  5  Magenta = Red | Blue
    DarkYellow = 0b00000110, //  6  Yellow = Red | Green
    Gray = 0b00000111, //  7  Gray = Red | Green | Blue
    DarkGray = 0b00001000, //  8  Intense Black
    Blue = 0b00001001, //  9  Intense Blue
    Green = 0b00001010, // 10  Intense Green
    Cyan = 0b00001011, // 11  Intense Cyan = Intense | Green | Blue
    Red = 0b00001100, // 12  Intense Red
    Magenta = 0b00001101, // 13  Intense Magenta = Intense | Red | Blue
    Yellow = 0b00001110, // 14  Intense Yellow = Intense | Red | Green
    White = 0b00001111, // 15  Intense Gray = Intense | Red | Green | Blue
}