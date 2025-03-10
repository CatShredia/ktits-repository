namespace OAIP
{
    using static System.Console;

    class SeaFight : Object
    {
        public User User;
        public Bot Bot;

        public bool isGame;

        public SeaFight()
        {
            // строки столбцы

            // создаем игрока и бота, и левелы вместе с ними
            User = new User("игрок");
            Bot = new Bot("бот");
            Bot.LevelFog.Ships = Bot.Level.Ships;

            // Clear();

            // выводим левелы уже с туманом
            User.Level.PrintContent();
            Bot.LevelFog.PrintContent();

            // начинаем игру
            isGame = true;
            Game();

            ReadKey();
        }

        public void Game()
        {
            while (isGame)
            {
                // выводим левелы
                // Clear();
                User.Level.PrintContent();
                Bot.Level.PrintContent();
                Bot.LevelFog.PrintContent();

                // выбираем точку удара
                int[] damagePoint = User.ChoisePointToDamage();
                // наносим удар
                if (Bot.Damage(damagePoint))
                {
                    PrintWithColor("\n\nВы попали!\n", ConsoleColor.Black, ConsoleColor.Red);
                    // проверка на уничтоженные корабли
                    Bot.LevelFog.CheckDestroyesShips(isGame);
                    User.Level.CheckDestroyesShips(isGame);

                    CheckWinner();
                }
                else
                {
                    PrintWithColor("\n\nЭхх, промах...\n", ConsoleColor.Black, ConsoleColor.Gray);
                }
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