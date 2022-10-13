using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance;

namespace Jay.Terminalis.Native;

public static class CellsExtensions
{
    public static void ForEach(this Span<TerminalCell> cells, ModifyCell modifyCell)
    {
        for (var i = 0; i < cells.Length; i++)
        {
            modifyCell(ref cells[i]);
        }
    }

    public static void ForEach(this Span2D<TerminalCell> cells, ModifyCell modifyCell)
    {
        if (cells.IsEmpty) return;

        if (cells.TryGetSpan(out Span<TerminalCell> span))
        {
            span.ForEach(modifyCell);
        }
        else
        {
            // Fill one row at a time
            for (int row = 0; row < cells.Height; row++)
            {
                cells.GetRowSpan(row).ForEach(modifyCell);
            }
        }
    }

    public static bool TryGetNeighbor(this Span2D<TerminalCell> cells,
        int startX, int startY, Direction direction,
        out TerminalCell cell)
    {
        int x = startX;
        int y = startY;
        if (direction.HasFlag(Direction.Left))
        {
            x -= 1;
        }
        if (direction.HasFlag(Direction.Top))
        {
            y -= 1;
        }
        if (direction.HasFlag(Direction.Right))
        {
            x += 1;
        }
        if (direction.HasFlag(Direction.Bottom))
        {
            y += 1;
        }
        if ((uint)x < cells.Width && (uint)y < cells.Height)
        {
            cell = cells[row: y, column: x];
            return true;
        }
        cell = default;
        return false;
    }

    public static ref TerminalCell GetNeighbor(this Span2D<TerminalCell> cells,
        int startX, int startY, Direction direction)
    {
        int x = startX;
        int y = startY;
        if (direction.HasFlag(Direction.Left))
        {
            x -= 1;
        }
        if (direction.HasFlag(Direction.Top))
        {
            y -= 1;
        }
        if (direction.HasFlag(Direction.Right))
        {
            x += 1;
        }
        if (direction.HasFlag(Direction.Bottom))
        {
            y += 1;
        }
        if ((uint)x < cells.Width && (uint)y < cells.Height)
        {
            return ref cells.DangerousGetReferenceAt(y, x);
        }
        else
        {
            return ref Unsafe.NullRef<TerminalCell>();
        }
    }
}

[Flags]
public enum Direction
{
    None = 0,
    
    Left = 1 << 0,
    Top = 1 << 1,
    Right = 1 << 2,
    Bottom = 1 << 3,
    
    UpperLeft = Left | Top,
    UpperRight = Right | Top,
    LowerLeft = Left | Bottom,
    LowerRight = Right | Bottom,
}