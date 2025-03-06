namespace OAIP
{
    using static System.Console;

    class SeaFight : Object
    {
        public SeaFight(bool isDevelop)
        {
            WriteLine();

            Bot bot = new Bot("бот");
            Level botLevel = new Level(bot);

            User user = new User("игрок");
            Level playerLevel = new Level(user);

        }
    }
}