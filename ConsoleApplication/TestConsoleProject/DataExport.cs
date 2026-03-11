using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsoleProject
{
    public class DataExport
    {
        public DataExport()
        {
            
        }

        public string ExportDataToFile(string data, string absoluteFilePath = null)
        {
            if (data == null)
            {
                Console.WriteLine("data is null, stop");
                return null;
            }
            if (absoluteFilePath == null)
            {
                Console.WriteLine("path is null, path set to current user's desctop folder");
                absoluteFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "text.txt");
            }

            try
            {
                File.WriteAllText(absoluteFilePath, data);
                return absoluteFilePath;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Exception of unauthorized acces");
                return null;
                
            }
            catch (Exception)
            {
                Console.WriteLine("Exception occured with file work");
                return null;
            }
        }
    }
}