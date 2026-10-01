namespace Minesweeper.Core.Models;

public class Cell
{
    public int X { get; }
    public int Y { get; }
    public bool IsMine { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsFlagged { get; set; }
    public int AdjacentMines { get; set; }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }
}