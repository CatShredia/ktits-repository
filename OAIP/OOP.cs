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
            Clear();
            // 1. Сумма двух чисел
            (int sum, int difference, int product, double quotient) results = Calcul(10, 5);

            WriteLine("---------------1 задание---------------");
            WriteLine($"Сумма: {results.sum}, Разность: {results.difference}, Произведение: {results.product}, Частное: {results.quotient}");

            // 2. Минимальное и максимальное значение
            WriteLine("---------------2 задание---------------");
            WriteLine($"Минимум: {Min(5, 10)}, Максимум: {Max(5, 10)}");

            // 3. Проверка на палиндром
            WriteLine("---------------3 задание---------------");
            WriteLine($"\"level\" - палиндром: {IsPalindrome("level")}");
            WriteLine($"\"hello\" - палиндром: {IsPalindrome("hello")}");

            // 4. Факториал числа
            WriteLine("---------------4 задание---------------");
            WriteLine($"Факториал 5: {Factorial(5)}");

            // 5. Среднее арифметическое
            WriteLine("---------------5 задание---------------");
            WriteLine($"Среднее арифметическое 2, 4, 6: {Average(2, 4, 6)}");

            // 6. Поиск символа в строке
            WriteLine("---------------6 задание---------------");
            WriteLine($"Индекс 'e' в \"hello\": {FindChar("hello", 'e')}");
            WriteLine($"Индекс 'a' в \"hello\": {FindChar("hello", 'a')}");

            // 7. Генерация случайного пароля
            WriteLine("---------------7 задание---------------");
            WriteLine($"Случайный пароль длиной 6: {GeneratePassword(6)}");

            // 8. Конвертация температуры
            WriteLine("---------------8 задание---------------");
            WriteLine($"0°C в °F: {CelsiusToFahrenheit(0)}");
            WriteLine($"32°F в °C: {FahrenheitToCelsius(32)}");

            // 9. Перестановка слов в предложении
            WriteLine("---------------9 задание---------------");
            WriteLine($"Перестановка слов в \"Hello world!\": {ReverseWords("Hello world!")}");

            // 10. Таблица умножения
            WriteLine("---------------10 задание---------------");
            WriteLine("Таблица умножения для 5:");
            MultiplicationTable(5);
        }

        // 1. Сумма двух и трёх чисел
        public static (int sum, int difference, int product, double quotient) Calcul(int a, int b)
        {
            return (a + b, a - b, a * b, (double)a / b);
        }

        // 2. Минимальное и максимальное значение
        public static int Min(int a, int b)
        {
            return Math.Min(a, b);
        }

        public static int Max(int a, int b)
        {
            return Math.Max(a, b);
        }

        // 3. Проверка на палиндром
        public static bool IsPalindrome(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;  //Пустая строка тоже палиндром

            string cleanStr = str.ToLower().Where(char.IsLetterOrDigit).ToArray().ToString(); //Remove non alphanumeric

            for (int i = 0; i < str.Length / 2; i++)
            {
                if (str[i] != str[str.Length - i - 1])
                {
                    return false;
                }
            }
            return true;
        }


        // 4. Факториал числа
        public static long Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Число не может быть отрицательным.");
            }
            if (n == 0)
            {
                return 1;
            }

            long result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        // 5. Среднее арифметическое
        public static double Average(int a, int b, int c)
        {
            return (double)(a + b + c) / 3;
        }

        // 6. Поиск символа в строке
        public static int FindChar(string text, char c)
        {
            return text.IndexOf(c);
        }

        // 7. Генерация случайного пароля (только цифры)
        public static string GeneratePassword(int length)
        {
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                password.Append(random.Next(0, 10)); // Цифры от 0 до 9
            }
            return password.ToString();
        }

        // 8. Конвертация температуры
        public static double CelsiusToFahrenheit(double c)
        {
            return c * 9 / 5 + 32;
        }

        public static double FahrenheitToCelsius(double f)
        {
            return (f - 32) * 5 / 9;
        }

        // 9. Перестановка слов в предложении
        public static string ReverseWords(string sentence)
        {
            string[] words = sentence.Split(' ');
            Array.Reverse(words);
            return string.Join(" ", words);
        }

        // 10. Таблица умножения для числа
        public static void MultiplicationTable(int n)
        {
            for (int i = 1; i <= 10; i++)
            {
                WriteLine($"{n} x {i} = {n * i}");
            }
        }


    }
}