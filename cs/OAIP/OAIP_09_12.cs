namespace OAIP
{
    internal class OAIP_09_12
    {
        public bool isDevoperEdition;
        public OAIP_09_12(bool isDevoperEdition)
        {
            this.isDevoperEdition = isDevoperEdition;


            int choise = 1;
            while (choise != 0)
            {
                Console.WriteLine("Введите нужное задание, для выхода: 0");

                choise = Convert.ToInt32(Console.ReadLine());

                switch (choise)
                {
                    case 0:
                        Console.WriteLine("Выход");
                        break;
                    case 1:
                        task1();
                        break;
                    case 2:
                        task2();
                        break;
                    case 3:
                        task3();
                        break;
                    case 6:
                        Console.WriteLine("Тестовый Char");
                        charTest();
                        break;
                    default:
                        System.Console.WriteLine("Нет такого задания!");
                        break;
                }
            }
        }

        public void charTest()
        {
            char charTestPer = 'k';
            Console.WriteLine(charTestPer);
            Console.WriteLine((int)charTestPer);
        }
        public void task1()
        {
            Console.WriteLine("1 задача");

            Console.WriteLine("Введите символ:");
            char charPer = Console.ReadLine()[0];

            Console.WriteLine(charPer);

            Console.WriteLine(char.IsLetter(charPer) + " буквы"); //букавки
            Console.WriteLine(char.IsDigit(charPer) + " десятичное число"); //десятичное число
            Console.WriteLine(char.IsSymbol(charPer) + " специальный символ");//спец символы
        }

        public void task2()
        {
            Console.WriteLine("2 задача");

            Console.WriteLine("Введите символ:");
            char sumbol = Console.ReadLine()[0];
            Console.WriteLine("Введите строку:");
            string strin = Console.ReadLine();

            Console.WriteLine(strin.IndexOf(sumbol));
        }

        public void task3()
        {
            Console.WriteLine("3 задача");

            Console.WriteLine("Введите строку:");
            string str = Console.ReadLine();

            Console.WriteLine(str.Split(' ').Length);
        }
    }
}