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

        // получаем выстрел на карту
        public void Damage(int[] damagePoint) {
            WriteLine(LevelFog.LevelContent[damagePoint[0], damagePoint[1]]);
            WriteLine(Level.LevelContent[damagePoint[0], damagePoint[1]]);

            LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = Level.LevelContent[damagePoint[0], damagePoint[1]];
        }
    }
}