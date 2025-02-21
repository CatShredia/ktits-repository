namespace OAIP
{
    using System;
    using System.Linq;
    using System.Text;
    using static System.Console;

    public class OOP
    {
        public OOP()
        {
            while (true)
            {
                WriteLine("Выберите задание (1-10) или 0 для выхода:");
                WriteLine("1. Сумма двух чисел");
                WriteLine("2. Минимальное и максимальное значения");
                WriteLine("3. Проверка на палиндром");
                WriteLine("4. Факториал числа");
                WriteLine("5. Среднее арифметическое трех чисел");
                WriteLine("6. Поиск символа в строке");
                WriteLine("7. Генерация случайного пароля");
                WriteLine("8. Конвертация температуры");
                WriteLine("9. Перестановка слов в предложении");
                WriteLine("10. Таблица умножения");
                WriteLine("0. Выход");

                if (int.TryParse(ReadLine(), out int choice) && choice >= 0 && choice <= 10)
                {
                    switch (choice)
                    {
                        case 0:
                            return; // Выход из программы
                        case 1:
                            HandleSum();
                            break;
                        case 2:
                            HandleMinMax();
                            break;
                        case 3:
                            HandlePalindrome();
                            break;
                        case 4:
                            HandleFactorial();
                            break;
                        case 5:
                            HandleAverage();
                            break;
                        case 6:
                            HandleFindChar();
                            break;
                        case 7:
                            HandleGeneratePassword();
                            break;
                        case 8:
                            HandleTemperatureConversion();
                            break;
                        case 9:
                            HandleReverseWords();
                            break;
                        case 10:
                            HandleMultiplicationTable();
                            break;
                        default:
                            WriteLine("Неверный вариант. Пожалуйста, попробуйте снова.");
                            break;
                    }
                }
                else
                {
                    WriteLine("Неверный ввод. Пожалуйста, введите число от 0 до 10.");
                }
            }
        }

        private void HandleSum()
        {
            WriteLine("Введите два числа:");
            int a = int.Parse(ReadLine());
            int b = int.Parse(ReadLine());
            var results = Calcul(a, b);
            WriteLine($"Сумма: {results.sum}, Разность: {results.difference}, Произведение: {results.product}, Частное: {results.quotient}");
        }

        private void HandleMinMax()
        {
            WriteLine("Введите два числа:");
            int a = int.Parse(ReadLine());
            int b = int.Parse(ReadLine());
            WriteLine($"Минимум: {Min(a, b)}, Максимум: {Max(a, b)}");
        }

        private void HandlePalindrome()
        {
            WriteLine("Введите строку для проверки на палиндром:");
            string str = ReadLine();
            WriteLine($"\"{str}\" - палиндром: {IsPalindrome(str)}");
        }

        private void HandleFactorial()
        {
            WriteLine("Введите число для вычисления факториала:");
            int n = int.Parse(ReadLine());
            WriteLine($"Факториал {n}: {Factorial(n)}");
        }

        private void HandleAverage()
        {
            WriteLine("Введите три числа:");
            int a = int.Parse(ReadLine());
            int b = int.Parse(ReadLine());
            int c = int.Parse(ReadLine());
            WriteLine($"Среднее арифметическое {a}, {b}, {c}: {Average(a, b, c)}");
        }

        private void HandleFindChar()
        {
            WriteLine("Введите строку:");
            string text = ReadLine();
            WriteLine("Введите символ для поиска:");
            char c = ReadLine()[0];
            WriteLine($"Индекс '{c}' в \"{text}\": {FindChar(text, c)}");
        }

        private void HandleGeneratePassword()
        {
            WriteLine("Введите длину пароля:");
            int length = int.Parse(ReadLine());
            WriteLine($"Случайный пароль длиной {length}: {GeneratePassword(length)}");
        }

        private void HandleTemperatureConversion()
        {
            WriteLine("Выберите конвертацию:");
            WriteLine("1. Цельсий в Фаренгейт");
            WriteLine("2. Фаренгейт в Цельсий");
            int choice = int.Parse(ReadLine());
            if (choice == 1)
            {
                WriteLine("Введите температуру в Цельсиях:");
                double celsius = double.Parse(ReadLine());
                WriteLine($"{celsius}°C = {CelsiusToFahrenheit(celsius)}°F");
            }
            else if (choice == 2)
            {
                WriteLine("Введите температуру в Фаренгейтах:");
                double fahrenheit = double.Parse(ReadLine());
                WriteLine($"{fahrenheit}°F = {FahrenheitToCelsius(fahrenheit)}°C");
            }
        }

        private void HandleReverseWords()
        {
            WriteLine("Введите предложение:");
            string sentence = ReadLine();
            WriteLine($"Перестановка слов: {ReverseWords(sentence)}");
        }

        private void HandleMultiplicationTable()
        {
            WriteLine("Введите число для таблицы умножения:");
            int n = int.Parse(ReadLine());
            WriteLine($"Таблица умножения для {n}:");
            MultiplicationTable(n);
        }

        // Существующие методы...
        public static (int sum, int difference, int product, double quotient) Calcul(int a, int b)
        {
            return (a + b, a - b, a * b, (double)a / b);
        }

        public static int Min(int a, int b) => Math.Min(a, b);
        public static int Max(int a, int b) => Math.Max(a, b);
        
        public static bool IsPalindrome(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            string cleanStr = new string(str.ToLower().Where(char.IsLetterOrDigit).ToArray());
            return cleanStr.SequenceEqual(cleanStr.Reverse());
        }

        public static long Factorial(int n)
        {
            if (n < 0) throw new ArgumentException("Число не может быть отрицательным.");
            return (n == 0) ? 1 : Enumerable.Range(1, n).Aggregate(1L, (acc, x) => acc * x);
        }

        public static double Average(int a, int b, int c) => (double)(a + b + c) / 3;

        public static int FindChar(string text, char c) => text.IndexOf(c);

        public static string GeneratePassword(int length)
        {
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                password.Append(random.Next(0, 10));
            }
            return password.ToString();
        }

        public static double CelsiusToFahrenheit(double c) => c * 9 / 5 + 32;
        public static double FahrenheitToCelsius(double f) => (f - 32) * 5 / 9;

        public static string ReverseWords(string sentence)
        {
            string[] words = sentence.Split(' ');
            Array.Reverse(words);
            return string.Join(" ", words);
        }

        public static void MultiplicationTable(int n)
        {
            for (int i = 1; i <= 10; i++)
            {
                WriteLine($"{n} x {i} = {n * i}");
            }
        }
    }
}