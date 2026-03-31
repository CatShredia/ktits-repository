using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;

namespace SortBenchmark;

[HtmlExporter]
[MemoryDiagnoser]
public class SortBenchmarks
{
    private readonly int[] _testArraySmall = [5, 2, 8, 1, 9, 3, 7, 4, 6, 10];
    private readonly int[] _testArrayMedium;
    private readonly int[] _testArrayLarge;

    public SortBenchmarks()
    {
        _testArrayMedium = GenerateArray(100);
        _testArrayLarge = GenerateArray(1000);
    }

    private static int[] GenerateArray(int size)
    {
        var random = new Random(42);
        var arr = new int[size];
        for (int i = 0; i < size; i++)
        {
            arr[i] = random.Next(1, 10000);
        }
        return arr;
    }

    [Benchmark]
    [BenchmarkCategory("Small")]
    public int[] ArraySort1_Small()
    {
        int[] arr = _testArraySmall.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort1();
    }

    [Benchmark]
    [BenchmarkCategory("Small")]
    public int[] ArraySort2_Small()
    {
        int[] arr = _testArraySmall.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort2();
    }

    [Benchmark]
    [BenchmarkCategory("Small")]
    public int[] ArraySort3_Small()
    {
        int[] arr = _testArraySmall.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort3();
    }

    [Benchmark]
    [BenchmarkCategory("Small")]
    public int[] ArraySort4_Small()
    {
        int[] arr = _testArraySmall.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort4();
    }

    [Benchmark]
    [BenchmarkCategory("Medium")]
    public int[] ArraySort1_Medium()
    {
        int[] arr = _testArrayMedium.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort1();
    }

    [Benchmark]
    [BenchmarkCategory("Medium")]
    public int[] ArraySort2_Medium()
    {
        int[] arr = _testArrayMedium.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort2();
    }

    [Benchmark]
    [BenchmarkCategory("Medium")]
    public int[] ArraySort3_Medium()
    {
        int[] arr = _testArrayMedium.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort3();
    }

    [Benchmark]
    [BenchmarkCategory("Medium")]
    public int[] ArraySort4_Medium()
    {
        int[] arr = _testArrayMedium.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort4();
    }

    [Benchmark]
    [BenchmarkCategory("Large")]
    public int[] ArraySort1_Large()
    {
        int[] arr = _testArrayLarge.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort1();
    }

    [Benchmark]
    [BenchmarkCategory("Large")]
    public int[] ArraySort2_Large()
    {
        int[] arr = _testArrayLarge.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort2();
    }

    [Benchmark]
    [BenchmarkCategory("Large")]
    public int[] ArraySort3_Large()
    {
        int[] arr = _testArrayLarge.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort3();
    }

    [Benchmark]
    [BenchmarkCategory("Large")]
    public int[] ArraySort4_Large()
    {
        int[] arr = _testArrayLarge.ToArray();
        var sorter = new SortClass(arr);
        return sorter.ArraySort4();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<SortBenchmarks>(DefaultConfig.Instance);
    }
}
