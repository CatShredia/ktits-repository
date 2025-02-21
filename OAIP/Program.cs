// ** ---навигация---
// Программа - main и внутренние инструменты работы с массивами
// OAIP_Arrays - методы и функции для работы с массивами
// OAIP_Files - методы и функции для работы с файлами
// OAIP_да_та - практики

using System;
using System.Collections.Generic;

using static System.Console;

using static OAIP.OAIP_Files;
using static OAIP.OAIP_Arrays;

namespace OAIP
{
    internal class OAIP_Main
    {
        // Переменная для определения, является ли версия разработческой
        public static bool isDevoperEdition = true;
        public const string DEVTASK = "18.02";
        public static bool isRun = false;

        // Список проектов
        public static List<string> projects;

        // Метод для добавления проектов в список
        public static void AddProjects()
        {
            // Инициализация списка проектов
            projects = new List<string>() {
                "\"18.10\"",
                "\"21.11\"",
                "\"28.11\"",
                "\"09.12\"",
                "\"14.01\"",
                "\"21.01:l Файлы Лекция\"",
                "\"21.01:p Файлы 1\"",
                "\"24.01 Файлы 2\"",
                "\"28.01 ПДФ\"",
                "\"31.01 docx (вариант с нерабочей библиотекой)\"",
                "\"31.01:NPOI (вариант с NPOI, БЕСПЛАТНОЙ)\"",
                "\"07.02:l лекция с коллекциями\"",
                "\"07.02:p практика с коллекциями\"",
                "\"14.02 OOP\"",
                "\"18.02 fynans\"",
                "\"Шифр Цезаря\"",
            };
        }

        static void Main(string[] args)
        {
            isRun = true;
            while (isRun)
            {
                Clear();
                // Вывод приветственного сообщения
                PrintHelloDev();
                WriteLine("Какая практика вас интересует? (за какое число: 01.01)");

                // Добавление проектов в список
                AddProjects();

                // Вывод существующих практик
                WriteLine("---");
                foreach (var item in projects)
                {
                    WriteLine(item);
                }
                WriteLine("---");

                // Переменная для хранения даты пользователя
                string date;

                // Если не разработческая версия, считываем ввод пользователя
                if (!isDevoperEdition)
                {
                    Write("Для выхода: 0 - ");
                    date = ReadLine();
                }
                else
                {
                    // В противном случае, используем предустановленное значение
                    date = DEVTASK;
                }

                Menu(date);
            }
        }

        public static void Menu(string date)
        {
            Clear();
            // Выбор практики по введенной дате
            switch (date)
            {
                case "0":
                    isRun = false;
                    break;
                case "18.10":
                    OAIP_18_10 oaip_18_10 = new OAIP_18_10(isDevoperEdition);
                    break;
                case "21.11":
                    OAIP_21_11 oaip_21_11 = new OAIP_21_11(isDevoperEdition);
                    break;
                case "28.11":
                    OAIP_28_11 oaip_28_11 = new OAIP_28_11(isDevoperEdition);
                    break;
                case "09.12":
                    OAIP_09_12 oAIP_09_12 = new OAIP_09_12(isDevoperEdition);
                    break;
                case "14.01":
                    OAIP_14_01 oaip_14_01 = new OAIP_14_01(isDevoperEdition);
                    break;
                case "Шифр Цезаря":
                    Caesar caesar = new Caesar(isDevoperEdition);
                    break;
                case "21.01:l":
                    Files files = new Files(isDevoperEdition);
                    break;
                case "21.01:p":
                    FilesPractise filesPractise = new FilesPractise(isDevoperEdition);
                    break;
                case "24.01":
                    FilePlactise2 filesPractise2 = new FilePlactise2(isDevoperEdition);
                    break;
                case "28.01":
                    PdfPractise pdfPractise = new PdfPractise(isDevoperEdition);
                    break;
                case "31.01":
                    Word word = new Word(isDevoperEdition);
                    break;
                case "31.01:NPOI":
                    NPOI npoi = new NPOI(isDevoperEdition);
                    break;
                case "07.02:l":
                    Collections collections = new Collections(isDevoperEdition);
                    break;
                case "07.02:p":
                    CollectionsPractise collectionsPractise = new CollectionsPractise(isDevoperEdition);
                    break;
                case "14.02":
                    OOP oop = new OOP();
                    break;
                case "18.02":
                    Fynans fynanc = new Fynans(isDevoperEdition);
                    break;
                default:
                    // Сообщение об ошибке, если практика не найдена
                    WriteLine("Нет такой практики");
                    break;
            }
        }

        private static void PrintHelloDev()
        {
            string[] helloDev = new string[]
        {
            "H   H  EEEEE  L      L       OOO      DDDD   EEEEE  V   V",
            "H   H  E      L      L      O   O     D   D  E      V   V",
            "HHHHH  EEEEE  L      L      O   O     D   D  EEEEE  V   V",
            "H   H  E      L      L      O   O     D   D  E      V V V",
            "H   H  EEEEE  LLLLL  LLLLL   OOO      DDDD   EEEEE   V V",
        };

            foreach (var line in helloDev)
            {
                WriteLine(line);
            }

            WriteLine("");
        }
    }
}