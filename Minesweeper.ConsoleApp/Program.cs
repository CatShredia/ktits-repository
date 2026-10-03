using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;
using Minesweeper.Core.Services;
using Minesweeper.ConsoleApp.Services;
using Minesweeper.Data;
using Minesweeper.Data.Models;
using Minesweeper.Data.Repositories;

var services = new ServiceCollection();
services.AddDataLayer();
services.AddTransient<IGameTimer, ConsoleTimer>();
var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
await DatabaseInitializer.InitializeAsync(sp);

var hasher = sp.GetRequiredService<IPasswordHasher>();

async Task RunInDb(Func<IServiceProvider, Task> work)
{
    await using var scope = sp.CreateAsyncScope();
    await work(scope.ServiceProvider);
}

async Task<T> FromDb<T>(Func<IServiceProvider, Task<T>> work)
{
    await using var scope = sp.CreateAsyncScope();
    return await work(scope.ServiceProvider);
}

User? currentUser = null;
while (currentUser == null)
{
    Console.Clear();
    Console.WriteLine("=== MINESWEEPER ===\n");
    Console.Write("Login: "); var login = Console.ReadLine()!;
    Console.Write("Password: "); var pass = Console.ReadLine()!;
    var user = await FromDb(s => s.GetRequiredService<IUserRepository>().GetByLoginAsync(login));
    if (user != null && hasher.Verify(pass, user.PasswordHash))
        currentUser = user;
    else
    {
        Console.Write("User not found. Register? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            currentUser = await FromDb(s => s.GetRequiredService<IUserRepository>().CreateAsync(login, hasher.Generate(pass)));
            Console.WriteLine("Registered!");
        }
    }
}

while (true)
{
    Console.Clear();
    Console.WriteLine($"=== Minesweeper | {currentUser.Login} ===\n");
    Console.WriteLine("1. New Game\n2. History\n3. Leaderboard\n4. Exit");
    Console.Write("> ");
    switch (Console.ReadLine())
    {
        case "1": await PlayGame(); break;
        case "2": await ShowHistory(); break;
        case "3": await ShowLeaderboard(); break;
        case "4": return;
    }
}

async Task PlayGame()
{
    Console.Clear();
    Console.WriteLine("Size: 1=Beginner(9x9) 2=Amateur(12x12) 3=Pro(20x20)");
    Console.Write("> ");
    var sizeChoice = Console.ReadLine();
    var (w, h, m, size) = sizeChoice switch
    {
        "2" => (12, 12, 20, GameSize.Amateur),
        "3" => (20, 20, 40, GameSize.Professional),
        _ => (9, 9, 10, GameSize.Beginner)
    };

    try
    {
        int needH = h + 8;
        int needW = w * 3 + 6;
        if (OperatingSystem.IsWindows())
        {
            Console.BufferWidth = Math.Max(Console.BufferWidth, needW);
            Console.BufferHeight = Math.Max(Console.BufferHeight, needH);
            Console.WindowWidth = Math.Min(Console.LargestWindowWidth, needW);
            Console.WindowHeight = Math.Min(Console.LargestWindowHeight, needH);
        }
    }
    catch { }

    Console.Title = $"Minesweeper - {size} ({w}x{h})";

    var timer = sp.GetRequiredService<IGameTimer>();
    var session = new GameSession(w, h, m, timer);

    int inputRow = h + 4;
    object lockObj = new();

    void DrawHeader()
    {
        Console.SetCursorPosition(0, 2);
        Console.Write("   ");
        for (int x = 0; x < w; x++) Console.Write($"{x,2} ");
    }

    void DrawFullBoard()
    {
        for (int y = 0; y < h; y++)
        {
            Console.SetCursorPosition(0, 3 + y);
            Console.Write($"{y,2} ");
            for (int x = 0; x < w; x++)
            {
                var c = session.Board.Grid[x, y];
                WriteCellSymbol(c);
            }
        }
    }

    void WriteCellSymbol(Cell c)
    {
        if (c.IsFlagged) Console.Write(" F ");
        else if (!c.IsRevealed) Console.Write(" . ");
        else if (c.IsMine) Console.Write(" * ");
        else Console.Write(c.AdjacentMines == 0 ? "   " : $" {c.AdjacentMines} ");
    }

    void DrawInputLine(string text = "")
    {
        Console.SetCursorPosition(0, inputRow);
        Console.Write(new string(' ', Console.BufferWidth - 1));
        Console.SetCursorPosition(0, inputRow);
        Console.Write(text);
    }

    void ReturnCursorToInput()
    {
        Console.SetCursorPosition(0, inputRow);
    }

    timer.OnTick += s =>
    {
        lock (lockObj)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write($"Time: {s}s    ");
            ReturnCursorToInput();
        }
    };

    session.OnCellsRevealed += cells =>
    {
        lock (lockObj)
        {
            foreach (var c in cells)
            {
                Console.SetCursorPosition(3 + c.X * 3, 3 + c.Y);
                WriteCellSymbol(c);
            }
            ReturnCursorToInput();
        }
    };

    session.OnCellFlagged += c =>
    {
        lock (lockObj)
        {
            Console.SetCursorPosition(3 + c.X * 3, 3 + c.Y);
            WriteCellSymbol(c);
            ReturnCursorToInput();
        }
    };

    session.OnGameEnded += status =>
    {
        lock (lockObj)
        {
            if (status == GameStatus.Lose)
            {
                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                        if (session.Board.Grid[x, y].IsMine)
                        {
                            Console.SetCursorPosition(3 + x * 3, 3 + y);
                            WriteCellSymbol(session.Board.Grid[x, y]);
                        }
            }

            Console.SetCursorPosition(0, 1);
            Console.Write(status == GameStatus.Win
                ? "YOU WIN!           "
                : "BOOM! You lost.    ");
            ReturnCursorToInput();
        }
    };

    Console.Clear();
    lock (lockObj)
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("Time: 0s");
        Console.SetCursorPosition(0, 1);
        Console.Write("Status: In Progress");
        DrawHeader();
        DrawFullBoard();
        DrawInputLine("Command (open X Y / flag X Y): ");
    }

    while (session.Status == GameStatus.InProgress)
    {
        lock (lockObj)
        {
            DrawInputLine("Command (open X Y / flag X Y): ");
        }

        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) continue;

        var parts = input.Trim().Split(' ');
        if (parts.Length != 3) continue;
        if (!int.TryParse(parts[1], out int cx) || !int.TryParse(parts[2], out int cy)) continue;
        if (cx < 0 || cx >= w || cy < 0 || cy >= h) continue;

        session.MakeMove(cx, cy, parts[0] == "flag");
    }

    await RunInDb(s => s.GetRequiredService<IGameRepository>().SaveGameAsync(
        currentUser.Id, size, session.Status,
        timer.ElapsedSeconds, session.Board.GetMineMap()));

    lock (lockObj)
    {
        DrawInputLine("Game saved. Press Enter to continue...");
    }
    Console.ReadLine();
    Console.Title = "Minesweeper";
}

