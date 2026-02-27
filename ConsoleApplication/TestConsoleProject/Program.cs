namespace TestConsoleProject
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Entry point for console application
        }

        // =============== Task 10 ===============
        // Метод, определяющий введена ли строка с разными символами.

        public static string[] testArray10 =
        {
        // пустая, один, все уникальные, повтор, все одинаковые, регистр, null
        "", "a", "abc", "aab", "777", "Aa", null
    };

        public static void Task10()
        {
            foreach (var VARIABLE in testArray10)
            {
                HasUniqueSymbols(VARIABLE);
                Console.WriteLine("----------------");
            }

            HasUniqueSymbols(Console.ReadLine());
        }

        public static bool HasUniqueSymbols(string s)
        {
            Console.Write($"\"{s}\" ");
            if (s == null) return false;

            for (int i = 0; i < s.Length; i++)
            {
                for (int j = i + 1; j < s.Length; j++)
                {
                    if (s[i] == s[j])
                    {
                        Console.WriteLine("FALSE Строка имеет одинаковые символы");
                        return false;
                    }
                }
            }

            Console.WriteLine("TRUE Строка не имеет одинаковых символов");
            return true;
        }

        // =============== Task 11 ===============
        // Метод, определяющий объем сферы

        public static double[][] testArray11 =
        {
        [0, 0],
        [1, 2],
        [3, 1],
        [2.5, 3],
        [-1, 2],
        [1, -1],
        [100, 0]
    };

        public static void Task11()
        {
            try
            {
                foreach (var VARIABLE in testArray11)
                {
                    CalculateSphereVolume(VARIABLE[0], (int)VARIABLE[1]);
                    Console.WriteLine("----------------");
                }

                Console.WriteLine("Введите радиус (double):");
                double radius = Convert.ToDouble(Console.ReadLine());

                Console.WriteLine("Введите точность (int):");
                int precision = Convert.ToInt32(Console.ReadLine());

                CalculateSphereVolume(radius, precision);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Неправильный формат");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static double CalculateSphereVolume(double radius, int precision)
        {
            if (radius < 0)
            {
                Console.WriteLine("Радиус не может быть отрицательным.");
                return 0;
            }

            if (precision < 0)
            {
                Console.WriteLine("Точность должна быть неотрицательной.");
                return 0;
            }

            const double Pi = Math.PI;
            double volume = (4.0 / 3.0) * Pi * Math.Pow(radius, 3);

            Console.WriteLine($"Ответ: {Math.Round(volume, precision)} для {radius} : {precision}");
            return Math.Round(volume, precision);
        }
    }
}