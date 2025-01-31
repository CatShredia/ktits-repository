using Xceed.Words.NET;
using Xceed.Document.NET;
using System.IO;

using static System.Console;
using static OAIP.OAIP_Files;

namespace OAIP
{
    class Word
    {
        private const string KEY = "WDN30-K7ARA-6UXEP-LA1A";

        public static bool isDevoperEdition;

        public Word(bool isDevoperEditionI)
        {
            try
            {
                Xceed.Document.NET.Licenser.LicenseKey = KEY;

                isDevoperEdition = isDevoperEditionI;

                string filePath = Directory.GetCurrentDirectory() + "\\31.01.docx\\files";

                WriteLine(filePath);

                filePath = CreateFileAndDirectory("test", filePath, ".docx", false) + "\\";

                WriteLine(filePath);

                // Создаем ворд документ
                using (var document = DocX.Create(filePath))
                {
                    // Добавляем заголовок
                    document.InsertParagraph("пример - ворд документ")
                        .FontSize(20)
                        .Bold()
                        .Alignment = Alignment.center;

                    // Добавляем текст
                    document.InsertParagraph("Файл создан в си шарпе ")
                        .FontSize(20)
                        .Italic()
                        .Font("Arial")
                        .Alignment = Alignment.right;

                    WriteLine("Документ создан");

                    document.Save();
                }
            }
            catch(Exception e){
                WriteLine("----ВНИМАНИ! БЕЗ КЛЮЧА НЕ РАБОТАЕТ----");

                WriteLine(e);
            }


        }
    }
}