namespace OAIP
{
    using System.Text.RegularExpressions;

    using static OAIP_Files;

    using static System.Console;

    class FilesPractise
    {
        public static string fio = "Evimov Aleksandr" + "\n";
        public static string filePath = "";
        public static string path = "";

        public const string OLDWORD = "старое";
        public const string NEWWORD = "новое";

        public FilesPractise(bool isDevoperEdition)
        {
            WriteLine("---Практика с файлами---");
            WriteLine("21.01.2025");

            path = Directory.GetCurrentDirectory() + "\\21.01.files";

            Menu();

            ReadLine();
        }

        public static void Menu()
        {
            WriteLine("Выберите задание, для выхода: 0");

            int number = Convert.ToInt32(ReadLine());

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
                    WriteLine("Delete Directory: " + path + ".txts");
                    DeleteFilesInDirectory(path + "\\.txts");
                    break;
                default:
                    Menu();
                    break;
            }
        }

        public static void Task1()
        {
            WriteLine("-Задание 1-");

            filePath = CreateFileAndDirectory("text", path, ".txt", true);

            TxtWriteInformationToFile("старое \nстарое \nАлександр Ефимов\nАлександр Ефимов", filePath);

            TxtWriteFileInformation(filePath);

            Menu();
        }

        public static void Task2()
        {
            WriteLine("-Задание 2-");

            filePath = CreateFileAndDirectory("example", path, ".txt", true);
            TxtWriteInformationToFile("старое\nстарое\nстарое\nстарое\nстарое\n", filePath);

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
            WriteLine(filePath + " путь!");
            
            TxtReplaceWords(filePath, OLDWORD,NEWWORD,false);

            Menu();
        }

    }
}