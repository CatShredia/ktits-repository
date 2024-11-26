// TODO: не работает вывод двумерного массива в 21.11

// навигация по файлу происходит текущий образом:
// название класса - OAIP_дата, когда дали задачу
// отдельные методы - отдельные задачи

// существуют и отдельные классы с отдельными методами


using System;
using System.Collections.Generic;

namespace OAIP
{

    // класс с запуском через main
    internal class OAIP
    {
        static void Main(string[] args)
        {
            OAIP_array oaip_array = new OAIP_array();

            Console.WriteLine("Hello World!");
            Console.WriteLine("Какая практика вас интересует? (за какое число: 01.01)");

            // String str = Console.ReadLine();
            string str = "21.11";

            switch (str)
            {
                case "21.11":
                    OAIP_21_11 program = new OAIP_21_11(oaip_array);
                    break;

                default:
                    Console.WriteLine("Нет такой практики");
                    break;
            }


            // Console.ReadKey();
        }
    }

    // класс практики 21_11
    public class OAIP_21_11
    {
        public static int[] temp;
        public static int[,] area;

        public static Dictionary<string, int> menu;
        public static Dictionary<string, int> sales;
        public static Dictionary<string, int> store;

        public static OAIP_array oaip_array;
        public OAIP_21_11(OAIP_array oaip_arrayI)
        {
            //цены
            menu = new Dictionary<string, int>()
            {
                        {"Эспрессо", 100},
                        {"Капучино", 75},
                        {"Латте", 100},
                        {"Флет Уайт", 25},
                        {"Раф", 150}
            };

            //склад
            store = new Dictionary<string, int>()
            {
                        {"Эспрессо", 100},
                        {"Капучино", 1},
                        {"Латте", 55},
                        {"Флет Уайт", 25},
                        {"Раф", 550}
            };
            //продажи
            sales = new Dictionary<string, int>()
            {
                        {"Эспрессо", 0},
                        {"Капучино", 0},
                        {"Латте", 0 },
                        {"Флет Уайт", 0},
                        {"Раф", 0}
            };

            oaip_array = oaip_arrayI;
            Console.WriteLine("Выбрана практика 21.11");
            

            //oaip_array.writeArray(temp, "Температуры этой недели: ");

            Console.WriteLine("Выберите задачу");

            // TODO: поменять
            // int numb = Convert.ToInt32(Console.ReadLine());
            int numb = 2;
            switch (numb)
            {
                case 1:
                    Console.WriteLine("Задание: «Симуляция исследовательской миссии на другую планету»");

                    while (true)
                    {
                        if (menu1() == true)
                        {
                            break;
                        }
                    }
                    break;
                case 2:
                    Console.WriteLine("Задание: \"Менеджер кафе \'Кофейня мечты\'\"");

                    while (true)
                    {
                        if (menu2() == true)
                        {
                            break;
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Такой задачи нет");
                    break;
            }


        }
        public static void warningStore()
        {
            foreach (var item in store)
            {
                if (item.Value < 3)
                {
                    Console.WriteLine($"ВНИМАНИЕ количество {item.Key} равно {item.Value}, необходимо пополнить запасы");
                }
            }
        }
        public static bool menu2()
        {
            // запуск меню
            int number = 0;

            while (true)
            {
                Console.WriteLine("--------------");
                Console.WriteLine("1. Обработка заказа");
                Console.WriteLine("2. Просмотр статистики продаж");
                Console.WriteLine("3. Проверка запасов на складе");
                Console.WriteLine("4. Завершить работу");
                // Console.WriteLine("Введите число от 1 до 4");
                number = Convert.ToInt32(Console.ReadLine());

                if (number > 0 && number < 5)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("КРИТИЧЕСКАЯ ОШИБКА. ПОЖАЛУЙСТА ВВЕДИТЕ ВЕРНОЕ ЗНАЧЕНИЕ!!!");
                }
            }

            //меню
            Console.WriteLine("--------------");
            switch (number)
            {
                case 1:
                    Console.WriteLine("Меню: ");
;                    foreach (var item in menu)
                    {
                        Console.WriteLine($"{item.Key} - {item.Value} руб");
                    }
                    
                    //напиток
                    Console.WriteLine("Введите Напиток");

                    string dring = Console.ReadLine();


                    if (menu.ContainsKey(dring))
                    {
                        Console.WriteLine($"В наличии {store[dring]} порций {dring}");

                        if (store[dring] > 0)
                        {
                            Console.WriteLine("Выполнить заказ? Да/Нет");

                            if (Console.ReadLine().Equals("Да"))
                            {
                                Console.WriteLine("Заказ выполнен");
                                store[dring] -= 1;
                                sales[dring] += 1;

                                warningStore();
                            }
                        } else
                        {
                            Console.WriteLine($"Извините, {dring} закончился.");
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("Извините, у нас пока таких напитков нет");
                    }
                    return false;
                case 2:
                    Console.WriteLine("Продажи на текущий момент: ");
                    foreach (var item in sales)
                    {
                        int menuCost = 0;
                        menu.TryGetValue(item.Key, out menuCost);
                        Console.WriteLine($"{item}. Выручка: {item.Value * menuCost}");
                    }
                    return false;
                case 3:
                    Console.WriteLine("Запасы на складе: ");
                    foreach (var item in store)
                    {
                        Console.WriteLine(item);
                    }
                    return false;
                case 4:
                    Console.WriteLine("Выход из программы");
                    return true;
                default:
                    return false;
            }
        }
        public static bool menu1()
        {
            // запуск меню
            int number = 0;

            while (true)
            {
                Console.WriteLine("--------------");
                Console.WriteLine("1. Показать температуру за последние 7 дней");
                Console.WriteLine("2. Найти среднюю температуру");
                Console.WriteLine("3. Анализировать карту рельефа планеты");
                Console.WriteLine("4. Завершить работу");

                number = Convert.ToInt32(Console.ReadLine());

                if (number > 0 && number < 5)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("КРИТИЧЕСКАЯ ОШИБКА. ПОЖАЛУЙСТА ВВЕДИТЕ ВЕРНОЕ ЗНАЧЕНИЕ!!!");
                }
            }

            //меню
            Console.WriteLine("--------------");
            switch (number)
            {
                case 1:
                    temp = oaip_array.generateRandomArray(7, -100, 100);
                    oaip_array.WriteArray(temp, "Температуры этой недели: ");
                    return false;
                case 2:
                    Console.WriteLine(oaip_array.averageArray(temp, "Температуры этой недели: "));
                    return false;
                case 3:
                    area = oaip_array.generateRandomTwoArray(5, 0, 1000);
                    oaip_array.WriteTwoArray(area, 5, "Рельеф местности: ");

                    oaip_array.AnalizeTwoArray(area, 5);
                    return false;
                case 4:
                    Console.WriteLine("Выход из программы");
                    return true;
                default:
                    return false;
            }

        }
    }

