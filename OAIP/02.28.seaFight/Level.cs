namespace OAIP
{
    using static System.Console;

    class Level : Object
    {
        public string[,] LevelContent;

        private const int LevelLength = 13;

        private bool isShipError = false;

        private Player Player;

        public bool isDevEdition;

        /*
            "[-]" - туман
            "[.]" - пусто
            "[O]" - корабль
        */

        public Level(bool isDevEditionI, Player player)
        {
            LevelContent = new string[LevelLength, LevelLength];

            Player = player;

            FillLevel();
            PrintContent();
            FillShip();

            isDevEdition = isDevEditionI;
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

        // заполняем корабли
        public void FillShip()
        {
            WriteLine("Поставим корабли");

            Dictionary<int, int> countGenerateOfShip = new Dictionary<int, int>(Player.CountOfShip);

            // перебираем корабли
            for (int i = countGenerateOfShip.Count; i != 0; i--)
            {
                while (countGenerateOfShip[i] != 0)
                {
                    WriteLine($"Установка {i}x палубный корабль");

                    int[] noseLocation;
                    char shipDirection;
                    if (Player.Name.Equals("игрок"))
                    {
                        WriteLine(isDevEdition);
                        if (isDevEdition)
                        {
                            noseLocation = Player.SelectNoseForRandom();
                            shipDirection = Player.SelectDirectionShipForRandom();
                        }
                        else
                        {
                            noseLocation = Player.SelectNose();
                            shipDirection = Player.SelectDirectionShip();
                        }
                    }
                    else if (Player.Name.Equals("бот"))
                    {
                        noseLocation = Player.SelectNoseForRandom();
                        shipDirection = Player.SelectDirectionShipForRandom();
                    }
                    else
                    {
                        noseLocation = null;
                        shipDirection = 'w';
                    }

                    if (SetShip(noseLocation, shipDirection, i))
                    {
                        PrintWithColor("Корабль установлен!", ConsoleColor.Black, ConsoleColor.Green);

                        countGenerateOfShip[i] -= 1;
                    }

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
                if (isShipError)
                {
                    break;
                }
            }
            if (isShipError)
            {
                PrintWithColor("Невозможное расположение корабля!", ConsoleColor.Black, ConsoleColor.Red);
                return false;
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
            // Clear();

            PrintWithColor($"Карта {Player.Name}a", ConsoleColor.Black, ConsoleColor.DarkBlue);
            WriteLine();

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