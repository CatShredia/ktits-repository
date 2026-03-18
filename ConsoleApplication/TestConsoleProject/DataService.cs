using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsoleProject
{
    public class DataService
    {
        private readonly DataImporter _importer;
        private readonly DataExport _export;

        public DataService()
        {
            _importer = new DataImporter();
            _export = new DataExport();
        }

        public DataService(DataImporter importer, DataExport export)
        {
            _importer = importer;
            _export = export;
        }

        public string TransferData(string sourceFilePath, string destinationFilePath = null)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                Console.WriteLine("Source file path is empty or null");
                return null;
            }

            Console.WriteLine($"Importing data from: {sourceFilePath}");
            string data = _importer.ImportDateFromTxt(sourceFilePath);

            if (data == null)
            {
                Console.WriteLine("Failed to import data");
                return null;
            }

            Console.WriteLine($"Data imported successfully: {data.Length} characters");
            Console.WriteLine($"Exporting data to: {destinationFilePath ?? "default location"}");
            
            string resultPath = _export.ExportDataToFile(data, destinationFilePath);

            if (resultPath != null)
            {
                Console.WriteLine($"Data transfer completed successfully to: {resultPath}");
            }
            else
            {
                Console.WriteLine("Data transfer failed");
            }

            return resultPath;
        }

        public string MergeAndExportData(List<string> sourceFilePaths, string destinationFilePath = null)
        {
            if (sourceFilePaths == null || sourceFilePaths.Count == 0)
            {
                Console.WriteLine("Source file paths list is empty or null");
                return null;
            }

            Console.WriteLine($"Merging {sourceFilePaths.Count} files...");
            var combinedData = new List<string>();

            foreach (var filePath in sourceFilePaths)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine($"Skipping empty file path");
                    continue;
                }

                try
                {
                    string data = _importer.ImportDateFromTxt(filePath);
                    if (data != null)
                    {
                        combinedData.Add(data);
                        Console.WriteLine($"Successfully imported: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error importing {filePath}: {ex.Message}");
                }
            }

            if (combinedData.Count == 0)
            {
                Console.WriteLine("No data was imported from any source file");
                return null;
            }

            string mergedData = string.Join(Environment.NewLine, combinedData);
            Console.WriteLine($"Exporting merged data ({combinedData.Count} sources) to destination...");
            
            return _export.ExportDataToFile(mergedData, destinationFilePath);
        }

        public string TransformAndExportData(
            string sourceFilePath, 
            Func<string, string> transformation, 
            string destinationFilePath = null)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                Console.WriteLine("Source file path is empty or null");
                return null;
            }

            if (transformation == null)
            {
                Console.WriteLine("Transformation function is null");
                return null;
            }

            Console.WriteLine($"Importing data from: {sourceFilePath}");
            string data = _importer.ImportDateFromTxt(sourceFilePath);

            if (data == null)
            {
                Console.WriteLine("Failed to import data");
                return null;
            }

            Console.WriteLine("Applying transformation...");
            string transformedData = transformation(data);

            if (transformedData == null)
            {
                Console.WriteLine("Transformation returned null");
                return null;
            }

            Console.WriteLine($"Exporting transformed data...");
            return _export.ExportDataToFile(transformedData, destinationFilePath);
        }
    }
}
