namespace OAIP
{
    class Files
    {
        public Files(bool isDevoperEdition) {
            lesson();
        }
        public void lesson()
        {
            //указываем путь к файлу
            string filePath = "text.txt";

            //Указываем полный путь
            string newFailPath = @"D:\directory-git\ktits-repository\OAIP\text.txt";

            //Данные для записи в фаил
            string content = "Текст для файла";

            //Данные для записи в файл на рабочем столе
            string contentDesktop = "Текст для файла на рабочем столе";

            //Запись данных в файл
            File.WriteAllText(filePath, content);

            //Запись в файл на рабочем столе
            File.WriteAllText(newFailPath, contentDesktop);

            //Данные для добавления в файл
            string newData = "\nЭто новые данные";

            //Метод добавления новых данных
            File.AppendAllText(filePath, newData);

            //Чтение данных из файла
            string content2 = File.ReadAllText(filePath);

            //Чтение файла построчно
            foreach (string line in File.ReadLines(newFailPath))
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("Данные в файл записаны");
            Console.WriteLine("Данные, хранящиеся в файле: " + content2);
            
            Console.ReadKey();
        }
    }

}

