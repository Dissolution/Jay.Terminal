using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jay.Terminalis.Native;
using Microsoft.Toolkit.HighPerformance;

// ReSharper disable RedundantAssignment

namespace Jay.Terminalis;

/// <summary>
/// Modifies a <c>ref</c> <see cref="TerminalCell"/> argument
/// </summary>
public delegate void ModifyCell(ref TerminalCell cell);
    
public sealed class TerminalCells
{
    public static TerminalCells Instance { get; } = new TerminalCells();
        
    private readonly IntPtr _consoleHandle;
    private readonly TerminalCell[] _cells;

    public int Width { get; }
    public int Height { get; }

    public ref TerminalCell this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Cells[index];
    }

    public ref TerminalCell this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Cells2D[row: y, column: x];
    }

    public Span<TerminalCell> Cells
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _cells;
    }

    public Span2D<TerminalCell> Cells2D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Span2D<TerminalCell>(_cells, Height, Width);
    }

    internal IntPtr ConsoleHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _consoleHandle;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _cells.Length;
    }

    public TerminalCells()
    {
        _consoleHandle = NativeMethods.GetConsoleHandle();
        var info = new NativeMethods.ConsoleScreenBufferInfo();
        NativeMethods.GetConsoleScreenBufferInfo(_consoleHandle, ref info);
        if (!info.Size.IsValid)
            throw new InvalidOperationException();
        this.Width = info.Size.Width;
        this.Height = info.Size.Height;
        _cells = new TerminalCell[Width * Height];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Rect16 AllCellsRect() => Rect16.FromLTWH32(0, 0, Width, Height);

    /// <summary>
    /// Refreshes the <see cref="Cells"/> to be consistent with what is currently on the Terminal's screen
    /// </summary>
    public void Refresh()
    {
        var rect = AllCellsRect();
        bool read = NativeMethods.ReadConsoleOutput(_consoleHandle,
            _cells,
            rect.Size,
            rect.Location,
            ref rect);
        if (!read)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    /// Modify all <see cref="TerminalCell"/>s
    /// </summary>
    /// <param name="modifyCell">The action to perform upon each TerminalCell</param>
    public void ModifyCells(ModifyCell modifyCell)
    {
        var cells = Cells;
        int length = cells.Length;
        for (var i = 0; i < length; i++)
        {
            modifyCell(ref cells[i]);
        }
    }

    /// <summary>
    /// Sets all <see cref="TerminalCell"/>s to the <paramref name="template"/>
    /// </summary>
    public void SetCells(TerminalCell template)
    {
        ModifyCells((ref TerminalCell cell) => cell = template);
        Flush();
    }

    /// <summary>
    /// Flush all changes to <see cref="Cells"/> to the Terminal
    /// </summary>
    public void Flush()
    {
        var rect = AllCellsRect();
        bool wrote = NativeMethods.WriteConsoleOutput(_consoleHandle,
            _cells,
            rect.Size,
            rect.Location,
            ref rect);
        if (!wrote)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
        
    /// <summary>
    /// Clears the <see cref="Cells"/> by returning them to their defaults
    /// </summary>
    public void Clear()
    {
        ModifyCells((ref TerminalCell cell) =>
        {
            cell = TerminalCell.Default;
        });
        Flush();
    }
}
