namespace Statistics;

public class StatisticsClass
{
    private readonly double[] _values;

    public int N => _values.Length;

    public StatisticsClass(double[] values)
    {
        _values = values ?? Array.Empty<double>();
    }

    // находим среднее арифмитическое 
    public double Mean()
    {
        if (N == 0)
        {
            return 0;
        }

        double sum = 0;
        foreach (var value in _values)
        {
            sum += value;
        }
        return sum / N;
    }

    // среднеквадратичное отклонение
    public double StdDeviation()
    {
        if (N < 2)
        {
            return 0;
        }

        double mean = Mean();
        double sumOfSquares = 0;

        foreach (var value in _values)
        {
            double diff = value - mean;
            sumOfSquares += diff * diff;
        }

        double variance = sumOfSquares / (N - 1);
        return Math.Sqrt(variance);
    }
}
