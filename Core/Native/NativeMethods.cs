using System.ComponentModel;
using System.Drawing;
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
    
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hwnd, out Rect32 lpRect);

    public static bool GetWindowRect(IntPtr windowHandle, out Rectangle rectangle)
    {
        if (GetWindowRect(windowHandle, out Rect32 rect))
        {
            rectangle = (Rectangle)rect;
            return true;
        }
        else
        {
            rectangle = Rectangle.Empty;
            return false;
        }
    }
    
    
    /// <summary>
    ///     The MoveWindow function changes the position and dimensions of the specified window. For a top-level window, the
    ///     position and dimensions are relative to the upper-left corner of the screen. For a child window, they are relative
    ///     to the upper-left corner of the parent window's client area.
    ///     <para>
    ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633534%28v=vs.85%29.aspx for more
    ///     information
    ///     </para>
    /// </summary>
    /// <param name="hWnd">C++ ( hWnd [in]. Type: HWND )<br /> Handle to the window.</param>
    /// <param name="X">C++ ( X [in]. Type: int )<br />Specifies the new position of the left side of the window.</param>
    /// <param name="Y">C++ ( Y [in]. Type: int )<br /> Specifies the new position of the top of the window.</param>
    /// <param name="nWidth">C++ ( nWidth [in]. Type: int )<br />Specifies the new width of the window.</param>
    /// <param name="nHeight">C++ ( nHeight [in]. Type: int )<br />Specifies the new height of the window.</param>
    /// <param name="bRepaint">
    ///     C++ ( bRepaint [in]. Type: bool )<br />Specifies whether the window is to be repainted. If this
    ///     parameter is TRUE, the window receives a message. If the parameter is FALSE, no repainting of any kind occurs. This
    ///     applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the
    ///     parent window uncovered as a result of moving a child window.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.<br /> If the function fails, the return value is zero.
    ///     <br />To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    public static bool MoveAndResizeWindow(IntPtr windowHandle, Rectangle newBounds, bool repaint = true)
    {
        return MoveWindow(windowHandle, newBounds.X, newBounds.Y, newBounds.Width, newBounds.Height, repaint);
    }
}