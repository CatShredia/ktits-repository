using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAIP
{
    // класс практики 28_11
    class OAIP_28_11
    {
        public static bool isDevoperEdition;
        // TODO: конструктор
        public OAIP_28_11(bool isDevoperEditioni)
        {
            isDevoperEdition = isDevoperEditioni;
            Console.WriteLine("Выбрана практика 28.11");

            while (true)
            {
                if (choiseTask() == true)
                {
                    Console.WriteLine("Выход из программы");
                    break;
                }
            }
        }

        // TODO: выбор задачи
        public bool choiseTask()
        {
            Console.WriteLine("Выберите задачу. Для выхода: 0");


            int number = Convert.ToInt32(Console.ReadLine());

            switch (number)
            {
                case 0:
                    return true;
                case 1:
                    task1();
                    return false;
                case 2:
                    task2();
                    return false;
                case 3:
                    task3();
                    return false;
                case 4:
                    task4();
                    return false;
                case 5:
                    task5();
                    return false;
                default:
                    Console.WriteLine("Такой задачи нет");
                    return false;
            }
        }
        // TODO: 0 задача
        public void task0()
        {
            Console.WriteLine("0 задача");
        }
        // TODO: 1 задача
        //10, 15, 20, 25, 30, 35, 40 
        //10, 20, 30, 40
        public void task1()
        {
            Console.WriteLine("\"Удаление нечётных чисел\"");

            List<int> list1 = new List<int>{
                10, 15, 20, 25, 30, 35, 40
            };

            OAIP_Arrays.WriteArray(list1, "numbers");

            list1.RemoveAt(1);
            list1.RemoveAt(2);
            list1.RemoveAt(3);

            OAIP_Arrays.WriteArray(list1, "numbers");
        }
        // TODO: 2 задача
        public void task2()
        {
            Console.WriteLine("\"Умножение чисел на их индекс\"");

            List<int> list2 = new List<int> { };

            for (int i = 0; i < 10; i++)
            {
                list2.Add(i + 1);
            }

            OAIP_Arrays.WriteArray(list2, "numbers");

            for (int i = 0; i < list2.Count; i++)
            {
                list2[i] = list2[i] * i;
            }

            OAIP_Arrays.WriteArray(list2, "numbers");
        }

        // TODO: 3 задача
        public void task3()
        {
            Console.WriteLine("\"Сортировка списка и поиск минимального элемента\"");

            List<int> numbers = OAIP_Arrays.generateRandomList(10, -100, 100);

            OAIP_Arrays.WriteArray(numbers, "Лист до Сортировки");

            numbers.Sort();

            OAIP_Arrays.WriteArray(numbers, "Лист после Сортировки");

            Console.WriteLine("Минимальный элемент: " + numbers[0]);
        }

        // TODO: 4 задача
        public void task4()
        {
            Console.WriteLine("\"Поиск повторяющихся элементов\"");

            List<int> numbers = OAIP_Arrays.generateRandomList(10, 0, 10);
            List<int> answer = new List<int>();

            OAIP_Arrays.WriteArray(numbers, "Лист");

            for (int i = 0; i < numbers.Count; i++)
            {

                for (int j = 0; j < numbers.Count; j++)
                {
                    if (numbers[i] == numbers[j])
                    {
                        if (i != j)
                        {
                            if (OAIP_Arrays.getElementList(answer, numbers[i]) == -1)
                            {
                                answer.Add(numbers[i]);
                            }
                        }
                    }
                }
            }

            answer.Sort();
            OAIP_Arrays.WriteArray(answer, "Ответ");
        }
        // TODO: 5 задача
        public void task5()
        {
            Console.WriteLine("\"Сумма элементов между минимальным и максимальным\"");

            int summ = 0;

            List<int> numbers = OAIP_Arrays.generateRandomList(10, 0, 10);

            List<int> numbersSort = new List<int>();

            // копирование листа
            for (int i = 0; i < numbers.Count; i++)
            {
                numbersSort.Add(numbers[i]);
            }

            numbersSort.Sort();
            int minIndex = OAIP_Arrays.getElementList(numbers, numbersSort[0]);
            int maxIndex = OAIP_Arrays.getElementList(numbers, numbersSort[numbersSort.Count - 1]);

            OAIP_Arrays.WriteArray(numbers, "Лист");

            if (minIndex > maxIndex)
            {
                int p = maxIndex;
                maxIndex = minIndex;
                minIndex = p;
            }
            // сумма
            for (int i = minIndex; i < maxIndex + 1; i++)
            {
                summ += numbers[i];
            }

            // Console.WriteLine($"max - {maxIndex} min - {minIndex}");
            Console.WriteLine("Сумма: " + summ);
        }
    }

}