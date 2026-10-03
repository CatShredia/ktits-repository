namespace Minesweeper.Core.Services;

public interface IGameTimer
{
    int ElapsedSeconds { get; }
    event Action<int> OnTick;
    
    void Start();
    void Stop();
    void Reset();
}