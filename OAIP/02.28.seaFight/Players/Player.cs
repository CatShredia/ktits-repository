namespace OAIP
{
    using static System.Console;

    class Player : Object
    {
        public string Name;

        // план на генерацию кораблей
        public Dictionary<int, int> CountOfShip = new Dictionary<int, int>
        {
            // сколько палуб в корабле, количество у игрока
            { 1, 4 },
            { 2, 3 },
            { 3, 2 },
            { 4, 1 },
        };

        public Level Level;

        public int CountLiveShips;

        public Player(string name)
        {
            Name = name;
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

        // берем от пользовтеля направление корабля
        public char SelectDirectionShipForRandom()
        {
            Random random = new Random();
            char charRandom;

            int intRandom = random.Next(1, 4);

            switch (intRandom)
            {
                case 1:
                    charRandom = 'w';
                    break;
                case 2:
                    charRandom = 'a';
                    break;
                case 3:
                    charRandom = 's';
                    break;
                case 4:
                    charRandom = 'd';
                    break;
                default:
                    charRandom = 'e';
                    break;
            }

            return charRandom;
        }

        // берем от пользовтеля точку носа корабля
        public int[] SelectNoseForRandom()
        {
            Random random = new Random();

            int[] numbers = [random.Next(1, 12), random.Next(1, 12)];

            return numbers;
        }

    }
}