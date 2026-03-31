namespace Statistics;

/// <summary>
/// A class for statistical analysis of numerical data samples.
/// </summary>
public class StatisticsClass
{
    private readonly double[] _values;

    /// <summary>
    /// Gets the number of elements in the sample.
    /// </summary>
    public int N => _values.Length;

    /// <summary>
    /// Initializes a new instance of the StatisticsClass with the specified values.
    /// </summary>
    /// <param name="values">Array of numerical values</param>
    public StatisticsClass(double[] values)
    {
        _values = values ?? Array.Empty<double>();
    }

    /// <summary>
    /// Calculates the arithmetic mean of the sample.
    /// </summary>
    /// <returns>Arithmetic mean, or 0 if sample is empty</returns>
    public double Mean()
    {
        if (N == 0)
            return 0;

        double sum = 0;
        foreach (var value in _values)
        {
            sum += value;
        }
        return sum / N;
    }

    /// <summary>
    /// Calculates the standard deviation of the sample (sample standard deviation).
    /// </summary>
    /// <returns>Standard deviation, or 0 if sample has less than 2 elements</returns>
    public double StdDeviation()
    {
        if (N < 2)
            return 0;

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
