```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.6199/23H2/2023Update/SunValley3)
AMD Ryzen 5 5600H with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103
  [Host]   : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=1  

```
| Method                     | Mean      | Error        | StdDev    | Median    | Gen0   | Allocated |
|--------------------------- |----------:|-------------:|----------:|----------:|-------:|----------:|
| GetAllFilms                | 127.41 μs | 1,003.886 μs | 55.026 μs | 136.39 μs | 3.4180 |  30.57 KB |
| GetFilmsSortedByRating     |  67.08 μs |    58.720 μs |  3.219 μs |  66.69 μs | 4.3945 |  37.16 KB |
| GetFilmsSortedByRatingDesc |  63.92 μs |     6.480 μs |  0.355 μs |  63.73 μs | 4.3945 |  37.16 KB |
| GetFilmsByGenre            |  77.94 μs |   681.221 μs | 37.340 μs |  65.46 μs | 2.9297 |  23.97 KB |
| GetFilmsBySearch           | 102.24 μs |   803.170 μs | 44.024 μs | 119.72 μs | 3.1738 |  26.73 KB |
| GetFilmById                |  63.51 μs |   609.828 μs | 33.427 μs |  49.75 μs | 2.1973 |  19.67 KB |
| GetAverageRating           |  11.71 μs |     0.660 μs |  0.036 μs |  11.70 μs | 0.8545 |   7.08 KB |
