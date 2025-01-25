using static System.Console;

using static OAIP.OAIP_Files;

namespace OAIP
{

    class FilePlactise2
    {

        private static string path;
        public static bool isDevoperEdition;

        public FilePlactise2(bool isDevoperEdition)
        {
            FilePlactise2.isDevoperEdition = isDevoperEdition;

            WriteLine("---24.01.25---");

            path = Directory.GetCurrentDirectory() + "\\24.01.files";

            WriteLine(path);

            Menu();

            ReadKey();
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
                    Menu();
                    break;
                case 2:
                    Task2();
                    Menu();
                    break;
                case 3:
                    Task3();
                    Menu();
                    break;
                case 100:
                    WriteLine("Delete Directory: " + "D:\\directory-git\\ktits-repository\\OAIP\\24.01.files\\texts");
                    DeleteFilesInDirectory("D:\\directory-git\\ktits-repository\\OAIP\\24.01.files\\texts");
                    Menu();
                    break;
                default:
                    Menu();
                    break;
            }
        }

        private static void Task1()
        {
            WriteLine("-1 задание-");

            path = CreateFileAndDirectory("text", path);

            WriteLine();
            if (FilePlactise2.isDevoperEdition)
            {
                WriteInformationToFile("Привет, как дела?\nЯ изучаю работу с файлами.\nЭто интересно!", path);
            }
            else
            {
                WriteLine("Введите текст файла: ");
                WriteInformationToFile(ReadLine(), path);
            }

            WriteLine(CountOfLines(path) + " - количество строк");
            WriteLine(CountOfWords(path) + " - количество слов");
            WriteLine(CountOfChar(path) + " - количество символов");

            path = "";
        }
        private static void Task2()
        {
            WriteLine("-2 задание-");
            path = CreateFileAndDirectory("text2", path);

            WriteLine(path);

            if (FilePlactise2.isDevoperEdition)
            {
                WriteInformationToFile("Привет, мир! Мир прекрасен.\nЯ люблю изучать программирование и мир технологий.", path);
            }
            else
            {
                WriteLine("Введите текст файла: ");
                WriteInformationToFile(ReadLine(), path);
            }

            WriteLine("Введи заменяемое слово");
            string OLDWORD = ReadLine().Split(" ")[0];
            WriteLine("Введи слово, которым нужно заменить");
            string NEWWORD = ReadLine().Split(" ")[0];

            ReplaceWords(path, OLDWORD, NEWWORD);
        }
        private static void Task3()
        {

        }

    }
}