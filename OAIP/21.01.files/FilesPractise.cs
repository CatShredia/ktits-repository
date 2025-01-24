namespace OAIP
{
    using System.Text.RegularExpressions;

    using static OAIP_Files;

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

            path = Directory.GetCurrentDirectory() + "\\21.01.files";

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
                case 100:
                    Console.WriteLine("Delete Directory: " + path + "\\texts");
                    DeleteFilesInDirectory(path + "\\texts");
                    break;
                default:
                    Menu();
                    break;
            }
        }

        public static void Task1()
        {
            Console.WriteLine("-Задание 1-");

            filePath = CreateFileAndDirectory("text", path);

            WriteInformationToFile("старое \nстарое \nАлександр Ефимов\nАлександр Ефимов", filePath);

            WriteFileInformation(filePath);

            Menu();
        }
        
        public static void Task2()
        {
            Console.WriteLine("-Задание 2-");

            filePath = CreateFileAndDirectory("example", path);
            WriteInformationToFile("старое\nстарое\nстарое\nстарое\nстарое\n", filePath);

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

            Menu();
        }

        public static void Task3()
        {
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