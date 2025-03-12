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
        public SeaFight()
        {
            // создаем игрока и бота, и левелы вместе с ними
            User = new User("игрок");
            Bot = new Bot("бот");
            // создаем левел бота, который видет игрок
            Bot.LevelFog.Ships = Bot.Level.Ships;
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
                User.Level.PrintContent();
                Bot.Level.PrintContent();
                Bot.LevelFog.PrintContent();
                UserShoot();
            }
        }
        // выстрел игрока
        public void UserShoot()
        {
            // выбираем точку удара
            int[] damagePoint = User.ChoisePointToDamage();
            // наносим удар
            if (Bot.Damage(damagePoint))
            {
                PrintWithColor("\n\nВы попали!\n", ConsoleColor.Black, ConsoleColor.Red);
                // проверка на уничтоженные корабли
                Bot.LevelFog.CheckDestroyesShips(isGame);
                User.Level.CheckDestroyesShips(isGame);
                // проверка, есть ли победитель
                CheckWinner();
            }
            else
            {
                PrintWithColor("\n\nЭхх, промах...\n", ConsoleColor.Black, ConsoleColor.Gray);
            }
        }
        // проверка, есть ли победитель
        public void CheckWinner()
        {
            if (
                Bot.CountLiveShips == 0
                && User.CountLiveShips == 0
            )
            {
                Draw(Bot, User);
            }
            else
            {
                if (Bot.CountLiveShips == 0)
                {
                    Victory(User);
                }
                if (User.CountLiveShips == 0)
                {
                    Victory(Bot);
                }
            }
        }
        // победа
        public void Victory(Player winner)
        {
            isGame = false;
            PrintWithColor($"\n\nПобеда за {winner.Name}, у него осталось {winner.CountLiveShips}\n", ConsoleColor.Black, ConsoleColor.Yellow);
        }
        // ничья
        public void Draw(Player drawPlayer1, Player drawPlayer2)
        {
            isGame = false;
            PrintWithColor($"\n\nИгроки: {drawPlayer1.Name} и {drawPlayer2.Name} сыграли вничью\n", ConsoleColor.Black, ConsoleColor.DarkMagenta);
        }
    }
}