class ConsoleApplication
{
    public static void Main()
    {
        foreach (var testArrayVariable in testArray4)
        {
            Task4(testArrayVariable);
        }
        
        Task4();
    }

    // Вводятся четыре числа: A, B, C и D — позиции двух фигур на шахматной доске.
    // Определить, угрожает ли слон, стоящий на клетке A, B фигуре, стоящей на клетке C, D.
    // Учесть, что размер доски 8 × 8 клеток, и что две фигуры на одной клетке стоять не могут.
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
        if (number < 0 || number > 8)
        {
            throw new Exception("Проблема с границами");
        }

        return true;
    }

    // Task3.cs
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

            // используется целочисленное деление!
            bool result = distance / speedDay <= countDay;

            if (result)
            {
                Console.WriteLine("Улитка успеет доползти");
            }
            else
            {
                Console.WriteLine("Улитка не успеет доползти");
            }

            return result;
        }
        catch (FormatException)
        {
            Console.WriteLine("Необходимо число!");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("Скорость не может быть нулевой!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Произошло исключение: " + e.Message);
        }

        return false;
    }

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

    public static int[][] testArray4 =
    {
        [2, 3, -1], // ветка 1
        [4, 0, 2], // ветка 2
        [6, 2, 3], // ветка 3
        [5, 1, 4], // ветка 3
        [-2, 5, -3], // ветка 1
        [1, 0, 5], // ветка 2
        [1, 0, 0], // DivideByZero
        [0, 0, 2] // ветка 2 DivideByZero
    };

    public static int Task4(int[] testVariable = null)
    {
        Console.WriteLine("Task 4");

        try
        {
            int x = 0;
            int a = 0;
            int c = 0;
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
            throw;
        }

        return 0;
    }

    public static double[] testArray6 = [-4.01, -4, -3.99, 0, 3.99, 4, 4.01, -10, -9.99];

    public static void Task6()
    {
        try
        {
            double y = 0;
            foreach (var doubleNumber in testArray6)
            {
                Console.WriteLine("Тестовое значение: " + doubleNumber);
                y = GetValueTask6(doubleNumber);
            }

            Console.WriteLine("Введите значение x");
            double x = Convert.ToDouble(Console.ReadLine());
            y = GetValueTask6(x);
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
            Console.WriteLine("Exception happened" + e.Message);
        }
    }

    public static double GetValueTask6(double x)
    {
        double y = (Math.Sqrt(x + 10) / (Math.Sqrt(16 - x * x)));

        if (double.IsNaN(y) || double.IsInfinity(y))
        {
            Console.WriteLine("Вычисление невозможно");
        }
        else
        {
            Console.WriteLine("y:" + y);
        }

        return y;
    }
}