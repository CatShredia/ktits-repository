namespace Minesweeper.Core.Services;

public interface IGameTimer
{
    int ElapsedSeconds { get; }
    event Action<int> OnTick; // UI subscribes to this
    
    void Start();
    void Stop();
    void Reset();
}