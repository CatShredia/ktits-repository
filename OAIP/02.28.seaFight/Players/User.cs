namespace OAIP
{
    using static System.Console;

    class User : Player
    {
        public User(string name) : base(name)
        {
            Level = new Level(this, "юзер");
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
    }
}