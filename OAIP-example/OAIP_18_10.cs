using System;

namespace OAIP
{
    internal class OAIP_18_10 {
        public OAIP_18_10(bool isDevoperEdition)
        {
            Console.WriteLine("Hello World!");
            int n = 10;

            int[] numbers = OAIP_Arrays.generateRandomArray(n, -100, 100);

            OAIP_Arrays.WriteArray(numbers, "Рандомный массив");

            // TODO: 1
            Array.Sort(numbers);

            Console.WriteLine(numbers[n - 1] + " - максимальный элемент");

            // TODO: 2
            Array.Reverse(numbers);

            Console.WriteLine("Реверс: ");
            OAIP_Arrays.WriteArray(numbers, "Реверсивный массив");

            // TODO: 3
            Array.Clear(numbers, 0, 3);

            Console.WriteLine("Очистка: ");
            OAIP_Arrays.WriteArray(numbers, "Очищенный массив");

        }
    }
}