namespace OAIP
{
    using System.ComponentModel.Design;
    using static System.Console;
    using static OAIP_Arrays;
    class Fynans : Object
    {
        public Dictionary<string, List<double>> Dictionary;

        public bool isDevoperEdition;
        public Fynans(bool isDevoperEditioni)
        {
            isDevoperEdition = isDevoperEditioni;
            PrintWithColor("Программа расчета финанс", ConsoleColor.Black, ConsoleColor.DarkGreen);

            Dictionary = new Dictionary<string, List<double>>();

            if (isDevoperEdition)
            {
                AddTransaction("еда", 14);
                AddTransaction("еда", -14);
                AddTransaction("транспорт", 100);
                AddTransaction("марат казуал", 666);
                AddTransaction("пытки", 142678);
            }

            Menu();
        }

        private void Menu()
        {
            PrintWithColor("Введите номер необходимого метода", ConsoleColor.Black, ConsoleColor.DarkGreen);
            WriteLine("Выход - 0");
            WriteLine("AddTransaction - 1");
            WriteLine("PrintFinanceReport  - 2");
            WriteLine("CalculateBalance  - 3");
            WriteLine("GetAverageExpense   - 4");
            WriteLine("PredictNextMonthExpenses - 5");
            WriteLine("PrintStatistics   - 6");
            try
            {
                int number = Convert.ToInt32(ReadLine());

                switch (number)
                {
                    case 0:
                        break;
                    case 1:
                        WriteLine("Введите категорию и сумму");
                        AddTransaction(ReadLine(), Convert.ToInt32(ReadLine()));
                        Menu();
                        break;
                    case 2:
                        PrintFinanceReport();
                        Menu();
                        break;
                    case 3:
                        WriteLine("Введите категорию, для расчета разницы");
                        CalculateBalance(ReadLine());
                        Menu();
                        break;
                    case 4:
                        WriteLine("Введите категорию, для расчета разницы");
                        CalculateBalance(ReadLine());
                        Menu();
                        break;
                    case 5:
                        
                    default:
                        PrintWithColor("Ошибка ввода, такого порядкового номера не существует", ConsoleColor.DarkRed, ConsoleColor.Red);
                        break;
                }
            }
            catch (FormatException)
            {
                PrintWithColor("Ошибка ввода, введите число", ConsoleColor.Black, ConsoleColor.Red);
                Menu();
                throw;
            }
        }

        private void AddTransaction(string str, int num)
        {
            if (!Dictionary.ContainsKey(str))
            {
                Dictionary[str] = new List<double>();
            }

            Dictionary[str].Add(num);
        }

        private void PrintFinanceReport()
        {
            if (Dictionary.Count == 0)
            {
                PrintWithColor("Словарь пуст. Нет данных для отображения.", ConsoleColor.Black, ConsoleColor.Gray);
                return;
            }

            WriteLine("Финансовый отчет:");
            foreach (var entry in Dictionary)
            {
                string category = entry.Key;
                List<double> amounts = entry.Value;

                PrintWithColor($"Категория: {category}", ConsoleColor.DarkBlue, ConsoleColor.Black);

                WriteLine("Суммы:");

                foreach (var amount in amounts)
                {
                    if (amount <= 0)
                    {
                        WriteLine($"Расход: {amount}");
                    }
                    else
                    {
                        WriteLine($"Доход: {amount}");
                    }
                }

                WriteLine();
            }
        }

        private void CalculateBalance(string category)
        {
            if (!Dictionary.ContainsKey(category))
            {
                WriteLine($"Категория '{category}' не найдена.");
                return;
            }

            List<double> amounts = Dictionary[category];
            double totalExpenses = 0;
            double totalIncome = 0;

            foreach (var amount in amounts)
            {
                if (amount < 0)
                {
                    totalExpenses += Math.Abs(amount);
                }
                else
                {
                    totalIncome += amount;
                }
            }

            double balance = totalIncome - totalExpenses;
            WriteLine($"Баланс для категории '{category}': {balance}");
        }
    }
}