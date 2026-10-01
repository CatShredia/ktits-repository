using Minesweeper.Core.Models;

namespace Minesweeper.Core.Services;

public class GameSession
{
    public Board Board { get; }
    public GameStatus Status { get; private set; } = GameStatus.InProgress;
    public IGameTimer Timer { get; }

    public event Action<List<Cell>>? OnCellsRevealed;
    public event Action<Cell>? OnCellFlagged;
    public event Action<GameStatus>? OnGameEnded;

    public GameSession(int width, int height, int mines, IGameTimer timer)
    {
        Board = new Board(width, height, mines);
        Timer = timer;
    }

    public void MakeMove(int x, int y, bool isFlagging)
    {
        if (Status != GameStatus.InProgress) return;

        if (isFlagging)
        {
            Board.ToggleFlag(x, y);
            OnCellFlagged?.Invoke(Board.Grid[x, y]);
            return;
        }

        var revealed = Board.RevealCell(x, y);
        if (revealed.Count > 0 && Timer.ElapsedSeconds == 0) Timer.Start();

        OnCellsRevealed?.Invoke(revealed);

        if (revealed.Any(c => c.IsMine))
        {
            Status = GameStatus.Lose;
            Timer.Stop();
            OnGameEnded?.Invoke(Status);
        }
        else if (Board.CheckWin())
        {
            Status = GameStatus.Win;
            Timer.Stop();
            OnGameEnded?.Invoke(Status);
        }
    }
}