
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.6199/23H2/2023Update/SunValley3)
AMD Ryzen 5 5600H with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103
  [Host]   : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=1  

 Method             | Mean     | Error      | StdDev    | Gen0   | Allocated |
------------------- |---------:|-----------:|----------:|-------:|----------:|
 GetAllRatings      | 35.05 μs |   5.732 μs |  0.314 μs | 3.6621 |  30.53 KB |
 GetRatingById      | 34.08 μs |  91.729 μs |  5.028 μs | 1.9531 |   16.3 KB |
 CreateRating       |       NA |         NA |        NA |     NA |        NA |
 UpdateRating       | 18.09 μs |   5.461 μs |  0.299 μs | 2.0447 |  16.83 KB |
 DeleteRating       | 32.81 μs | 227.244 μs | 12.456 μs | 1.0986 |   9.27 KB |
 GetMyRatingForFilm |       NA |         NA |        NA |     NA |        NA |

Benchmarks with issues:
  RatingsControllerBenchmarks.CreateRating: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=1)
  RatingsControllerBenchmarks.GetMyRatingForFilm: ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=1)
