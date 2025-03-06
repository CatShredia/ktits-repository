namespace OAIP
{
    using static System.Console;

    class SeaFight : Object
    {
        public SeaFight()
        {
            User user = new User("игрок");
            Level playerLevel = new Level(user);

            Bot bot = new Bot("бот");
            Level botLevel = new Level(bot);

            Clear();
            playerLevel.PrintContent();
            botLevel.PrintContent();
            ReadKey();
        }
    }
}