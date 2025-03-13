namespace OAIP
{
    using static System.Console;

    class User : Player
    {
        public User(string name) : base(name)
        {
            Level = new Level(this, "юзер");

            Level.CountLiveShips += 10;
        }

        // получаем выстрел на карту бота
        public bool Damage(int[] damagePoint, SeaFight game)
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
                Level.LevelContent[damagePoint[0], damagePoint[1]] = "[Ж]";
                // PrintWithColor("\n\nКорабль подбит\n", ConsoleColor.Black, ConsoleColor.DarkRed);

                game.Bot.MemoryBot = new int[2] { damagePoint[0], damagePoint[1] };

                return true;
            }
            else
            {
                Level.LevelContent[damagePoint[0], damagePoint[1]] = "[X]";
                return false;
            }
        }

        // выбор точки выстрела
        public int[] ChoisePointToDamage()
        {
            WriteLine("Введите точку, куда будете стрелять (например: А1 или a1)");
            string stringForUser = ReadLine().ToLower();

            if (stringForUser.Length == 2)
            {
                WriteLine(GetLetterIndexIgnoreCase(stringForUser[0]));
                WriteLine(int.Parse(stringForUser[1].ToString()));

                return [int.Parse(stringForUser[1].ToString()) + 1, GetLetterIndexIgnoreCase(stringForUser[0])];
            }
            else if (stringForUser.Length == 3 || stringForUser[1] + stringForUser[2] == 10)
            {
                return [int.Parse(stringForUser[1] + stringForUser[2].ToString()) + 1, GetLetterIndexIgnoreCase(stringForUser[0])];
            }
            else
            {
                return [0];
            }
        }

        // выстрел игрока
        public void UserShoot(Bot bot, SeaFight game)
        {
            // выбираем точку удара
            int[] damagePoint = ChoisePointToDamage();
            game.Messages += $"Игрок выстрелил по [{damagePoint[0] - 1} {damagePoint[1]}]: ";
            // наносим удар
            if (bot.Damage(damagePoint))
            {
                game.Messages += $" Корабль подбит!\n";
                // проверка на уничтоженные корабли
                bot.LevelFog.CheckDestroyesShips(game.isGame);
                // проверка, есть ли победитель
            }
            else
            {
                game.Messages += $" Эхх... Промах!\n";
            }
        }
    }
}