namespace Minesweeper.Core.Models;

public class Board
{
    public int Width { get; }
    public int Height { get; }
    public int TotalMines { get; }
    public Cell[,] Grid { get; }

    private bool _minesPlaced = false;
    private int _revealedSafeCells = 0;

    public Board(int width, int height, int totalMines)
    {
        Width = width;
        Height = height;
        TotalMines = totalMines;
        Grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Grid[x, y] = new Cell(x, y);
    }

    // First-click safety: generate mines only after the first move
    public void PlaceMines(int safeX, int safeY)
    {
        if (_minesPlaced) return;

        var random = new Random();
        int minesToPlace = TotalMines;

        while (minesToPlace > 0)
        {
            int x = random.Next(Width);
            int y = random.Next(Height);

            // Ensure the first clicked cell and its neighbors are safe
            if (Math.Abs(x - safeX) <= 1 && Math.Abs(y - safeY) <= 1) continue;
            if (Grid[x, y].IsMine) continue;

            Grid[x, y].IsMine = true;
            minesToPlace--;
        }

        CalculateAdjacentMines();
        _minesPlaced = true;
    }

    private void CalculateAdjacentMines()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Grid[x, y].IsMine) continue;
                Grid[x, y].AdjacentMines = CountNeighbors(x, y);
            }
        }
    }

    private int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && Grid[nx, ny].IsMine)
                    count++;
            }
        }
        return count;
    }

    // Returns list of cells that were revealed (for UI updates)
    public List<Cell> RevealCell(int x, int y)
    {
        if (!_minesPlaced) PlaceMines(x, y);

        var cell = Grid[x, y];
        if (cell.IsRevealed || cell.IsFlagged) return new List<Cell>();

        cell.IsRevealed = true;
        var revealed = new List<Cell> { cell };

        if (cell.IsMine) return revealed; // Hit a mine

        _revealedSafeCells++;

        // Cascade reveal (BFS) for empty cells
        if (cell.AdjacentMines == 0)
        {
            var queue = new Queue<Cell>();
            queue.Enqueue(cell);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = current.X + dx, ny = current.Y + dy;
                        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                        {
                            var neighbor = Grid[nx, ny];
                            if (!neighbor.IsRevealed && !neighbor.IsFlagged && !neighbor.IsMine)
                            {
                                neighbor.IsRevealed = true;
                                _revealedSafeCells++;
                                revealed.Add(neighbor);
                                if (neighbor.AdjacentMines == 0) queue.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }
        }
        return revealed;
    }

    public void ToggleFlag(int x, int y)
    {
        var cell = Grid[x, y];
        if (!cell.IsRevealed) cell.IsFlagged = !cell.IsFlagged;
    }

    public bool CheckWin() => _revealedSafeCells == (Width * Height - TotalMines);

    public bool[,] GetMineMap()
    {
        var map = new bool[Width, Height];
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                map[x, y] = Grid[x, y].IsMine;
        return map;
    }
}