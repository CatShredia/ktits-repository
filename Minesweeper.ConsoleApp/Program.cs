using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;
using Minesweeper.Core.Services;
using Minesweeper.ConsoleApp.Services;
using Minesweeper.Data;
using Minesweeper.Data.Repositories;

var services = new ServiceCollection();
services.AddDataLayer("minesweeper.db");
services.AddTransient<IGameTimer, ConsoleTimer>();
var sp = services.BuildServiceProvider();
await DatabaseInitializer.InitializeAsync(sp);

var userRepo = sp.GetRequiredService<IUserRepository>();
var gameRepo = sp.GetRequiredService<IGameRepository>();
var hasher = sp.GetRequiredService<IPasswordHasher>();

// ── Auth ──
Minesweeper.Data.Models.User? currentUser = null;
while (currentUser == null)
{
    Console.Write("Login: "); var login = Console.ReadLine()!;
    Console.Write("Password: "); var pass = Console.ReadLine()!;
    var user = await userRepo.GetByLoginAsync(login);
    if (user != null && hasher.Verify(pass, user.PasswordHash)) { currentUser = user; }
    else
    {
        Console.Write("User not found. Register? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            currentUser = await userRepo.CreateAsync(login, hasher.Generate(pass));
            Console.WriteLine("Registered!");
        }
    }
}

// ── Main Menu Loop ──
while (true)
{
    Console.WriteLine($"\n=== Minesweeper | {currentUser.Login} ===");
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
    Console.WriteLine("Size: 1=Beginner(9x9) 2=Amateur(12x12) 3=Pro(20x20)");
    var sizeChoice = Console.ReadLine();
    var (w, h, m, size) = sizeChoice switch
    {
        "2" => (12, 12, 20, GameSize.Amateur),
        "3" => (20, 20, 40, GameSize.Professional),
        _ => (9, 9, 10, GameSize.Beginner)
    };

    var timer = sp.GetRequiredService<IGameTimer>();
    var session = new GameSession(w, h, m, timer);
    int timerRow = 0;
    object consoleLock = new();

    timer.OnTick += s => { lock (consoleLock) { WriteAt(10, timerRow, $"Time: {s}s   "); } };

    session.OnCellsRevealed += cells => { lock (consoleLock) { foreach (var c in cells) DrawCell(c, w); } };
    session.OnCellFlagged += c => { lock (consoleLock) { DrawCell(c, w); } };
    session.OnGameEnded += status =>
    {
        lock (consoleLock)
        {
            Console.SetCursorPosition(0, h + 3);
            Console.WriteLine(status == GameStatus.Win ? "🎉 YOU WIN!" : "💥 BOOM! You lost.");
            // Reveal all mines on lose
            if (status == GameStatus.Lose)
                for (int x = 0; x < w; x++) for (int y = 0; y < h; y++)
                    if (session.Board.Grid[x, y].IsMine) DrawCell(session.Board.Grid[x, y], w);
        }
    };

    Console.Clear();
    Console.WriteLine($"Time: 0s");
    Console.WriteLine();
    Console.Write("   ");
    for (int x = 0; x < w; x++) Console.Write($"{x,2}");
    Console.WriteLine();
    for (int y = 0; y < h; y++)
    {
        Console.Write($"{y,2} ");
        for (int x = 0; x < w; x++) Console.Write(" .");
        Console.WriteLine();
    }
    Console.WriteLine();

    while (session.Status == GameStatus.InProgress)
    {
        Console.SetCursorPosition(0, h + 4);
        Console.Write("Command (open x y / flag x y): ");
        var input = Console.ReadLine()?.Split(' ');
        if (input is not { Length: 3 }) continue;
        if (!int.TryParse(input[1], out int cx) || !int.TryParse(input[2], out int cy)) continue;
        if (cx < 0 || cx >= w || cy < 0 || cy >= h) continue;
        session.MakeMove(cx, cy, input[0] == "flag");
    }

    await gameRepo.SaveGameAsync(currentUser.Id, size, session.Status, timer.ElapsedSeconds, session.Board.GetMineMap());
    Console.WriteLine("Game saved. Press Enter...");
    Console.ReadLine();
}

void DrawCell(Cell c, int boardWidth)
{
    int col = 3 + c.X * 2;
    int row = 3 + c.Y;
    Console.SetCursorPosition(col, row);
    if (c.IsFlagged) Console.Write(" F");
    else if (!c.IsRevealed) Console.Write(" .");
    else if (c.IsMine) Console.Write(" *");
    else Console.Write(c.AdjacentMines == 0 ? "  " : $" {c.AdjacentMines}");
}

void WriteAt(int x, int y, string text) { Console.SetCursorPosition(x, y); Console.Write(text); }

async Task ShowHistory()
{
    var games = await gameRepo.GetUserHistoryAsync(currentUser!.Id);
    Console.WriteLine("\n--- Your History ---");
    foreach (var g in games)
        Console.WriteLine($"{g.Date:g} | {g.Size} | {g.Status} | {g.TimeInSeconds}s");
}

async Task ShowLeaderboard()
{
    Console.WriteLine("Size: 1=Beginner 2=Amateur 3=Pro");
    var s = Console.ReadLine() switch { "2" => GameSize.Amateur, "3" => GameSize.Professional, _ => GameSize.Beginner };
    var top = await gameRepo.GetLeaderboardAsync(s);
    Console.WriteLine($"\n--- Top 10 ({s}) ---");
    for (int i = 0; i < top.Count; i++)
        Console.WriteLine($"{i + 1}. User#{top[i].UserId} — {top[i].TimeInSeconds}s ({top[i].Date:g})");
}