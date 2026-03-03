using System;
using System.Text.RegularExpressions;

namespace ConsoleApplication
{
    class ConsoleApplication
    {
        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Выберите задачу: 2, 3, 4, 6, 6_1, 7, 10, 11 или C4");
                string choice = Console.ReadLine();
                // string choice = "7";

                switch (choice)
                {
                    case "2": Task2(); break;
                    case "3": Task3(); break;
                    case "4":
                        foreach (var test in testArray4)
                        {
                            Task4(test);
                        }

                        Task4();
                        break;
                    case "6": Task6(); break;
                    case "6_1": Task6_1(); break;
                    case "C4": Class4(); break;
                    case "7": Task7(); break;
                    default: Console.WriteLine("Неизвестная задача"); break;
                }

                Console.WriteLine(
                    "==========================================================================================");
            }
        }

        // =============== Task 2 ===============
        // Два Слона на шахматной доске
        public static bool Task2()
        {
            try
            {
                Console.WriteLine("Task 2");
                Console.WriteLine("Введите a:");
                int a = Convert.ToInt32(Console.ReadLine());
                CheckBorders(a);

                Console.WriteLine("Введите b:");
                int b = Convert.ToInt32(Console.ReadLine());
                CheckBorders(b);

                Console.WriteLine("Введите c:");
                int c = Convert.ToInt32(Console.ReadLine());
                CheckBorders(c);

                Console.WriteLine("Введите d:");
                int d = Convert.ToInt32(Console.ReadLine());
                CheckBorders(d);

                if (a == c && b == d)
                {
                    throw new Exception("Фигуры не могут стоять в одной клетке!");
                }

                bool isThreatening = Math.Abs(a - c) == Math.Abs(b - d);
                Console.WriteLine(isThreatening ? "Слон угрожает фигуре." : "Слон не угрожает фигуре.");
                return isThreatening;
            }
            catch (FormatException)
            {
                Console.WriteLine("Необходимо число!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошло исключение: " + e.Message);
            }

            return false;
        }

        public static bool CheckBorders(int number)
        {
            if (number < 1 || number > 8)
            {
                throw new Exception("Координата должна быть от 1 до 8");
            }

            return true;
        }

        // =============== Task 3 ===============
        // test_cases улитка ползет
        public static bool Task3()
        {
            try
            {
                Console.WriteLine("Task 3");
                Console.WriteLine("Введите сколько улитка проползает за день (см/день):");
                int speedDay = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Введите расстояние от улитки до цели (см):");
                int distance = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Введите количество дней (день):");
                int countDay = Convert.ToInt32(Console.ReadLine());

                if (speedDay <= 0)
                    throw new DivideByZeroException();

                bool result = distance / speedDay <= countDay;

                if (result)
                    Console.WriteLine("Улитка успеет доползти");
                else
                    Console.WriteLine("Улитка не успеет доползти");

                return result;
            }
            catch (FormatException)
            {
                Console.WriteLine("Необходимо число!");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Скорость не может быть нулевой или отрицательной!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошло исключение: " + e.Message);
            }

            return false;
        }

        // =============== Classwork 4 ===============
        public static void Class4()
        {
            Console.WriteLine("Classwork 4");

            Console.Write("Введите число вопросов теста: ");
            int countAll = int.Parse(Console.ReadLine());
            Console.Write("Введите число правильных ответов на 5: ");
            int count5 = int.Parse(Console.ReadLine());
            Console.Write("Введите число правильных ответов на 4: ");
            int count4 = int.Parse(Console.ReadLine());
            Console.Write("Введите число правильных ответов на 3: ");
            int count3 = int.Parse(Console.ReadLine());

            if (count3 < count4 && count4 < count5 && count5 <= countAll)
                Console.WriteLine("Критерии оценки корректны");
            else
                Console.WriteLine("Критерии оценки некорректны");
        }

        // =============== Task 4 ===============
        // Система уравнений | белый ящик
        public static int[][] testArray4 =
        {
        [2, 3, -1], // ветка 1
        [4, 0, 2], // ветка 2
        [6, 2, 3], // ветка 3
        [5, 1, 4], // ветка 3
        [-2, 5, -3], // ветка 1
        [1, 0, 5], // ветка 2
        [1, 0, 0], // деление на 0 (else)
        [0, 0, 2] // деление на 0 (ветка 2)
    };

        public static int Task4(int[] testVariable = null)
        {
            Console.WriteLine("Task 4");

            try
            {
                int x = 0, a = 0, c = 0;
                if (testVariable != null)
                {
                    x = testVariable[0];
                    a = testVariable[1];
                    c = testVariable[2];
                }
                else
                {
                    Console.WriteLine("Введите x:");
                    x = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите a:");
                    a = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите c:");
                    c = Convert.ToInt32(Console.ReadLine());
                }

                int y = 0;

                if (c < 0 && a != 0)
                {
                    Console.WriteLine($"Ветка 1: -{a} * {x}^2");
                    y = -1 * a * x * x;
                }
                else if (c > 0 && a == 0)
                {
                    Console.WriteLine($"Ветка 2: ({a} - {x}) / ({c} * {x})");
                    y = (a - x) / (c * x);
                }
                else
                {
                    Console.WriteLine($"Ветка 3: {x} / {c}");
                    y = x / c;
                }

                Console.WriteLine($"{x} {a} {c} : {y}");
                Console.WriteLine("--------------------");
                return y;
            }
            catch (FormatException)
            {
                Console.WriteLine("Необходимо число!");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Деление на 0!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }

        // =============== Task 6 ===============
        // Две функции / Черный ящик
        public static double[] testArray6 = { -4.01, -4, -3.99, 0, 3.99, 4, 4.01, -10, -9.99 };

        public static void Task6()
        {
            try
            {
                Console.WriteLine("=== Task 6: y = sqrt(x + 10) / sqrt(16 - x^2) ===");
                foreach (var x in testArray6)
                {
                    Console.WriteLine("Тестовое значение: " + x);
                    GetValueTask6(x);
                }

                Console.WriteLine("Введите значение x");
                double xInput = Convert.ToDouble(Console.ReadLine());
                GetValueTask6(xInput);
            }
            catch (FormatException)
            {
                Console.WriteLine("Необходимо число!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e.Message);
            }
        }

        public static double GetValueTask6(double x)
        {
            double y = Math.Sqrt(x + 10) / Math.Sqrt(16 - x * x);

            if (double.IsNaN(y) || double.IsInfinity(y))
            {
                Console.WriteLine("Вычисление невозможно");
            }
            else
            {
                Console.WriteLine("y: " + y);
            }

            return y;
        }

        // =============== Task 6_1 ===============
        public static double[] testArray6_1 = { -5.1, -5, -4.9, 0, 1, 5, 10 };

        public static void Task6_1()
        {
            try
            {
                Console.WriteLine("=== Task 6.1: y = ln(x + 5) при x >= -5 ===");
                foreach (var x in testArray6_1)
                {
                    Console.WriteLine($"Тестовое значение: {x}");
                    try
                    {
                        double y = GetValueTask6_1(x);
                        Console.WriteLine($"y = {y:F4}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ошибка: {e.Message}");
                    }
                }

                Console.Write("\nВведите значение x: ");
                double input = Convert.ToDouble(Console.ReadLine());
                double result = GetValueTask6_1(input);
                Console.WriteLine($"Результат: y = {result:F4}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Необходимо число!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e.Message);
            }
        }

        public static double GetValueTask6_1(double x)
        {
            if (x < -5)
                throw new ArgumentException("x < -5: логарифм не определён");

            if (x == -5)
                return 0;

            double arg = x + 5;
            if (arg <= 0)
                throw new ArgumentException("Аргумент логарифма должен быть > 0");

            return Math.Log(arg);
        }

        // =============== Task 7 ===============
        // Ввод времени в определенном формате.
        // Вводится строка. Определить, соответствует ли она формату времени суток: **часы и минуты разделены двоеточием, необходимо выполнить проверку на корректность.

        public static string[] testArray7 =
        {
        "-1:30", "99:00", "10:-1", "10:60", "1030", "10:30:45", "", "     ", "ab:cd", "01:01", "1:11", "09:30", "9:5",
        "23:59", "24:00", "10:60"
    };

        public static void Task7()
        {
            try
            {
                foreach (var x in testArray7)
                {
                    Console.WriteLine("Тестовое значение: " + x);
                    GetValueTask7(x);
                }

                string inputString = Console.ReadLine();
                GetValueTask7(inputString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public static void GetValueTask7(string input)
        {
            // format: 24:60
            if (Regex.IsMatch(input, @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$"))
            {
                Console.WriteLine("Время имеет корректный формат (ЧЧ:ММ): " + input);
            }
            else
            {
                Console.WriteLine("Время имеет некорректный формат (ЧЧ:ММ): " + input);
            }
        }


    }
}
