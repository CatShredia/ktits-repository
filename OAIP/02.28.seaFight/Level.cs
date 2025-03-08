namespace OAIP
{
    using static System.Console;

    class Level : Object
    {
        public string LevelName;
        public string[,] LevelContent;

        public int LevelLength = 13;

        private bool isShipError = false;

        private Player Player;

        public Ship[] Ships;

        // простой конструктор
        public Level(Player player, string levelName)
        {
            LevelName = levelName;
            LevelContent = new string[LevelLength, LevelLength];

            Ships = new Ship[10];

            Player = player;

            FillLevel();
            PrintContent();
            FillShip();
        }

        // конструктор копирования
        public Level(Player player, Level copyLevel, string levelName)
        {
            LevelName = levelName;
            Player = player;

            LevelLength = copyLevel.LevelLength;
            LevelContent = new string[LevelLength, LevelLength];

            LevelContent = OAIP_Arrays.copy2StringArray(copyLevel.LevelContent);
        }

        // заполняем левел первоначальными значениями
        private void FillLevel()
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
            Dictionary<int, int> countGenerateOfShip = new Dictionary<int, int>(Player.CountOfShip);

            // ? перебираем корабли
            for (int i = countGenerateOfShip.Count; i != 0; i--)
            {
                // ? перебираем корабли одинаковой длины
                while (countGenerateOfShip[i] != 0)
                {
                    // создаем новый объект - корабль
                    int shipNumber = 0;
                    for (int p = 0; p < Ships.Length; p++)
                    {
                        if (Ships[p] == null)
                        {
                            Ships[p] = new Ship();
                            shipNumber = p;
                            break;
                        }
                    }

                    WriteLine($"Установка {i}x палубный корабль");

                    // получаем стартовую точку и направление установки
                    int[] noseLocation;
                    char shipDirection;
                    if (Player.Name.Equals("игрок"))
                    {
                        // TODO: дернуть коммиты во время
                        noseLocation = Player.SelectNoseForRandom();
                        shipDirection = Player.SelectDirectionShipForRandom();

                        // noseLocation = Player.SelectNose();
                        // shipDirection = Player.SelectDirectionShip();
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

                    if (SetShip(noseLocation, shipDirection, i, shipNumber))
                    {
                        PrintWithColor("Корабль установлен!", ConsoleColor.Black, ConsoleColor.Green);

                        countGenerateOfShip[i] -= 1;
                    }
                }
            }
        }

        // ставим корабль
        private bool SetShip(int[] startLocation, char direction, int deck, int shipNumber)
        {
            // копируем левел, для того, чтобы если корабль не установится, легко откатить изменения
            string[,] NewLevelContent = new string[LevelLength, LevelLength];
            Array.Copy(LevelContent, NewLevelContent, LevelContent.Length);

            // ? перебираем палубы корабля
            // TODO: черкануть i
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
                            Ships[shipNumber].Decks.Add(i, point);
                            Ships[shipNumber].LiveDeck++;
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
                            Ships[shipNumber].Decks.Add(i, point);
                            Ships[shipNumber].LiveDeck++;
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
                            Ships[shipNumber].Decks.Add(i, point);
                            Ships[shipNumber].LiveDeck++;
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
                            Ships[shipNumber].Decks.Add(i, point);
                            Ships[shipNumber].LiveDeck++;
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
                // если установка не возможна, останавливаем процесс, без сохранения
                if (isShipError)
                {
                    Ships[shipNumber] = null;
                    break;
                }
            }

            // если ошибка, выходим, иначе сохраняем
            if (isShipError)
            {
                PrintWithColor("Невозможное расположение корабля!", ConsoleColor.Black, ConsoleColor.Red);
                return false;
            }
            else
            {
                LevelContent = NewLevelContent;
                PrintContent();
            }
            return true;
        }

        // проверка на соседей
        private bool CheckNeighboards(int[] point)
        {
            // строка столбец

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
            // левый верхний
            if (LevelContent[point[0] - 1, point[1] - 1] == "[O]")
            {
                PrintWithColor("левый верхний", ConsoleColor.Black, ConsoleColor.DarkYellow);
                return false;
            }
            // левый нижний
            if (LevelContent[point[0] - 1, point[1] + 1] == "[O]")
            {
                PrintWithColor("левый нижний", ConsoleColor.Black, ConsoleColor.DarkYellow);
                return false;
            }
            // правый верхний
            if (LevelContent[point[0] + 1, point[1] - 1] == "[O]")
            {
                PrintWithColor("правый верхний", ConsoleColor.Black, ConsoleColor.DarkYellow);
                return false;
            }
            // правый нижний
            if (LevelContent[point[0] + 1, point[1] + 1] == "[O]")
            {
                PrintWithColor("правый нижний", ConsoleColor.Black, ConsoleColor.DarkYellow);
                return false;
            }

            return true;
        }

        // печатаем левел
        public void PrintContent()
        {
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
        private void ChoisePrintingColor(string typeOfShip)
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
                case '#':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;

                    Write(typeOfShip);
                    SetDefaultColor();
                    break;
                case 'Ж':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Red;

                    Write(typeOfShip);
                    SetDefaultColor();
                    break;
                case 'М':
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.DarkRed;

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

        // установка тумана
        public void SetFog()
        {
            for (int i = 0; i < LevelLength; i++)
            {
                for (int j = 0; j < LevelLength; j++)
                {
                    if (
                        LevelContent[i, j].Equals("[O]")
                        || LevelContent[i, j].Equals("[.]")
                    )
                    {
                        LevelContent[i, j] = "[#]";
                    }
                }
            }
        }

        // проверка на уничтоженные корабли
        public Player CheckDestroyesShips(bool isGame)
        {
            // ? перебираем корабли
            for (int shipNumber = 0; shipNumber < Ships.Length; shipNumber++)
            {
                if (Ships[shipNumber].LiveDeck <= 0)
                {
                    PrintWithColor($"Потоплен {Ships[shipNumber].Decks.Count}x корабль!", ConsoleColor.Black, ConsoleColor.Red);
                    // уменьшаем кол-во кораблей
                    Player.CountLiveShips -= 1;

                    for (int deckNumber = 0; deckNumber < Ships[shipNumber].Decks.Count; deckNumber++)
                    {
                        UnSetFogByPoint([Ships[shipNumber].Decks[deckNumber][0], Ships[shipNumber].Decks[deckNumber][1]]);
                    }

                    if(Player.CountLiveShips <= 0) {
                        isGame = false;

                        return Player;
                    }
                }
            }

            return null;
        }
        public void UnSetFogByPoint(int[] point)
        {
            if (LevelContent[point[0] - 1, point[1]] == "[#]")
            {
                LevelContent[point[0] - 1, point[1]] = "[.]";
            }
            if (LevelContent[point[0] + 1, point[1]] == "[#]")
            {
                LevelContent[point[0] + 1, point[1]] = "[.]";
            }
            if (LevelContent[point[0], point[1] + 1] == "[#]")
            {
                LevelContent[point[0], point[1] + 1] = "[.]";
            }
            if (LevelContent[point[0], point[1] - 1] == "[#]")
            {
                LevelContent[point[0], point[1] - 1] = "[.]";
            }
            if (LevelContent[point[0] - 1, point[1] - 1] == "[#]")
            {
                LevelContent[point[0] - 1, point[1] - 1] = "[.]";
            }
            if (LevelContent[point[0] - 1, point[1] + 1] == "[#]")
            {
                LevelContent[point[0] - 1, point[1] + 1] = "[.]";
            }
            if (LevelContent[point[0] + 1, point[1] - 1] == "[#]")
            {
                LevelContent[point[0] + 1, point[1] - 1] = "[.]";
            }
            if (LevelContent[point[0] + 1, point[1] + 1] == "[#]")
            {
                LevelContent[point[0] + 1, point[1] + 1] = "[.]";
            }
        }
    }
}