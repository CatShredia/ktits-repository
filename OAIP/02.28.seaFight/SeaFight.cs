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
                int[] damagePoint = User.ChoisePointToDamage();


                Clear();
                Bot.Damage(damagePoint);
                User.Level.PrintContent();
                Bot.LevelFog.PrintContent();
            }
        }
    }
}