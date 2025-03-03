namespace OAIP
{
    using static System.Console;

    class Level : Object
    {
        public string[,] LevelContent;

        public string LevelName;

        private const int LevelLength = 12;

        /*
            "[-]" - туман
            "[.]" - пусто
            "[O]" - корадль
        */

        public Level(string name)
        {
            LevelName = name;
            LevelContent = new string[LevelLength, LevelLength];

            // заполняем пустотой
            FillLevel();
            // поставитть корабли
            FillShip();
        }

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
                        LevelContent[i, j] = $"___";
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

        public void FillShip()
        {

        }

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