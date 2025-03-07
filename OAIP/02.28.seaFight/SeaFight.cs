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
            User = new User("игрок");

            Bot = new Bot("бот");

            Clear();
            User.Level.PrintContent();
            Bot.LevelFog.PrintContent();

            isGame = true;
            Game();

            ReadKey();
        }

        public void Game()
        {
            while (isGame)
            {
                // Clear();
                User.Level.PrintContent();
                Bot.Level.PrintContent();
                Bot.LevelFog.PrintContent();

                int[] damagePoint = User.ChoisePointToDamage();
                if(Bot.Damage(damagePoint)) {
                    PrintWithColor("\n\nВы попали!\n", ConsoleColor.Black, ConsoleColor.Red);
                } else {
                    PrintWithColor("\n\nЭхх, промах...\n", ConsoleColor.Black, ConsoleColor.Gray);
                }
            }
        }
    }
}