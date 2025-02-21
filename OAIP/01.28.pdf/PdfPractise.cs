namespace OAIP
{

    using static System.Console;
    using static OAIP_Files;

    using PdfSharp.Pdf;
    using PdfSharp.Drawing;
    using PdfSharp.Fonts;

    class PdfPractise
    {

        private static string path;
        private static bool isDevoperEdition;

        public PdfPractise(bool isDevoperEditionI)
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            isDevoperEdition = isDevoperEditionI;

            WriteLine("---28.01.25---");
            WriteLine("---Практика pdf---");

            path = Directory.GetCurrentDirectory() + "\\01.28.pdf";

            WriteLine(path);

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
                    DeleteFilesInDirectory("D:\\directory-git\\ktits-repository\\OAIP\\01.28.pdf\\.pdfs");
                    Menu();
                    break;
                default:
                    Menu();
                    break;
            }
        }

        public static void Task1()
        {
            string filePath = CreateFileAndDirectory("task1", path, ".pdf", false);

            PdfDocument document = new PdfDocument();

            document.Info.Title = "PDF с датой";

            var font = new XFont("Arial", 10, XFontStyleEx.Bold);
            string formattedDate = "Практика: 28.01.2025";

            // Создаем новую страницу
            for (int i = 0; i < 5; i++)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // Добавляем дату на текущую страницу
                gfx.DrawString(formattedDate, font, XBrushes.Red, new XRect(-210, 0, page.Width, page.Height), XStringFormats.BottomCenter);
            }

            // string file = Directory.GetCurrentDirectory() + "\\testFiles" + ".pdf";

            WriteLine("Путь: " + filePath);
            document.Save(filePath + "\\test1.pdf");
        }
        public static void Task2()
        {
            string filePath = CreateFileAndDirectory("task1", path, ".pdf", false);

            PdfDocument document = new PdfDocument();
            document.Info.Title = "PDF с таблицей";

            XFont font = new XFont("Times New Roman", 36, XFontStyleEx.Italic);

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Делим нашу страничку на 3 x 3
            double cellWidth = page.Width / 3;
            double cellHeight = page.Height / 3;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    //
                    double x = col * cellWidth;
                    double y = row * cellHeight;

                    gfx.DrawRectangle(XPens.Green, x, y, cellWidth, cellHeight);

                    // Делим ячейку на 2 части, чтобы вычислить центр
                    double textX = x + cellWidth / 2;
                    double textY = y + cellHeight / 2;


                    gfx.DrawString((row + 1).ToString() + ", " + (col + 1).ToString(), font, XBrushes.Black, new XRect(x, y, cellWidth, cellHeight), XStringFormats.Center);
                }
            }

            WriteLine("Путь: " + filePath);
            document.Save(filePath + "\\test2.pdf");

        }
    }
}