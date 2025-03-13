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

            Level.CountLiveShips += 10;
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
            int[] damagePoint = ChoisePointToDamage();
            game.Messages += $"Бот выстрелил по [{damagePoint[0] - 1} {damagePoint[1]}]:  ";

            // наносим удар
            if (user.Damage(damagePoint, game))
            {
                game.Messages += $" Корабль подбит!\n";
                // проверка на уничтоженные корабли
                user.Level.CheckDestroyesShips(game.isGame);
                // проверка, есть ли победитель
            }
            else
            {
                game.Messages += $" Эхх... Промах!\n";
            }
        }

        public int[] ChoisePointToDamage()
        {
            return SelectNoseForRandom();
        }
    }
}