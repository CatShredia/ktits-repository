using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OAIP
{
    internal class Collections : Object
    {
        public Collections(bool isDevoperEdition)
        {
            // ? Словарь
            PrintWithColor("Словари", ConsoleColor.Black, ConsoleColor.DarkBlue);
            Dictionary<string,int> ages = new Dictionary<string,int>()
            {
                {"mike" ,80},
                {"qwerty",17}
            };
            // по ключу qqq записываем 55
            ages["qqq"] = 55;
            
            // если содержиться qqq - выводим знач-е
            if (ages.ContainsKey("qqq"))
            {
                Console.WriteLine($"qwerty {ages["qqq"]}");
            }

            // выводим весь
            foreach (var age in ages)
            {
                Console.WriteLine(age);
            }

            // ? Стэк
            PrintWithColor("Стэк", ConsoleColor.Black, ConsoleColor.DarkBlue);
            Stack<int> stack = new Stack<int>();
            // ? Добавляем 10
            stack.Push(10);
            stack.Push(100);

            // ? Выводим последний элемент
            Console.WriteLine(stack.Pop());

            // ? Листы / Динамические массивы
            PrintWithColor("Листы", ConsoleColor.Black, ConsoleColor.DarkBlue);
            List<int> list = new List<int>() { 1,2,3,4,5,6,7,8,9};
            
            // Фильтр 
            var num = list.Where(n => n % 2 == 0);
            Console.WriteLine(string.Join(",", num));

            var gr = list.GroupBy(n => n % 2 == 0 ? "Четные " : "Нечетные");

            // вывод с фильтром
            foreach (var group in gr)
            {
                Console.WriteLine($"{group.Key}: {string.Join(",", group)}");
            }
        }

    }
}