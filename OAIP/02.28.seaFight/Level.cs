namespace OAIP
{
    using static System.Console;

    class Level : Object
    {
        // название левела
        public string LevelName;
        // содержимое левела
        public string[,] LevelContent;
        // кол-во неуничтоженных кораблей
        public int CountLiveShips;

        // кол-во строк и столбцов
        // TODO: необходимо переписать код генерации, для того, чтобы в контенте был только контент
        public int LevelLength = 13;

        // игрок, владелец
        private Player Player;

        // корабли
        public Ship[] Ships;

        // простой конструктор
        public Level(Player player, string levelName)
        {
            LevelName = levelName;
            LevelContent = new string[LevelLength, LevelLength];

            Ships = new Ship[10];

            Player = player;

            FillLevel();
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
            // ? заполнение изначальных
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

            // ? заполнение букв
            char numChar = 'A';
            for (int i = 1; i < LevelLength - 1; i++)
            {
                LevelContent[0, i] = " " + numChar.ToString() + " ";

                numChar++;
            }

            // ? заполнение цифр
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

            // ? заполнение последних
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

            // ? базовое обнуление
            LevelContent[0, 0] = "    ";
            LevelContent[0, LevelLength - 1] = "    ";
            LevelContent[LevelLength - 1, 0] = "  --";
        }

        // заполняем корабли
        public void FillShip()
        {
            // временный словарь, для подсчета кол-ва сгенерированных кораблей
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
            bool isShipError = false;
            for (int deckI = 0; deckI < deck; deckI++)
            {
                isShipError = false;
                int[] point;

                switch (direction)
                {
                    case 'w':
                        point = [startLocation[0] - deckI, startLocation[1]];
                        isShipError = SetPointOfShip(point, shipNumber, NewLevelContent, deckI);
                        break;
                    case 's':
                        point = [startLocation[0] + deckI, startLocation[1]];
                        isShipError = SetPointOfShip(point, shipNumber, NewLevelContent, deckI);
                        break;
                    case 'a':
                        point = [startLocation[0], startLocation[1] - deckI];
                        isShipError = SetPointOfShip(point, shipNumber, NewLevelContent, deckI);
                        break;
                    case 'd':
                        point = [startLocation[0], startLocation[1] + deckI];
                        isShipError = SetPointOfShip(point, shipNumber, NewLevelContent, deckI);
                        break;
                    default:
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
                return false;
            }
            else
            {
                LevelContent = NewLevelContent;
                PrintContent();
            }
            return true;
        }

        // установка точки в временный массив
        private bool SetPointOfShip(int[] point, int shipNumber, string[,] NewLevelContent, int deckI)
        {
            if (CheckNeighboards([point[0], point[1]]))
            {
                NewLevelContent[point[0], point[1]] = "[O]";
                Ships[shipNumber].Decks.Add(deckI, point);
                Ships[shipNumber].LiveDeck++;

                return false;
            }
            else
            {
                return true;
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

        // проверка на соседей
        private bool CheckNeighboards(int[] point)
        {
            try
            {
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
                    return false;
                }
                // левый нижний
                if (LevelContent[point[0] - 1, point[1] + 1] == "[O]")
                {
                    return false;
                }
                // правый верхний
                if (LevelContent[point[0] + 1, point[1] - 1] == "[O]")
                {
                    return false;
                }
                // правый нижний
                if (LevelContent[point[0] + 1, point[1] + 1] == "[O]")
                {
                    return false;
                }

            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            return true;
        }

        // проверка на соседей
        public bool CheckNeighboardsShootBot(int[] point)
        {
            try
            {
                // вверх
                if (LevelContent[point[0] - 1, point[1]] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0] + 1, point[1]] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0], point[1] + 1] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0], point[1] - 1] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0] - 1, point[1] - 1] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0] - 1, point[1] + 1] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0] + 1, point[1] - 1] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                    && LevelContent[point[0] + 1, point[1] + 1] == "[X]" || LevelContent[point[0] + 1, point[1]] == "[Ж]"
                )
                {
                    return true;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
            return false;
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
                case 'X':
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;

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

        // проверка на уничтоженные корабли
        public void CheckDestroyesShips(bool isGame, string LevelName)
        {
            // ? перебираем корабли
            for (int shipNumber = 0; shipNumber < Ships.Length; shipNumber++)
            {
                if (Ships[shipNumber].LiveDeck == 0)
                {
                    Player.Level.CountLiveShips -= 1;

                    for (int deckNumber = 0; deckNumber < Ships[shipNumber].Decks.Count; deckNumber++)
                    {
                        WriteLine(LevelName);
                        if (LevelName.Equals("туман"))
                        {
                            UnSetFogByPointBot([Ships[shipNumber].Decks[deckNumber][0], Ships[shipNumber].Decks[deckNumber][1]]);
                        }
                        else if (LevelName.Equals("юзер"))
                        {
                            UnSetFogByPoint([Ships[shipNumber].Decks[deckNumber][0], Ships[shipNumber].Decks[deckNumber][1]]);

                        }

                        Ships[shipNumber].LiveDeck = -1;
                    }
                }
            }
        }

        // удаляем точки, вблизи заданной
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
        // удаляем точки, вблизи заданной
        public void UnSetFogByPointBot(int[] point)
        {
            if (LevelContent[point[0] - 1, point[1]] == "[.]")
            {
                LevelContent[point[0] - 1, point[1]] = "[X]";
            }
            if (LevelContent[point[0] + 1, point[1]] == "[.]")
            {
                LevelContent[point[0] + 1, point[1]] = "[X]";
            }
            if (LevelContent[point[0], point[1] + 1] == "[.]")
            {
                LevelContent[point[0], point[1] + 1] = "[X]";
            }
            if (LevelContent[point[0], point[1] - 1] == "[.]")
            {
                LevelContent[point[0], point[1] - 1] = "[X]";
            }
            if (LevelContent[point[0] - 1, point[1] - 1] == "[.]")
            {
                LevelContent[point[0] - 1, point[1] - 1] = "[X]";
            }
            if (LevelContent[point[0] - 1, point[1] + 1] == "[.]")
            {
                LevelContent[point[0] - 1, point[1] + 1] = "[X]";
            }
            if (LevelContent[point[0] + 1, point[1] - 1] == "[.]")
            {
                LevelContent[point[0] + 1, point[1] - 1] = "[X]";
            }
            if (LevelContent[point[0] + 1, point[1] + 1] == "[.]")
            {
                LevelContent[point[0] + 1, point[1] + 1] = "[X]";
            }
        }
    }
}
