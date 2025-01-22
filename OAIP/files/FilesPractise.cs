namespace OAIP
{
    using System.Text.RegularExpressions;

    class FilesPractise
    {
        public static string fio = "Evimov Aleksandr" + "\n";
        public static string filePath = "";
        public static string path = "";

        public const string OLDWORD = "старое";
        public const string NEWWORD = "новое";

        public FilesPractise(bool isDevoperEdition)
        {
            Console.WriteLine("---Практика с файлами---");
            Console.WriteLine("21.01.2025");

            path = Directory.GetCurrentDirectory() + "\\files\\texts";

            Menu();

            Console.ReadLine();
        }

        public static void Menu()
        {
            Console.WriteLine("Выберите задание, для выхода: 0");

            int number = Convert.ToInt32(Console.ReadLine());

            switch (number)
            {
                case 0:
                    break;
                case 1:
                    Task1();
                    break;
                case 2:
                    Task2();
                    break;
                case 3:
                    Task3();
                    break;
                default:
                    Menu();
                    break;
            }
        }

        public static void Task1()
        {
            Console.WriteLine("-Задание 1-");

            CreateFileAndDirectory("text");

            WriteFileInformation();

            Menu();
        }
        public static void CreateFileAndDirectory(string nameFile)
        {
            Console.WriteLine(path);

            if (!Directory.Exists(path + "texts"))
            {
                Directory.CreateDirectory(path);

                Console.WriteLine("Папка создана: " + path);
            }

            filePath = path + "\\" + nameFile + ".txt";
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                Console.WriteLine("Файл создан: " + filePath);
            }

            PrintInformationToFile();
            PrintInformationToFile();
            PrintInformationToFile();
            PrintInformationToFile();
        }
        public static void PrintInformationToFile()
        {
            if (File.Exists(filePath))
            {
                File.AppendAllText(filePath, fio);
                Console.WriteLine("Файл перезаписан: " + filePath);
            }
        }
        public static void WriteFileInformation()
        {
            string[] informationLines = File.ReadAllLines(filePath);
            Console.WriteLine(File.ReadAllText(filePath));
            Console.WriteLine("Количество строк: " + informationLines.Length);
        }
        public static void Task2()
        {
            Console.WriteLine("-Задание 2-");

            CreateFileAndDirectory("example");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(OLDWORD);
                for (int i = 1; i <= 10; i++)
                {
                    writer.WriteLine(i);
                }
                writer.WriteLine(OLDWORD);
                writer.WriteLine(OLDWORD);
                writer.WriteLine(OLDWORD);

            }

            Console.WriteLine($"Файл {filePath} успешно создан и заполнен!");

            Menu();
        }

        public static void Task3()
        {
            filePath = path + "\\" + "example" + ".txt";

            Console.WriteLine(filePath + " путь!");
            try
            {
                string fileContent = File.ReadAllText(filePath);

                string newContent = Regex.Replace(fileContent, @"\b" + OLDWORD + @"\b", NEWWORD);

                // Записываем измененный текст обратно в файл (перезаписывая его)
                File.WriteAllText(filePath, newContent);

                Console.WriteLine("Замена слов выполнена успешно.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл не найден.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Ошибка при работе с файлом: " + ex.Message);
            }

            Menu();
        }
    }
}