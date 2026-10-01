using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;
using Minesweeper.Core.Services;
using Minesweeper.ConsoleApp.Services;
using Minesweeper.Data;
using Minesweeper.Data.Models;
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
User? currentUser = null;
while (currentUser == null)
{
    Console.Clear();
    Console.WriteLine("=== MINESWEEPER ===\n");
    Console.Write("Login: "); var login = Console.ReadLine()!;
    Console.Write("Password: "); var pass = Console.ReadLine()!;
    var user = await userRepo.GetByLoginAsync(login);
    if (user != null && hasher.Verify(pass, user.PasswordHash))
        currentUser = user;
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

// ── Game ──
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

    // Resize console buffer to fit the board
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
    catch { /* Non-Windows or restricted terminal — ignore */ }

    Console.Title = $"Minesweeper - {size} ({w}x{h})";

    var timer = sp.GetRequiredService<IGameTimer>();
    var session = new GameSession(w, h, m, timer);

    // Screen layout:
    // Row 0: Timer + Status
    // Row 1: empty
    // Row 2: column headers
    // Row 3..3+h-1: board rows
    // Row 3+h: empty
    // Row 4+h: input prompt
    int inputRow = h + 4;
    object lockObj = new();

    // ── Drawing helpers ──
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
        Console.Write(new string(' ', Console.BufferWidth - 1)); // clear line
        Console.SetCursorPosition(0, inputRow);
        Console.Write(text);
    }

    void ReturnCursorToInput()
    {
        Console.SetCursorPosition(0, inputRow);
    }

    // ── Subscribe to events ──
    timer.OnTick += s =>
    {
        lock (lockObj)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write($"⏱ Time: {s}s    ");
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
            // Reveal all mines on lose
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
                ? "🎉 YOU WIN!              "
                : "💥 BOOM! You lost.       ");
            ReturnCursorToInput();
        }
    };

    // ── Initial draw ──
    Console.Clear();
    lock (lockObj)
    {
        Console.SetCursorPosition(0, 0);
        Console.Write("⏱ Time: 0s");
        Console.SetCursorPosition(0, 1);
        Console.Write("Status: In Progress");
        DrawHeader();
        DrawFullBoard();
        DrawInputLine("Command (open X Y / flag X Y): ");
    }

    // ── Input loop ──
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

    // ── Save & exit ──
    await gameRepo.SaveGameAsync(
        currentUser.Id, size, session.Status,
        timer.ElapsedSeconds, session.Board.GetMineMap());

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
    var games = await gameRepo.GetUserHistoryAsync(currentUser!.Id);
    Console.WriteLine("--- Your History ---\n");
    if (games.Count == 0) Console.WriteLine("No games yet.");
    foreach (var g in games)
        Console.WriteLine($"{g.Date:g} | {g.Size,-12} | {g.Status,-10} | {g.TimeInSeconds}s");
    Console.WriteLine("\nPress Enter...");
    Console.ReadLine();
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
    var top = await gameRepo.GetLeaderboardAsync(s);
    Console.WriteLine($"\n--- Top 10 ({s}) ---\n");
    if (top.Count == 0) Console.WriteLine("No records yet.");
    for (int i = 0; i < top.Count; i++)
        Console.WriteLine($"{i + 1,2}. User#{top[i].UserId} — {top[i].TimeInSeconds}s ({top[i].Date:g})");
    Console.WriteLine("\nPress Enter...");
    Console.ReadLine();
}