    //генерация рандомного и обычного массивов
    public class OAIP_array
    {

        // генерация массива размером n с элементами от firstLine до secondLine
        internal int[] generateRandomArray(int n, int firstLine, int secondLine)
        {
            Random random = new Random();

            int[] numbers = new int[n];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = random.Next(firstLine, secondLine);
            }

            return numbers;
        }
        internal int[,] generateRandomTwoArray(int n, int firstLine, int secondLine)
        {
            Random random = new Random();

            int[,] numbers = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    numbers[i, j] = random.Next(firstLine, secondLine);
                }
            }

            return numbers;
        }

        // генерация массива размером n с элементами от пользователя
        internal int[] generateArrayWithString(int n)
        {
            int[] numbers = new int[n];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = Convert.ToInt32(Console.ReadLine());
            }

            return numbers;
        }

        // анализ двумерного массива
        internal void AnalizeTwoArray(int[,] array, int n)
        {
            int maks = 0;
            int min = 1000;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (maks < array[i, j])
                    {
                        maks = array[i, j];
                    }

                    if (min > array[i, j])
                    {
                        min = array[i, j];
                    }
                }
            }

            Console.WriteLine("Максимальная высота: " + maks);
            Console.WriteLine("Минимальная высота: " + min);
        }

        //вывод массива
        internal void WriteArray(int[] array, String titleArray)
        {
            Console.WriteLine("Массив " + titleArray);
            foreach (var item in array)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
        internal void WriteTwoArray(int[,] array, int n, String titleArray)
        {
            Console.WriteLine("Массив " + titleArray);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(array[i, j] + " ");
                    if (j == n - 1)
                    {
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine();
        }

        // среднее значение массива
        internal int averageArray(int[] array, String titleArray)
        {
            Console.WriteLine("Среднее значение массива " + titleArray);

            int summ = 0;
            foreach (var item in array)
            {
                summ += item;
            }

            return (summ / array.Length);
        }
    }

}