using System;
using System.Collections.Generic;
using static System.Console;

namespace OAIP
{
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
                        AddTransaction(ReadLine(), Convert.ToDouble(ReadLine()));
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
                        WriteLine("Введите категорию, для расчета средней траты");
                        GetAverageExpense(ReadLine());
                        Menu();
                        break;
                    case 5:
                        WriteLine("Введите категорию, для прогноза расходов на следующий месяц");
                        PredictNextMonthExpenses(ReadLine());
                        Menu();
                        break;
                    case 6:
                        PrintStatistics();
                        Menu();
                        break;
                    default:
                        PrintWithColor("Ошибка ввода, такого порядкового номера не существует", ConsoleColor.DarkRed, ConsoleColor.Red);
                        break;
                }
            }
            catch (FormatException)
            {
                PrintWithColor("Ошибка ввода, введите число", ConsoleColor.Black, ConsoleColor.Red);
                Menu();
            }
        }

        private void AddTransaction(string str, double num)
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
            PrintWithColor($"Баланс для категории '{category}': {balance}", ConsoleColor.DarkBlue, ConsoleColor.Black);

        }

        private void GetAverageExpense(string category)
        {
            if (!Dictionary.ContainsKey(category))
            {
                WriteLine($"Категория '{category}' не найдена.");
                return;
            }
            List<double> amounts = Dictionary[category];
            double totalExpenses = 0;
            int count = 0;

            foreach (var amount in amounts)
            {
                if (amount < 0) // учитываем только расходы
                {
                    totalExpenses += Math.Abs(amount);
                    count++;
                }
            }

            if (count == 0)
            {
                WriteLine($"Нет расходов в категории '{category}'.");
            }
            else
            {
                double average = totalExpenses / count;
                WriteLine($"Средние траты для категории '{category}': {average}");
            }
        }

        private void PredictNextMonthExpenses(string category)
        {
            if (!Dictionary.ContainsKey(category))
            {
                WriteLine($"Категория '{category}' не найдена.");
                return;
            }
            List<double> amounts = Dictionary[category];
            double totalExpenses = 0;
            int count = 0;

            foreach (var amount in amounts)
            {
                if (amount < 0) // учитываем только расходы
                {
                    totalExpenses += Math.Abs(amount);
                    count++;
                }
            }

            if (count == 0)
            {
                WriteLine($"Нет расходов в категории '{category}'.");
            }
            else
            {
                double average = totalExpenses / count;
                WriteLine($"Прогноз расходов на следующий месяц для категории '{category}': {average}");
            }
        }

        private void PrintStatistics()
        {
            double totalExpenses = 0;
            var categoryExpenses = new Dictionary<string, double>();
            var categoryCount = new Dictionary<string, int>();

            foreach (var entry in Dictionary)
            {
                string category = entry.Key;
                List<double> amounts = entry.Value;

                foreach (var amount in amounts)
                {
                    if (amount < 0) // учитываем только расходы
                    {
                        double expense = Math.Abs(amount);
                        totalExpenses += expense;

                        if (!categoryExpenses.ContainsKey(category))
                        {
                            categoryExpenses[category] = 0;
                            categoryCount[category] = 0;
                        }
                        categoryExpenses[category] += expense;
                        categoryCount[category]++;
                    }
                }
            }

            if (totalExpenses == 0)
            {
                WriteLine("Нет расходов для анализа.");
                return;
            }

            WriteLine($"Общая сумма расходов: {totalExpenses}");

            // Определяем самую затратную категорию
            var mostExpensiveCategory = "";
            var maxExpense = 0.0;
            foreach (var entry in categoryExpenses)
            {
                if (entry.Value > maxExpense)
                {
                    maxExpense = entry.Value;
                    mostExpensiveCategory = entry.Key;
                }
            }
            WriteLine($"Самая затратная категория: {mostExpensiveCategory} с расходами {maxExpense}");

            // Определяем самую частую категорию
            var mostFrequentCategory = "";
            var maxCount = 0;
            foreach (var entry in categoryCount)
            {
                if (entry.Value > maxCount)
                {
                    maxCount = entry.Value;
                    mostFrequentCategory = entry.Key;
                }
            }
            WriteLine($"Самая частая категория: {mostFrequentCategory} с {maxCount} расходами");

            // Расчет процентного соотношения расходов
            WriteLine("Процентное соотношение расходов:");
            foreach (var entry in categoryExpenses)
            {
                double percentage = (entry.Value / totalExpenses) * 100;
                WriteLine($"{entry.Key}: {percentage:F2}%");
            }
        }
    }
}