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

        // получаем выстрел на карту бота
        public bool Damage(int[] damagePoint)
        {
            WriteLine(LevelFog.LevelContent[damagePoint[0], damagePoint[1]]);
            WriteLine(Level.LevelContent[damagePoint[0], damagePoint[1]]);


            if (Level.LevelContent[damagePoint[0], damagePoint[1]].Equals("[O]"))
            {
                LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";
                Level.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";
                if (!CheckShip(damagePoint))
                {
                    PrintWithColor("\n\nКорабль подбит\n", ConsoleColor.Black, ConsoleColor.DarkRed);
                }
                return true;
            }
            else
            {
                LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = Level.LevelContent[damagePoint[0], damagePoint[1]];
                return false;
            }
        }

        // проверяем, остались ли у корабля нетронутые палубы
        public bool CheckShip(int[] point)
        {
            // вверх
            if (Level.LevelContent[point[0] - 1, point[1]] == "[O]")
            {
                return true;
            }
            // низ
            if (Level.LevelContent[point[0] + 1, point[1]] == "[O]")
            {
                return true;
            }
            // право
            if (Level.LevelContent[point[0], point[1] + 1] == "[O]")
            {
                return true;
            }
            // лево
            if (Level.LevelContent[point[0], point[1] - 1] == "[O]")
            {
                return true;
            }

            return false;
        }
    }
}