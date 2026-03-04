using System;
using System.Collections.Generic;
using System.Linq;

namespace TestConsoleProject
{

    public class LandPlotList
    {
        private List<double> plots;

        public LandPlotList()
        {
            plots = new List<double>();
        }

        // Добавить площадь
        public void AddPlot(double area)
        {
            if (area <= 0)
                throw new ArgumentException("Площадь участка должна быть положительной.");
            plots.Add(area);
        }

        public void SortByAreaAscending()
        {
            plots.Sort();
        }

        // удалить все участки, площадь которых меньше заданной
        public void RemovePlotsBelowThreshold(double threshold)
        {
            plots.RemoveAll(area => area < threshold);
        }

        // получить текущий список участков
        public List<double> GetPlots()
        {
            return new List<double>(plots);
        }

        // очистить список
        public void Clear()
        {
            plots.Clear();
        }

        // получить количество участков
        public int Count => plots.Count;
    }
}