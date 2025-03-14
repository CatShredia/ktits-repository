// Данный класс SeaFight реализует игровую логику для морского боя между игроком и ботом. 
// Он управляет началом игры, обработкой выстрелов игрока, проверкой победителей и отображением состояния игры. 
// Игрок и бот имеют свои уровни, которые отображают состояние их кораблей, и игра продолжается до тех пор, пока один из игроков не победит или не будет ничья.
namespace OAIP
{
    using static System.Console;
    class SeaFight : Object
    {
        // игрок 
        public User User;
        // бот
        public Bot Bot;
        // идет ли игра
        public bool isGame;

        // вывод сообщения
        public string Messages;

        public SeaFight()
        {
            // создаем игрока и бота, и левелы вместе с ними
            User = new User("игрок");
            Bot = new Bot("бот");
            // создаем левел бота, который видет игрок
            Bot.LevelFog.Ships = Bot.Level.Ships;
            Messages = "";
            // начинаем игру
            isGame = true;
            Game();
            ReadKey();
        }
        // цикл игры
        public void Game()
        {
            while (isGame)
            {
                // выводим левелы
                Clear();
                PrintWithColor($"\n{Messages}", ConsoleColor.Black, ConsoleColor.Green);
                Messages = "";

                User.Level.PrintContent();
                Bot.Level.PrintContent();
                Bot.LevelFog.PrintContent();

                // стреляет юзер по боту
                User.UserShoot(Bot, this);
                Bot.BotShoot(User, this);

                CheckWinner();
            }
        }

        // проверка, есть ли победитель
        public void CheckWinner()
        {
            if (
                Bot.Level.CountLiveShips == 0
                && User.Level.CountLiveShips == 0
            )
            {
                Draw(Bot, User);
            }
            else
            {
                if (Bot.Level.CountLiveShips == 0)
                {
                    Victory(User);
                }
                if (User.Level.CountLiveShips == 0)
                {
                    Victory(Bot);
                }
            }
        }
        // победа
        public void Victory(Player winner)
        {
            isGame = false;
            PrintWithColor($"\n\nПобеда за {winner.Name}, у него осталось {winner.Level.CountLiveShips}\n", ConsoleColor.Black, ConsoleColor.Yellow);
        }
        // ничья
        public void Draw(Player drawPlayer1, Player drawPlayer2)
        {
            isGame = false;
            PrintWithColor($"\n\nИгроки: {drawPlayer1.Name} и {drawPlayer2.Name} сыграли вничью\n", ConsoleColor.Black, ConsoleColor.DarkMagenta);
        }
    }
}