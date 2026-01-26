class Program
{
    public static void Main()
    {
        Console.WriteLine("Выберите задачу: 2, 3, 4, 6 или C4 (классная работа)");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "2": RunTask2(); break;
            case "3": RunTask3(); break;
            case "4": RunTask4(); break;
            case "6": RunTask6(); break;
            case "C4": RunClasswork4(); break;
            default: Console.WriteLine("Неизвестная задача"); break;
        }
    }

    // --- Task 2 ---
    static void RunTask2()
    {
        try
        {
            Console.WriteLine("Введите a, b, c, d (через Enter):");
            int a = ReadInt(); CheckBorders(a);
            int b = ReadInt(); CheckBorders(b);
            int c = ReadInt(); CheckBorders(c);
            int d = ReadInt(); CheckBorders(d);

            bool result = SolveTask2(a, b, c, d);
            Console.WriteLine(result ? "Слон угрожает фигуре." : "Слон не угрожает фигуре.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
        }
    }

    // Чистая логика — без Console!
    public static bool SolveTask2(int a, int b, int c, int d)
    {
        if (a == c && b == d)
            throw new ArgumentException("Фигуры не могут стоять в одной клетке!");
        if (a < 1 || a > 8 || b < 1 || b > 8 || c < 1 || c > 8 || d < 1 || d > 8)
            throw new ArgumentException("Координаты должны быть от 1 до 8");

        return Math.Abs(a - c) == Math.Abs(b - d);
    }

    // --- Task 3 ---
    static void RunTask3()
    {
        try
        {
            Console.Write("Скорость за день (см): "); int speed = ReadInt();
            Console.Write("Расстояние (см): "); int dist = ReadInt();
            Console.Write("Дней: "); int days = ReadInt();

            bool result = SolveTask3(speed, dist, days);
            Console.WriteLine(result ? "Успеет" : "Не успеет");
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
        }
    }

    public static bool SolveTask3(int speedDay, int distance, int countDay)
    {
        if (speedDay <= 0) throw new ArgumentException("Скорость должна быть > 0");
        if (distance < 0 || countDay < 0) throw new ArgumentException("Значения не могут быть отрицательными");
        return distance / speedDay <= countDay; // целочисленное деление
    }

    public static int path = 0;
    
    // --- Task 4 ---
    static void RunTask4()
    {
        // Тесты
        int[][] tests = {
            [2, 3, -1], [4, 0, 2], [6, 2, 3],
            [5, 1, 4], [-2, 5, -3], [1, 0, 5]
        };

        foreach (var t in tests)
        {
            try
            {
                int y = SolveTask4(t[0], t[1], t[2]);
                Console.WriteLine($"[{t[0]}, {t[1]}, {t[2]}] → {y} → {path} ветвь");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{t[0]}, {t[1]}, {t[2]}] → Ошибка: {e.Message}");
            }
        }

        // Интерактивный ввод
        Console.WriteLine("\nВведите x, a, c:");
        int x = ReadInt(), a = ReadInt(), c = ReadInt();
        try
        {
            int y = SolveTask4(x, a, c);
            Console.WriteLine($"Результат: {y}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
        }
    }

    public static int SolveTask4(int x, int a, int c)
    {
        path = 0;
        if (c < 0 && a != 0)
        {
            path = 1;
            return -a * x * x;
        }
        else if (c > 0 && a == 0)
        {
            path = 2;
            if (c * x == 0) throw new DivideByZeroException();
            return (a - x) / (c * x);
        }
        else
        {
            path = 3;
            if (c == 0) throw new DivideByZeroException();
            return x / c;
        }
    }

    // --- Task 6 ---
    static void RunTask6()
    {
        double[] testValues = { -4.01, -4, -3.99, 0, 3.99, 4, 4.01, -10, -9.99 };
        foreach (double x in testValues)
        {
            try
            {
                double y = SolveTask6(x);
                Console.WriteLine($"x={x} → y={y:F4}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"x={x} → Ошибка: {e.Message}");
            }
        }

        Console.Write("\nВведите x: ");
        double input = Convert.ToDouble(Console.ReadLine());
        try
        {
            double y = SolveTask6(input);
            Console.WriteLine($"Результат: {y:F4}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка: " + e.Message);
        }
    }

    public static double SolveTask6(double x)
    {
        if (x + 10 < 0) throw new ArgumentException("x + 10 < 0 → корень из отрицательного");
        if (16 - x * x <= 0) throw new ArgumentException("16 - x² <= 0 → деление на ноль или корень из отриц.");
        return Math.Sqrt(x + 10) / Math.Sqrt(16 - x * x);
    }

    // --- Classwork 4 ---
    static void RunClasswork4()
    {
        Console.Write("Всего вопросов: "); int all = ReadInt();
        Console.Write("На 5: "); int f5 = ReadInt();
        Console.Write("На 4: "); int f4 = ReadInt();
        Console.Write("На 3: "); int f3 = ReadInt();

        bool valid = f3 < f4 && f4 < f5 && f5 <= all && all > 0;
        Console.WriteLine(valid ? "Критерии корректны" : "Критерии некорректны");
    }

    // Вспомогательные методы
    static int ReadInt() => Convert.ToInt32(Console.ReadLine());
    static void CheckBorders(int n)
    {
        if (n < 1 || n > 8) throw new ArgumentException("Должно быть от 1 до 8");
    }
}