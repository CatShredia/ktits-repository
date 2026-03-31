```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.6199/23H2/2023Update/SunValley3)
AMD Ryzen 5 5600H with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103
  [Host]   : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=1  

```
| Method                 | Mean           | Error             | StdDev          | Median         | Gen0     | Gen1    | Allocated |
|----------------------- |---------------:|------------------:|----------------:|---------------:|---------:|--------:|----------:|
| RegisterUser           | 4,752,927.9 ns | 104,716,120.87 ns | 5,739,843.93 ns | 1,463,479.7 ns | 179.6875 | 15.6250 | 1532956 B |
| RegisterDuplicateLogin |    12,583.0 ns |      46,124.67 ns |     2,528.25 ns |    11,353.3 ns |   0.7324 |       - |    6264 B |
| LoginUser              |    39,120.3 ns |     218,144.35 ns |    11,957.23 ns |    32,679.9 ns |   1.7090 |       - |   14377 B |
| LoginInvalidUser       |    41,146.7 ns |     270,627.50 ns |    14,834.01 ns |    32,981.7 ns |   1.7090 |       - |   14377 B |
| LoginNonExistentUser   |    27,624.7 ns |      73,271.12 ns |     4,016.24 ns |    25,744.1 ns |   1.3428 |       - |   11480 B |
| HashPassword           |       431.0 ns |          85.39 ns |         4.68 ns |       431.0 ns |   0.0467 |       - |     392 B |
| VerifyPassword         |       430.9 ns |          73.04 ns |         4.00 ns |       429.5 ns |   0.0467 |       - |     392 B |
