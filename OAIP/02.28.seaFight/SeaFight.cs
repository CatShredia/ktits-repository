namespace OAIP
{
    using static System.Console;

    class SeaFight : Object
    {
        public SeaFight()
        {
            // строки столбцы
            User user = new User("игрок");

            Bot bot = new Bot("бот");
            bot.LevelFog = new Level(bot.Level);

            Clear();
            user.Level.PrintContent();
            bot.Level.PrintContent();
            bot.LevelFog.PrintContent();

            ReadKey();
        }
    }
}