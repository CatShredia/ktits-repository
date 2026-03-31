```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.6199/23H2/2023Update/SunValley3)
AMD Ryzen 5 5600H with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.418
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2


```
| Method            | Mean          | Error        | StdDev       | Gen0   | Allocated |
|------------------ |--------------:|-------------:|-------------:|-------:|----------:|
| ArraySort1_Large  |   7,450.49 ns |   106.007 ns |    93.973 ns | 0.4730 |    4056 B |
| ArraySort2_Large  | 454,877.12 ns | 8,418.931 ns | 7,875.073 ns |      - |    4056 B |
| ArraySort3_Large  |  13,907.10 ns |    80.526 ns |    75.324 ns | 0.4730 |    4056 B |
| ArraySort4_Large  |  23,532.81 ns |   192.756 ns |   180.304 ns | 0.9460 |    8080 B |
| ArraySort1_Medium |     442.95 ns |     6.595 ns |     5.846 ns | 0.0544 |     456 B |
| ArraySort2_Medium |   4,698.94 ns |    50.524 ns |    47.260 ns | 0.0534 |     456 B |
| ArraySort3_Medium |     755.00 ns |    14.792 ns |    13.837 ns | 0.0544 |     456 B |
| ArraySort4_Medium |   1,766.50 ns |    23.552 ns |    19.667 ns | 0.1049 |     880 B |
| ArraySort1_Small  |      46.33 ns |     0.671 ns |     0.628 ns | 0.0114 |      96 B |
| ArraySort2_Small  |      72.25 ns |     0.741 ns |     0.693 ns | 0.0114 |      96 B |
| ArraySort3_Small  |      72.56 ns |     1.351 ns |     1.264 ns | 0.0114 |      96 B |
| ArraySort4_Small  |     140.82 ns |     2.640 ns |     2.340 ns | 0.0191 |     160 B |
