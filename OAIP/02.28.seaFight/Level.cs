namespace OAIP
{
    using static System.Console;

    class Level : Object
    {
        public string[,] LevelContent;

        public string LevelName;

        private const int LevelLength = 13;

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

        private bool isShipError = false;

        /*
            "[-]" - туман
            "[.]" - пусто
            "[O]" - корабль
        */

        public Level(string name)
        {
            LevelName = name;
            LevelContent = new string[LevelLength , LevelLength];

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
            for (int i = 1; i < LevelLength - 1; i++)
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

            // заполнение последних
            for (int i = 0; i < LevelLength; i++)
            {
                LevelContent[LevelLength - 1, i] = "---";
            }
            for (int i = 0; i < LevelLength; i++)
            {
                if (i != LevelLength - 1 && i != 0 && i != 1)
                {
                    LevelContent[i, LevelLength - 1] = " | ";
                }
            }

            LevelContent[0, 0] = "    ";
            LevelContent[0, LevelLength - 1] = "    ";
            LevelContent[LevelLength - 1, 0] = "  --";
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
            // копируем
            string[,] NewLevelContent = new string[LevelLength, LevelLength];
            Array.Copy(LevelContent, NewLevelContent, LevelContent.Length);

            for (int i = 0; i < deck; i++)
            {
                isShipError = false;

                int[] point;

                switch (direction)
                {
                    case 'w':
                        point = [startLocation[0] - i, startLocation[1]];
                        if (CheckNeighboards([point[0], point[1]]))
                        {
                            NewLevelContent[point[0], point[1]] = "[O]";
                        }
                        else
                        {
                            isShipError = true;
                        }
                        break;
                    case 's':
                        point = [startLocation[0] + i, startLocation[1]];
                        if (CheckNeighboards([point[0], point[1]]))
                        {
                            NewLevelContent[point[0], point[1]] = "[O]";
                        }
                        else
                        {
                            isShipError = true;
                        }
                        break;
                    case 'a':
                        point = [startLocation[0], startLocation[1] - i];
                        if (CheckNeighboards([point[0], point[1]]))
                        {
                            NewLevelContent[point[0], point[1]] = "[O]";
                        }
                        else
                        {
                            isShipError = true;
                        }
                        break;
                    case 'd':
                        point = [startLocation[0], startLocation[1] + i];
                        if (CheckNeighboards([point[0], point[1]]))
                        {
                            NewLevelContent[point[0], point[1]] = "[O]";
                        }
                        else
                        {
                            isShipError = true;
                        }
                        break;
                    default:
                        PrintWithColor("Ошибка", ConsoleColor.Black, ConsoleColor.Red);
                        break;
                }
                if(isShipError) {
                    break;
                }
            }
            if (isShipError)
            {
                PrintWithColor("Невозможное расположение корабля!", ConsoleColor.Black, ConsoleColor.Red);
            }
            else
            {
                LevelContent = NewLevelContent;
            }
            return true;
        }

        // проверка на соседей
        public bool CheckNeighboards(int[] point)
        {
            // строка столбец
            // WriteLine($"{point[0]}, {point[1]}");

            // устанавливаемая точка должна быть [.]
            if (LevelContent[point[0], point[1]] != "[.]")
            {
                return false;
            }

            // вверх
            if (LevelContent[point[0] - 1, point[1]] == "[O]")
            {
                return false;
            }
            // низ
            if (LevelContent[point[0] + 1, point[1]] == "[O]")
            {
                return false;
            }
            // право
            if (LevelContent[point[0], point[1] + 1] == "[O]")
            {
                return false;
            }
            // лево
            if (LevelContent[point[0], point[1] - 1] == "[O]")
            {
                return false;
            }

            return true;
        }

        // печатаем левел
        public void PrintContent()
        {
            ReadKey();
            Clear();
            WriteLine("Карта " + LevelName);

            ConsoleColor[] choisedColor = new ConsoleColor[2];

            for (int i = 0; i < LevelLength; i++)
            {
                for (int j = 0; j < LevelLength; j++)
                {
                    ChoisePrintingColor(LevelContent[i, j]);
                }
                WriteLine();
            }
        }

        // выбор цвета для печати, foreground, background
        public void ChoisePrintingColor(string typeOfShip)
        {
            switch (typeOfShip[1])
            {
                case 'O':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;

                    Write(typeOfShip);
                    SetDefaultColor();
                    break;
                case '.':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Blue;

                    Write(typeOfShip);
                    SetDefaultColor();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;

                    Write(typeOfShip);
                    SetDefaultColor();
                    break;
            }
        }
    }
}