//генерация рандомного и обычного массивов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAIP
{
    //методы и функции для работы с массивами
    public class OAIP_Arrays
    {
        // генерация массива размером n с элементами от firstLine до secondLine
        internal static int[] generateRandomArray(int n, int firstLine, int secondLine)
        {
            Random random = new Random();

            int[] numbers = new int[n];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = random.Next(firstLine, secondLine);
            }

            return numbers;
        }
        // генерация листа размером n с элементами от firstLine до secondLine
        internal static List<int> generateRandomList(int n, int firstLine, int secondLine)
        {
            Random random = new Random();

            List<int> numbers = new List<int>{};

            for (int i = 0; i < n; i++)
            {
                numbers.Add(random.Next(firstLine,secondLine));
            }

            return numbers;
        }
        // нахождение элемента в листе
        internal static int getElementList(List<int> numbers, int findsElement) {
            for (int i = 0; i < numbers.Count; i++)
            {
                if(numbers[i] == findsElement) {
                    return i;
                } 
            }
            return -1;
        } 
        // генерация массива размером (n * n) с элементами от firstLine до secondLine
        internal static int[,] generateRandomTwoArray(int n, int firstLine, int secondLine)
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
        internal static int[] generateArrayWithString(int n)
        {
            int[] numbers = new int[n];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = Convert.ToInt32(Console.ReadLine());
            }

            return numbers;
        }

        // анализ двумерного массива (макс и мин)
        internal static void AnalizeTwoArray(int[,] array, int n)
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
        internal static void WriteArray(int[] array, String titleArray)
        {
            Console.WriteLine("Массив " + titleArray);
            foreach (var item in array)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
        internal static void WriteArray(List<string> list, String titleArray)
        {
            Console.WriteLine("Массив " + titleArray);
            foreach (var item in list)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
        internal static void WriteArray(List<int> list, String titleArray)
        {
            Console.WriteLine("Массив " + titleArray);
            foreach (var item in list)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }

        //вывод двумерного массива
        internal static void WriteTwoArray(int[,] array, int n, String titleArray)
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

        // среднее значение одномерного массива
        internal static int averageArray(int[] array, String titleArray)
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

