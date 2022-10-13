using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Jay.Terminalis.Native;

internal static class NativeMethods
{
    private const int STD_INPUT_HANDLE = -10;
    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_ERROR_HANDLE = -12;
    private static readonly IntPtr INVALID_HANDLE = new IntPtr(-1);
    
  
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput,
        ref ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

    /// <seealso cref="https://learn.microsoft.com/en-us/windows/console/writeconsoleoutput"/>
    [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WriteConsoleOutput(IntPtr hConsoleOutput,
        TerminalCell[] lpBuffer,
        Size16 dwBufferSize,
        Point16 dwBufferShortPoint,
        ref Rect16 lpWriteRegion);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadConsoleOutput(IntPtr hConsoleOutput,
        [Out] TerminalCell[] lpBuffer,
        Size16 dwBufferSize,
        Point16 dwBufferShortPoint,
        ref Rect16 lpReadRegion);
       
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    /// <remarks>
    /// As per MS documentation, the Handle retrieved does not have to be released, disposed, nor returned.
    /// </remarks>
    public static IntPtr GetConsoleHandle()
    {
        IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (handle == INVALID_HANDLE)
        {
            int errorId = Marshal.GetLastWin32Error();
            throw new Win32Exception(errorId);
        }
        return handle;
    }

    [StructLayout(LayoutKind.Explicit, Size = 22)]
    public struct ConsoleScreenBufferInfo
    {
        [FieldOffset(0)]
        public Size16 Size;
        [FieldOffset(4)]
        public Point16 CursorPosition;
        [FieldOffset(8)]
        public TerminalColors Colors;
        [FieldOffset(9)] 
        public CommonLVB CommonLvb;
        [FieldOffset(10)]
        public Rect16 Window;
        [FieldOffset(18)]
        public Size16 MaximumWindowSize;
    }
}