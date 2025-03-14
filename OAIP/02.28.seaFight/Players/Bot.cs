namespace OAIP
{
    using static System.Console;

    class Bot : Player
    {
        public Level LevelFog;

        // память 
        public int[] MemoryBot;

        public Bot(string name) : base(name)
        {
            Level = new Level(this, "бот");
            LevelFog = new Level(this, Level, "туман");
            LevelFog.SetFog();

            Level.CountLiveShips += 10;

            MemoryBot = new int[2];
            MemoryBot[0] = 0;
            MemoryBot[1] = 0;
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
                            Level.Ships[shipNumber].LiveDeck--;
                        }
                    }
                }
                LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";
                Level.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";

                return true;
            }
            else
            {
                LevelFog.LevelContent[damagePoint[0], damagePoint[1]] = Level.LevelContent[damagePoint[0], damagePoint[1]];
                return false;
            }
        }

        // выстрел бота
        public void BotShoot(User user, SeaFight game)
        {
            // выбираем точку удара
            // выбираем пока, точка не примет правильное значение
            int[] damagePoint = [0, 0];
            while (
                user.Level.LevelContent[damagePoint[0], damagePoint[1]] != "[.]"
                || user.Level.LevelContent[damagePoint[0], damagePoint[1]] != "[O]"
            )
            {
                WriteLine(user.Level.LevelContent[damagePoint[0], damagePoint[1]]);
                damagePoint = ChoisePointToDamage();

                // TODO: дернуть
                if (user.Level.LevelContent[damagePoint[0], damagePoint[1]] == "[O]"
                // || user.Level.LevelContent[damagePoint[0], damagePoint[1]] == "[.]"
                )
                {
                    break;
                }

                if (user.Level.CheckNeighboardsShootBot(damagePoint))
                {
                    MemoryBot = [0, 0];
                }
            }
            game.Messages += $"Бот выстрелил по [{damagePoint[0] - 1} {damagePoint[1]}]:  ";

            // наносим удар
            if (user.Damage(damagePoint, game))
            {
                game.Messages += $" Корабль подбит!\n";
                // проверка на уничтоженные корабли
                user.Level.CheckDestroyesShips(game.isGame, LevelFog.LevelName);
                // проверка, есть ли победитель
            }
            else
            {
                game.Messages += $" Эхх... Промах!\n";
            }
        }

        public int[] ChoisePointToDamage()
        {
            PrintWithColor($"\nточка памяти: {MemoryBot[0]} : {MemoryBot[1]}\n", ConsoleColor.Black, ConsoleColor.DarkBlue);
            // проверка, что в памяти не пусто
            if (MemoryBot[0] >= 2 && MemoryBot[1] >= 1)
            {
                char direction = SelectDirectionShipForRandom();
                PrintWithColor($"\nнаправление: {direction}\n", ConsoleColor.Black, ConsoleColor.DarkBlue);

                int[] point = [0, 0];
                switch (direction)
                {
                    case 'w':
                        point = [MemoryBot[0], MemoryBot[1] - 1];
                        break;
                    case 's':
                        point = [MemoryBot[0], MemoryBot[1] + 1];
                        break;
                    case 'a':
                        point = [MemoryBot[0] - 1, MemoryBot[1]];
                        break;
                    case 'd':
                        point = [MemoryBot[0] + 1, MemoryBot[1]];
                        break;
                }

                return point;
            }
            return SelectNoseForRandom();
        }
    }
}