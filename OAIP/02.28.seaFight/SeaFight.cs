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
                if(Bot.Damage(damagePoint)) {
                    PrintWithColor("\n\nВы попали!\n", ConsoleColor.Black, ConsoleColor.Red);
                } else {
                    PrintWithColor("\n\nЭхх, промах...\n", ConsoleColor.Black, ConsoleColor.Gray);
                }

                // проверка на уничтоженные корабли
                User.Level.CheckDestroyesShips();
            }
        }
    }
}