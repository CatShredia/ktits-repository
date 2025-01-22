// 

// Program - main и внутренние инструменты работы с массивами
// OAIP_Arrays - методы и функции для работы с массивами
// OAIP_да_та - практики

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAIP
{
    //  
    internal class OAIP_Main
    {
        public static bool isDevoperEdition = true; //переменная для разработчика
        //public static OAIP_Arrays oaip_arrays; //переменная внутренних технологий массивов

        public static List<string> projects;


        // Добавление проектов в проект
        public static void addProjects()
        {
            projects = new List<string>() {
            "\"18.10\"",
            "\"21.11\"",
            "\"28.11\"",
            "\"09.12\"",
            "\"Шифр Цезаря\"",
            "\"14.01\"",
            "\"Работа с файлами\"",
            "\"Работа с файлами: Практика\"",
            "\"null\"   " };
        }

        static void Main(string[] args)
        {
            // Console.Clear();

            Console.WriteLine("Hello World!");
            Console.WriteLine("Какая практика вас интересует? (за какое число: 01.01)");

            addProjects();
            OAIP_Arrays.WriteArray(projects, "Существующие практики: ");

            // 
            string date;
            if (isDevoperEdition == false)
            {
                date = Console.ReadLine();
            }
            else
            {
                date = "Работа с файлами: Практика";
            }

            //выбор даты практики
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
                    Console.WriteLine("Нет такой практики");
                    break;
            }

            Console.ReadKey();

            Console.Clear();
        }
    }
}