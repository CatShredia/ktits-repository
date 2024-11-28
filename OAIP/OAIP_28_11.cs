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
        // TODO: конструктор
        public OAIP_28_11(bool isDevoperEdition)
        {
            Console.WriteLine("Выбрана практика 28.11");

            choiseTask();
        }

        // TODO: выбор задачи
        public void choiseTask() {
            Console.WriteLine("Выберите задачу");

            int number = Convert.ToInt32(Console.ReadLine());

            switch (number)
            {
                case 1:
                    task1();
                    break;
                case 2:
                    task2();
                    break;
                case 3:
                    task3();
                    break;
                default:
                    Console.WriteLine("Такой задачи нет");
                    break;
            }
        }
        // TODO: 0 задача
        public void task0() {
            Console.WriteLine("0 задача");
        }
        // TODO: 1 задача
        //10, 15, 20, 25, 30, 35, 40 
        //10, 20, 30, 40
        public void task1() {
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
        public void task2() {
            Console.WriteLine("\"Умножение чисел на их индекс\"");

            List<int> list2 = new List<int>{};

            for (int i = 0; i < 10; i++)
            {   
                list2.Add(i + 1);
            }

            OAIP_Arrays.WriteArray(list2,"numbers");

            for (int i = 0; i < list2.Count; i++)
            {   
                list2[i] = list2[i] * i;
            }

            OAIP_Arrays.WriteArray(list2,"numbers");
        }

        // TODO: 3 задача
        public void task3() {
            Console.WriteLine("\"Сортировка списка и поиск минимального элемента\"");

            List<int> numbers = OAIP_Arrays.generateRandomList(10, -100, 100);

            OAIP_Arrays.WriteArray(numbers,"numbers");
        }
    }
     
}