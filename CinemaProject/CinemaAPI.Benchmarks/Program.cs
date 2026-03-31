using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace CinemaAPI.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub)
            .AddExporter(BenchmarkDotNet.Exporters.HtmlExporter.Default)
            .WithSummaryStyle(BenchmarkDotNet.Reports.SummaryStyle.Default)
            .AddJob(Job.ShortRun
                .WithWarmupCount(1)
                .WithIterationCount(3));

        // Run all benchmarks
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly, config);

        Console.WriteLine("Benchmarks completed!");
        Console.WriteLine($"Results saved in BenchmarkDotNet.Artifacts folder");
    }
}
