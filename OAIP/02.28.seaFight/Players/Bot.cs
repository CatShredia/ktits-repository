namespace OAIP
{
    using static System.Console;

    class Bot : Player
    {
        public Level LevelFog;
        public Bot(string name) : base(name)
        {
            Level = new Level(this);
            LevelFog = new Level(this, Level);

            LevelFog.SetFog();
        }
    }
}