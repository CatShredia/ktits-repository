namespace OAIP
{
    using static System.Console;

    class User : Player
    {
        public User(string name) : base(name)
        {
            Level = new Level(this);
        }
    }
}