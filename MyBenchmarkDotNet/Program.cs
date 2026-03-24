using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Text;

public class StringTest
{
    string[] numbers = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };

    [Benchmark]
    public string WithStringBuilder()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string s in numbers)
        {
            stringBuilder.Append(s);
            stringBuilder.Append(".");
        }
        return stringBuilder.ToString();
    }

    [Benchmark]
    public string WithConcatenation()
    {
        string result = "";
        foreach (string s in numbers) result = result + s + ".";
        return result;
    }

    [Benchmark]
    public string WithInterpolation()
    {
        string result = "";
        foreach (string s in numbers) result = $"{result}{s}.";
        return result;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<StringTest>();
    }
}