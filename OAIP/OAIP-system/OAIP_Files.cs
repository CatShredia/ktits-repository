namespace OAIP
{
    using System.Text.RegularExpressions;

    using static System.Console;

    using static OAIP_Arrays;

    class OAIP_Files
    {

        private static char[] splitChar = [' ', '\t', '\n', '\r'];

        public static string CreateFileAndDirectory(string nameFile, string path, string typeOfFile, bool createFile)
        {
            path += "\\" + typeOfFile + "s";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                WriteLine("Папка создана: " + path);
            }

            string filePath;
            if (!File.Exists(path + "\\" + nameFile + typeOfFile) && createFile == true)
            {
                filePath = path + "\\" + nameFile + typeOfFile;

                using (File.Create(filePath)) { }
                WriteLine("Файл создан: " + filePath);
            }
            else
            {
                filePath = path;
            }

            return filePath;
        }
        public static void TxtWriteFileInformation(string filepath)
        {
            string[] informationLines = File.ReadAllLines(filepath);
            WriteLine(File.ReadAllText(filepath));
            WriteLine("Количество строк: " + informationLines.Length);
        }

        public static void TxtWriteInformationToFile(string information, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(information);  //  Переходим на новую строку, после добавления текста
                }
                WriteLine("Текст успешно добавлен в конец файла.");
            }
            catch (IOException ex)
            {
                WriteLine("Ошибка записи в файл: " + ex.Message);
            }
        }

        public static void DeleteFilesInDirectory(string dir)
        {
            WriteLine("ВНИМАНИЕ! Удаление: " + dir);
            try
            {
                Directory.Delete(dir);
            }
            catch (IOException)
            {
                WriteLine("Папка не пуста, продолжит удаление? (да / нет)");

                if (ReadLine() == "да" ||
                 ReadLine() == "yes")
                {
                    // Получаем все файлы в указанной папке
                    string[] files = Directory.GetFiles(dir);

                    // Удаляем каждый файл
                    foreach (string file in files)
                    {
                        File.Delete(file);
                        WriteLine($"Файл {file} успешно удален.");
                    }

                    Directory.Delete(dir);

                }
            }
            catch (Exception e)
            {
                WriteLine("Произошло исключение: " + e);
            }

        }

        public static int TxtCountOfLines(string pathTxtFile)
        {
            int countOfLines = 0;
            try
            {
                string[] lines = File.ReadAllLines(pathTxtFile);

                WriteArray(lines, "string");

                countOfLines = lines.Length;
            }
            catch (Exception ex)
            {
                WriteLine($"Произошла ошибка: {ex.Message}");
            }

            return countOfLines;
        }

        public static int TxtCountOfWords(string pathTxtFile)
        {
            int countOfWords = 0;
            try
            {
                using (StreamReader reader = new StreamReader(pathTxtFile))
                {
                    string lines = reader.ReadToEnd();

                    // WriteArray(lines.Split(splitChar),"title");

                    countOfWords = lines.Split(splitChar).Length;
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Произошла ошибка: {ex.Message}");
            }

            countOfWords -= 2;
            return countOfWords;
        }

        public static int TxtCountOfChar(string pathTxtFile)
        {
            int countOfChar = 0;
            try
            {
                using (StreamReader reader = new StreamReader(pathTxtFile))
                {
                    string text = reader.ReadToEnd();

                    foreach (char charPer in text)
                    {
                        if (charPer != splitChar[1]
                            && charPer != splitChar[2]
                            && charPer != splitChar[3])
                        {
                            countOfChar++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Произошла ошибка: {ex.Message}");
            }

            return countOfChar;
        }

        public static void TxtReplaceWords(string filePath, string OLDWORD, string NEWWORD, bool isRegisterIxp)
        {
            try
            {
                string text = File.ReadAllText(filePath);

                WriteLine("Изначальный текст: " + text);

                string result = text.Replace(OLDWORD, NEWWORD);

                WriteLine("Конечный текст: " + result);

                File.WriteAllText(filePath, result);

            }
            catch (FileNotFoundException)
            {
                WriteLine("Файл не найден.");
            }
            catch (IOException ex)
            {
                WriteLine("Ошибка при работе с файлом: " + ex.Message);
            }

        }
    }
}