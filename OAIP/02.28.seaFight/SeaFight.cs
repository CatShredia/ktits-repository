namespace OAIP
{
    using static System.Console;

    class SeaFight : Object
    {
        public SeaFight(bool isDevelop)
        {
            WriteLine(isDevelop);

            User user = new User("игрок");
            Level playerLevel = new Level(isDevelop, user);

            Bot bot = new Bot("бот");
            Level botLevel = new Level(isDevelop, bot);
        }
    }
}