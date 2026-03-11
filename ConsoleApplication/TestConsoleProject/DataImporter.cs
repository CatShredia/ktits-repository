using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConsoleProject
{
    public class DataImporter
    {
        public string ImportDateFromTxt(string absoluteTxtFilePath)
        {
            string lines = string.Join(" ", File.ReadAllLines(absoluteTxtFilePath));

            return lines;
        }
    }
}