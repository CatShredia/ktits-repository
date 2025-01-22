// ** ---навигация---
// Программа - main и внутренние инструменты работы с массивами
// OAIP_Arrays - методы и функции для работы с массивами
// OAIP_да_та - практики

using System;
using System.Collections.Generic;

namespace OAIP
{
    internal class OAIP_Main
    {
        // Переменная для определения, является ли версия разработческой
        public static bool isDevoperEdition = false; 

        // Список проектов
        public static List<string> projects;

        // Метод для добавления проектов в список
        public static void addProjects()
        {
            // Инициализация списка проектов
            projects = new List<string>() {
                "\"18.10\"",
                "\"21.11\"",
                "\"28.11\"",
                "\"09.12\"",
                "\"Шифр Цезаря\"",
                "\"14.01\"",
                "\"Работа с файлами\"",
                "\"Работа с файлами: Практика\"",
                "\"null\""
            };
        }

        static void Main(string[] args)
        {
            if(!isDevoperEdition) {
                Console.Clear();
            } else {
                Console.WriteLine("Hello World!");
            }

            // Вывод приветственного сообщения
            Console.WriteLine("Какая практика вас интересует? (за какое число: 01.01)");

            // Добавление проектов в список
            addProjects();

            // Вывод существующих практик
            OAIP_Arrays.WriteArray(projects, "Существующие практики: ");

            // Переменная для хранения даты
            string date;

            // Если не разработческая версия, считываем ввод пользователя
            if (!isDevoperEdition)
            {
                date = Console.ReadLine();
            }
            else
            {
                // В противном случае, используем предустановленное значение
                date = "Работа с файлами: Практика";
            }

            // Выбор практики по введенной дате
            switch (date)
            {
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
                case "Работа с файлами":
                    Files files = new Files(isDevoperEdition);
                    break;
                case "Работа с файлами: Практика":
                    FilesPractise filesPractise = new FilesPractise(isDevoperEdition);
                    break;
                default:
                    // Сообщение об ошибке, если практика не найдена
                    Console.WriteLine("Нет такой практики");
                    break;
            }

            Console.Clear();
        }
    }
}