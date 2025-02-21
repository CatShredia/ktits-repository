using static System.Console;

using static OAIP.OAIP_Files;
using static OAIP.OAIP_Strings;

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
                    DeleteFilesInDirectory("D:\\directory-git\\ktits-repository\\OAIP\\01.24.files\\.txts");
                    Menu();
                    break;
                default:
                    Menu();
                    break;
            }
        }

        private static void Task1()
        {
            path = Directory.GetCurrentDirectory() + "\\01.24.files";

            WriteLine("-1 задание-");

            path = CreateFileAndDirectory("text", path, ".txt", true);

            WriteLine();
            if (FilePlactise2.isDevoperEdition)
            {
                TxtWriteInformationToFile("Привет, как дела?\nЯ изучаю работу с файлами.\nЭто интересно!", path);
            }
            else
            {
                WriteLine("Введите текст файла: ");
                TxtWriteInformationToFile(ReadMoreLines(), path);

            }

            WriteLine(TxtCountOfLines(path) + " - количество строк");
            WriteLine(TxtCountOfWords(path) + " - количество слов");
            WriteLine(TxtCountOfChar(path) + " - количество символов");

            path = "";
        }

        private static void Task2()
        {
            path = Directory.GetCurrentDirectory() + "\\01.24.files ";

            WriteLine("-2 задание-");

            WriteLine(path);

            path = CreateFileAndDirectory("text2", path, ".txt", true);


            if (FilePlactise2.isDevoperEdition)
            {
                TxtWriteInformationToFile("Привет, мир! мир прекрасен.\nЯ люблю изучать программирование и мир технологий.", path);
            }
            else
            {
                WriteLine("Введите текст файла: ");
                TxtWriteInformationToFile(ReadMoreLines(), path);
            }

            WriteLine("Введи заменяемое слово");
            string OLDWORD = ReadLine().Split(" ")[0];
            WriteLine("Введи слово, которым нужно заменить");
            string NEWWORD = ReadLine().Split(" ")[0];

            TxtReplaceWords(path, OLDWORD, NEWWORD, true);
        }
        private static void Task3()
        {
            path = Directory.GetCurrentDirectory() + "\\01.24.files";

            WriteLine("-2 задание-");

            WriteLine(path);

            path = CreateFileAndDirectory("text3", path, ".txt", true);


            if (FilePlactise2.isDevoperEdition)
            {
                TxtWriteInformationToFile("Привет, мир! \nКак твои дела?\nПогода сегодня прекрасная.\nХорошего дня!\n", path);
            }
            else
            {
                WriteLine("Введите текст файла: ");
                TxtWriteInformationToFile(ReadMoreLines(), path);
            }

            WriteLine("Введите букву: ");
            char charUser = ReadLine()[0];
            charUser = Char.ToLower(charUser);

            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (Char.ToLower(lines[i][0]) == charUser)
                {
                    WriteLine(lines[i]);
                }

            }
        }

    }
}