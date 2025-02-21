// Данный код реализует класс NPOI, который позволяет пользователю создавать документы Word (.docx) с использованием библиотеки NPOI. 
// Он предоставляет два задания: первое задание создает список из трех пунктов, а второе - таблицу с заданным количеством строк и столбцов. 
// Пользователь может выбирать задания через меню, а также удалять файлы в определенной директории.

using NPOI.XWPF.UserModel;
using static System.Console;
using static OAIP.OAIP_Files;
namespace OAIP
{
    class NPOI
    {
        public static bool isDevelopEdition;
        public static string filePath;
        public NPOI(bool isDevelopEditioni)
        {
            isDevelopEdition = isDevelopEditioni;
            filePath = Directory.GetCurrentDirectory() + "\\01.31.docx";
            WriteLine(filePath);
            filePath = CreateFileAndDirectory("test", filePath, ".docx", false);
            Menu();
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
                case 100:
                    DeleteFilesInDirectory("D:\\directory-git\\ktits-repository\\OAIP\\01.31.docx\\.docxs");
                    Menu();
                    break;
                default:
                    Menu();
                    break;
            }
        }
        public static void Task1()
        {
            string filePathI = filePath + "\\meth1.docx";
            List<string> list = new List<string>();
            WriteLine("Введите 1 пункт");
            list.Add(ReadLine());
            WriteLine("Введите 2 пункт");
            list.Add(ReadLine());
            WriteLine("Введите 3 пункт");
            list.Add(ReadLine());
            // Создаем новый документ
            using (XWPFDocument document = new XWPFDocument())
            {
                // Создаем параграф для заголовка
                XWPFParagraph titleParagraph = document.CreateParagraph();
                XWPFRun titleRun = titleParagraph.CreateRun();
                titleRun.SetText("Создан список из 3 пунктов:");
                titleRun.FontFamily = "Times New Roman";
                titleRun.FontSize = 14;
                // Создаем список
                foreach (var item in list)
                {
                    XWPFParagraph listParagraph = document.CreateParagraph();
                    XWPFRun listRun = listParagraph.CreateRun();
                    listRun.SetText("• " + item); // Добавляем маркер списка
                    listRun.FontFamily = "Times New Roman";
                    listRun.FontSize = 12;
                }
                // Сохраняем документ
                using (FileStream fs = new FileStream(filePathI, FileMode.Create, FileAccess.Write))
                {
                    document.Write(fs);
                }
            }
        }
        public static void Task2()
        {
            string filePathI = filePath + "\\meth2.docx";
            WriteLine("Введите кол-во строк");
            int couLine = Convert.ToInt32(ReadLine());
            WriteLine("Введите кол-во столбцов");
            int couColumn = Convert.ToInt32(ReadLine());
            using (XWPFDocument document = new XWPFDocument())
            {
                // Создаем таблицу 3x3
                XWPFTable table = document.CreateTable(couLine, couColumn); // 3 строки, 3 столбца
                int cellNumber = 1; // Счетчик ячеек
                // Заполняем таблицу
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.GetTableCells())
                    {
                        // Получаем или создаем параграф в ячейке
                        XWPFParagraph paragraph = cell.Paragraphs.Count > 0
                            ? cell.Paragraphs[0]
                            : cell.AddParagraph();
                        // Настраиваем текст и шрифт
                        XWPFRun run = paragraph.CreateRun();
                        run.SetText($"Hi {cellNumber}"); // Текст ячейки
                        run.FontFamily = "Times New Roman";
                        run.FontSize = 12;
                        cellNumber++; // Увеличиваем номер ячейки
                    }
                }
                // Сохраняем документ
                using (FileStream fs = new FileStream(filePathI, FileMode.Create))
                {
                    document.Write(fs);
                }
            }
        }
    }
}