namespace Minesweeper.Core.Models;

public enum GameStatus { InProgress, Win, Lose }

public enum GameSize 
{ 
    Beginner = 0,    // 9x9, 10 mines
    Amateur = 1,     // 12x12, 20 mines
    Professional = 2 // 20x20, 40 mines
}