async Task ShowHistory()
{
    Console.Clear();
    var games = await FromDb(s => s.GetRequiredService<IGameRepository>().GetUserHistoryAsync(currentUser!.Id));
    Console.WriteLine("--- Your History ---\n");
    if (games.Count == 0)
    {
        Console.WriteLine("No games yet.");
        Console.WriteLine("\nPress Enter...");
        Console.ReadLine();
        return;
    }

    for (int i = 0; i < games.Count; i++)
        Console.WriteLine($"{i + 1,2}. {games[i].Date:g} | {games[i].Size,-12} | {games[i].Status,-10} | {games[i].TimeInSeconds}s");
    Console.Write("\nMap number (Enter to go back): ");
    if (int.TryParse(Console.ReadLine(), out int number) && number >= 1 && number <= games.Count)
        ShowMineMap(games[number - 1]);
}

async Task ShowLeaderboard()
{
    Console.Clear();
    Console.WriteLine("Size: 1=Beginner 2=Amateur 3=Pro");
    Console.Write("> ");
    var s = Console.ReadLine() switch
    {
        "2" => GameSize.Amateur,
        "3" => GameSize.Professional,
        _ => GameSize.Beginner
    };
    var top = await FromDb(db => db.GetRequiredService<IGameRepository>().GetLeaderboardAsync(s));
    Console.WriteLine($"\n--- Top 10 ({s}) ---\n");
    if (top.Count == 0)
    {
        Console.WriteLine("No records yet.");
        Console.WriteLine("\nPress Enter...");
        Console.ReadLine();
        return;
    }

    for (int i = 0; i < top.Count; i++)
        Console.WriteLine($"{i + 1,2}. User#{top[i].UserId} — {top[i].TimeInSeconds}s ({top[i].Date:g})");
    Console.Write("\nMap number (Enter to go back): ");
    if (int.TryParse(Console.ReadLine(), out int number) && number >= 1 && number <= top.Count)
        ShowMineMap(top[number - 1]);
}

void ShowMineMap(Game game)
{
    Console.Clear();
    Console.WriteLine($"--- Mine map | {game.Size} | {game.Status} | {game.TimeInSeconds}s | {game.Date:g} ---\n");
    var map = MineMapCodec.Decode(game.MineMap);
    if (map == null)
    {
        Console.WriteLine("Map is unavailable.");
    }
    else
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
                Console.Write(map[x, y] ? " * " : " . ");
            Console.WriteLine();
        }
    }

    Console.WriteLine("\nPress Enter...");
    Console.ReadLine();
}