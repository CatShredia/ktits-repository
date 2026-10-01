using Minesweeper.Core.Services;

namespace Minesweeper.ConsoleApp.Services;

public class ConsoleTimer : IGameTimer
{
    public int ElapsedSeconds { get; private set; }
    public event Action<int>? OnTick;
    private CancellationTokenSource? _cts;

    public void Start()
    {
        _cts = new CancellationTokenSource();
        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await Task.Delay(1000, _cts.Token);
                ElapsedSeconds++;
                OnTick?.Invoke(ElapsedSeconds);
            }
        }, _cts.Token);
    }

    public void Stop() => _cts?.Cancel();
    public void Reset() { Stop(); ElapsedSeconds = 0; }
}