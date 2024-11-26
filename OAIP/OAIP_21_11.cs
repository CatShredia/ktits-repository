using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAIP
{
    // класс практики 21_11
    public class OAIP_21_11
    {
        //1 задача
        public static int[] temp;
        public static int[,] area;
        //2 задача
        public static Dictionary<string, int> menu; //цены
        public static Dictionary<string, int> sales;//склад
        public static Dictionary<string, int> store;//продажи

        //public static OAIP_Arrays oaip_array;

        //TODO: конструктор
        public OAIP_21_11(bool isDevoperEdition)
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

            Console.WriteLine("Выбрана практика 21.11");

            Console.WriteLine("Выберите задачу");


            //TODO: разраб
            int numb;
            if (isDevoperEdition == false)
            {
                numb = Convert.ToInt32(Console.ReadLine());
            } else
            {
                numb = 2;
            }

            //выбор задачи
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
        
        //2 задача
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
                    ; foreach (var item in menu)
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
                        }
                        else
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
                    temp = OAIP_Arrays.generateRandomArray(7, -100, 100);
                    OAIP_Arrays.WriteArray(temp, "Температуры этой недели: ");
                    return false;
                case 2:
                    Console.WriteLine(OAIP_Arrays.averageArray(temp, "Температуры этой недели: "));
                    return false;
                case 3:
                    area = OAIP_Arrays.generateRandomTwoArray(5, 0, 1000);
                    OAIP_Arrays.WriteTwoArray(area, 5, "Рельеф местности: ");

                    OAIP_Arrays.AnalizeTwoArray(area, 5);
                    return false;
                case 4:
                    Console.WriteLine("Выход из программы");
                    return true;
                default:
                    return false;
            }

        }
    }
}
