using System.Runtime.CompilerServices;

namespace OAIP
{
    class OAIP_14_01
    {
        private bool isDevoperEdition;

        public OAIP_14_01(bool isDevoperEdition)
        {
            this.isDevoperEdition = isDevoperEdition;

            Console.WriteLine("---Обработка исключений---");

            menu();
        }

        public static void menu()
        {
            Console.WriteLine("Введите номер исключения (для выхода - 0):");
            Console.WriteLine("FormatException \nInvalidOperationException \nOverflowException");

            int number = 0;

            try
            {
                number = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Вы ввели не верное выражение");

                menu();
            }

            switch (number)
            {
                case 0:
                    break;
                case 1:
                    task1();
                    break;
                case 3:
                    task3();
                    break;
                default:
                    Console.WriteLine("Такого исключения нет");
                    menu();
                    break;
            }
        }
        public static void task1()
        {
            try
            {
                Console.WriteLine("Попробуйте ввести строку, число");
                int number = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException e)
            {
                Console.WriteLine("Вы ввели не верное выражение" + " " + e.Message);

                task1();
            }

            menu();
        }
        public static void task3()
        {
            try
            {
                int maxValue = int.MaxValue;
                

                // Используем checked для бросания исключения OverflowException
                checked
                {
                    int resultChecked = maxValue + 1; // Выбросит OverflowException
                    Console.WriteLine($"Результат после переполнения (checked): {resultChecked}"); // Эта строка не выполнится
                }
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"Поймано исключение: {ex.Message}");
            }

            menu();
        }
    }
}