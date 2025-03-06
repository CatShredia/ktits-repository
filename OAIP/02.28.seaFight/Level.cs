namespace OAIP
{
    using static System.Console;

    class Level : Object
    {
        public string[,] LevelContent;

        public string LevelName;

        private const int LevelLength = 12;

        public Dictionary<string, int> TypeOfShip = new Dictionary<string, int>
        {
            { "4x", 4 },
            { "3x", 3 },
            { "2x", 2 },
            { "1x", 1 }
        };

        public Dictionary<string, int> CountOfShip = new Dictionary<string, int>
        {
            { "4x", 1 },
            { "3x", 2 },
            { "2x", 3 },
            { "1x", 4 }
        };

        /*
            "[-]" - туман
            "[.]" - пусто
            "[O]" - корабль
        */

        public Level(string name)
        {
            LevelName = name;
            LevelContent = new string[LevelLength, LevelLength];

            FillLevel();
            PrintContent();
            FillShip();

        }

        // заполняем левел первоначальными значениями
        public void FillLevel()
        {
            // заполнение изначальных
            for (int i = 0; i < LevelLength; i++)
            {
                for (int j = 0; j < LevelLength; j++)
                {
                    LevelContent[i, j] = $"[.]";

                    if (i == 1)
                    {
                        LevelContent[i, j] = $"---";
                        if (j == 0)
                        {
                            LevelContent[i, j] = $"   ";
                        }
                    }
                }
            }

            // заполнение букв
            char numChar = 'A';
            for (int i = 1; i < LevelLength; i++)
            {
                LevelContent[0, i] = " " + numChar.ToString() + " ";

                numChar++;
            }

            // заполнение цифр
            int numInt = 1;
            for (int i = 2; i < LevelLength; i++)
            {
                if (numInt == 10)
                {
                    LevelContent[i, 0] = "" + numInt.ToString() + "| ";
                }
                else
                {
                    LevelContent[i, 0] = " " + numInt.ToString() + "| ";
                }

                numInt++;
            }

            LevelContent[0, 0] = "    ";
        }

        // берем от пользовтеля направление корабля
        public char SelectDirectionShip()
        {
            while (true)
            {
                WriteLine("Выберите направление (wasd)");
                char charFromUser = ReadLine().ToLower()[0];

                if (charFromUser == 'w' || charFromUser == 'a' || charFromUser == 's' || charFromUser == 'd')
                {
                    return charFromUser;
                }
                else
                {
                    WriteLine("введи wasd!");
                }
            }
        }

        // берем от пользовтеля точку носа корабля
        public int[] SelectNose()
        {
            WriteLine("Введите точку, где будет нос корабля (например: А1 или a1)");
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

        // заполняем корабли
        public void FillShip()
        {
            WriteLine("Поставим корабли");

            while (true)
            {
                // перебираем корабли
                foreach (var item in TypeOfShip)
                {
                    WriteLine($"Устанавливание {item.Key} палубный корабль");
                    int[] noseLocation = SelectNose();
                    char shipDirection = SelectDirectionShip();

                    SetShip(noseLocation, shipDirection, item.Value);

                    PrintContent();
                }
            }
        }

        // ставим корабль
        public bool SetShip(int[] startLocation, char direction, int deck)
        {
            WriteLine("Точка: " + startLocation[0] + " " + startLocation[1]);
            WriteLine("Направление: " + direction);

            // копируем
            string[,] NewLevelContent = new string[LevelLength, LevelLength];
            Array.Copy(LevelContent, NewLevelContent, LevelContent.Length);

            for (int i = 0; i < deck; i++)
            {
                WriteLine("Палуба: " + i);

                switch (direction)
                {
                    case 'w':
                        if (NewLevelContent[startLocation[0] - i, startLocation[1]] != "[.]")
                        {
                            PrintWithColor("Этот корабль сюда не установится!", ConsoleColor.Black, ConsoleColor.Red);
                            return false;
                        }
                        else
                        {
                            NewLevelContent[startLocation[0] - i, startLocation[1]] = "[O]";
                        }
                        break;
                    case 'a':
                        if (NewLevelContent[startLocation[0], startLocation[1] - i] != "[.]")
                        {
                            PrintWithColor("Этот корабль сюда не установится!", ConsoleColor.Black, ConsoleColor.Red);
                            return false;
                        }
                        else
                        {
                            NewLevelContent[startLocation[0], startLocation[1] - i] = "[O]";
                        }
                        break;
                    case 's':
                        if (NewLevelContent[startLocation[0] + i, startLocation[1]] != "[.]")
                        {
                            PrintWithColor("Этот корабль сюда не установится!", ConsoleColor.Black, ConsoleColor.Red);
                            return false;
                        }
                        else
                        {
                            NewLevelContent[startLocation[0] + i, startLocation[1]] = "[O]";
                        }
                        break;
                    case 'd':
                        if (NewLevelContent[startLocation[0], startLocation[1] + i] != "[.]")
                        {
                            PrintWithColor("Этот корабль сюда не установится!", ConsoleColor.Black, ConsoleColor.Red);
                            return false;
                        }
                        else
                        {
                            NewLevelContent[startLocation[0], startLocation[1] + i] = "[O]";
                        }
                        break;
                    default:
                        PrintWithColor("Ошибка", ConsoleColor.Black, ConsoleColor.Red);
                        break;
                }

            }
            LevelContent = NewLevelContent;
            return true;
        }

        // проверяем есть ли в соседях корабли
        public bool CheckNeignboards(int[] location)
        {
            // верх
            if (LevelContent[(location[0] - 1), location[1]] == "[O]")
            {
                return false;
            }
            // низ
            if (LevelContent[(location[0] + 1), location[1]] == "[O]")
            {
                return false;
            }
            // право
            if (LevelContent[(location[0]), location[1] - 1] == "[O]")
            {
                return false;
            }
            // лево
            if (LevelContent[location[0], location[1] + 1] == "[O]")
            {
                return false;
            }
            return true;
        }

        // печатаем левел
        public void PrintContent()
        {
            WriteLine("Карта " + LevelName);

            for (int i = 0; i < LevelLength; i++)
            {
                for (int j = 0; j < LevelLength; j++)
                {
                    Write(LevelContent[i, j]);
                }
                WriteLine();
            }
        }
    }
}