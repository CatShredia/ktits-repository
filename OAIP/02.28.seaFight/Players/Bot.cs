namespace OAIP
{
    using static System.Console;

    class Bot : Player
    {
        public Level LevelFog;
        public Bot(string name) : base(name)
        {
            Level = new Level(this, "бот");
            LevelFog = new Level(this, Level, "туман");

            LevelFog.SetFog();
        }

        // получаем выстрел на карту бота
        public bool Damage(int[] damagePoint)
        {
            // проверка на попадание
            if (Level.LevelContent[damagePoint[0], damagePoint[1]].Equals("[O]"))
            {
                // ? перебираем корабли
                for (int shipNumber = 0; shipNumber < Level.Ships.Length; shipNumber++)
                {
                    // ? перебираем палубы
                    for (int deckNumber = 0; deckNumber < Level.Ships[shipNumber].Decks.Count; deckNumber++)
                    {
                        // проверка на соотвествие палуб
                        if (
                            Level.Ships[shipNumber].Decks[deckNumber][0] == damagePoint[0]
                            && Level.Ships[shipNumber].Decks[deckNumber][1] == damagePoint[1]
                        )
                        {
                            WriteLine(shipNumber + " shipnumber--");
                            Level.Ships[shipNumber].LiveDeck--;
                        }
                    }
                }
                LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";
                Level.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";
                // PrintWithColor("\n\nКорабль подбит\n", ConsoleColor.Black, ConsoleColor.DarkRed);
                return true;
            }
            else
            {
                LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = Level.LevelContent[damagePoint[0], damagePoint[1]];
                return false;
            }
        }
    }
}