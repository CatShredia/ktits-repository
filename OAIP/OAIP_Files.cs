namespace OAIP
{
    class OAIP_Files
    {
        public static string CreateFileAndDirectory(string nameFile, string path)
        {
            path += "\\texts";

            if (!Directory.Exists(path + "texts"))
            {
                Directory.CreateDirectory(path);

                Console.WriteLine("Папка создана: " + path);
            }

            string filePath = path + "\\" + nameFile + ".txt";
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                Console.WriteLine("Файл создан: " + filePath);
            }

            return filePath;
        }
        public static void WriteFileInformation(string filepath)
        {
            string[] informationLines = File.ReadAllLines(filepath);
            Console.WriteLine(File.ReadAllText(filepath));
            Console.WriteLine("Количество строк: " + informationLines.Length);
        }

        public static void WriteInformationToFile(string information, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(information);  //  Переходим на новую строку, после добавления текста
                }
                Console.WriteLine("Текст успешно добавлен в конец файла.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Ошибка записи в файл: " + ex.Message);
            }
        }

        public static void DeleteFilesInDirectory(string dir)
        {
            try
            {
                Directory.Delete(dir);
            }
            catch (IOException)
            {
                Console.WriteLine("Папка не пуста, продолжит удаление? (да / нет)");

                if (Console.ReadLine() == "да" ||
                Console.ReadLine() == "yes")
                {
                    // Получаем все файлы в указанной папке
                    string[] files = Directory.GetFiles(dir);

                    // Удаляем каждый файл
                    foreach (string file in files)
                    {
                        File.Delete(file);
                        Console.WriteLine($"Файл {file} успешно удален.");
                    }

                    Directory.Delete(dir);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Произошло исключение: " + e);
            }

        }
    }
}