namespace OAIP
{
    class FilesPractise
    {
        public static string fio = "Evimov Aleksandr";
        public static string filePath = "";
        public static string path = "";

        public FilesPractise(bool isDevoperEdition)
        {
            Console.WriteLine("---Практика с файлами---");
            Console.WriteLine("21.01.2025");

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
                default:
                    Menu();
                    break;
            }
        }

        public static void Task1()
        {
            Console.WriteLine("-Задание 1-");
            path = Directory.GetCurrentDirectory() + "\\files\\texts";


            CreateFileAndDirectory();

            WriteFileInformation();

            Menu();
        }
        public static void CreateFileAndDirectory()
        {
            Console.WriteLine(path);

            if (!Directory.Exists(path + "texts"))
            {
                Directory.CreateDirectory(path);

                Console.WriteLine("Папка создана: " + path);
            }

            filePath = path + "\\text.txt";
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                Console.WriteLine("Файл создан: " + filePath);

            }
            if (File.Exists(filePath))
            {
                File.AppendAllText(filePath, fio);
                File.AppendAllText(filePath, "\n ");
                File.AppendAllText(filePath, fio);
                Console.WriteLine("Файл заполнен: " + filePath);
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
        }
    }
}