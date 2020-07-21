// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms;
using BenchmarkDotNet.Attributes;

namespace TestApp
{
    [MemoryDiagnoser]
    [IterationCount(1000)]
    public class RefCachePerf
    {
        private IntColorCache _cache;

        [IterationSetup]
        public void IterationSetup()
        {
            _cache = new IntColorCache(N + 1, N + 1);
            for (int i = 0; i < N; i++)
            {
                using var color = _cache.GetEntry(i);
            }
        }

        [Params(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
        public int N;

//|         Method |   N |       Mean |    Error |    StdDev |     Median |
//|--------------- |---- |-----------:|---------:|----------:|-----------:|
//| RetrieveOldest |   1 |   577.5 ns | 16.06 ns |  42.02 ns |   600.0 ns |
//| RetrieveOldest |  10 |   725.9 ns | 22.94 ns |  62.02 ns |   700.0 ns |
//| RetrieveOldest |  20 |   840.0 ns | 22.25 ns |  60.16 ns |   800.0 ns |
//| RetrieveOldest |  30 |   889.7 ns | 21.14 ns |  30.99 ns |   900.0 ns |
//| RetrieveOldest |  40 |   969.4 ns | 23.17 ns |  57.26 ns | 1,000.0 ns |
//| RetrieveOldest |  50 | 1,077.8 ns | 25.24 ns |  42.16 ns | 1,100.0 ns |
//| RetrieveOldest |  60 | 1,182.4 ns | 23.96 ns |  38.70 ns | 1,200.0 ns |
//| RetrieveOldest |  70 | 1,326.4 ns | 61.65 ns | 168.76 ns | 1,300.0 ns |
//| RetrieveOldest |  80 | 1,464.0 ns | 66.45 ns | 180.79 ns | 1,400.0 ns |
//| RetrieveOldest |  90 | 1,477.2 ns | 28.85 ns |  62.73 ns | 1,500.0 ns |
//| RetrieveOldest | 100 | 1,541.4 ns | 34.20 ns |  50.12 ns | 1,500.0 ns |

        //  Move Always

//|         Method |   N |       Mean |     Error |    StdDev |     Median |
//|--------------- |---- |-----------:|----------:|----------:|-----------:|
//| RetrieveOldest |   1 |   868.5 ns |  24.05 ns |  67.82 ns |   900.0 ns |
//| RetrieveOldest |  10 | 1,010.6 ns |  29.12 ns |  78.72 ns | 1,000.0 ns |
//| RetrieveOldest |  20 | 1,117.3 ns |  37.82 ns | 110.32 ns | 1,100.0 ns |
//| RetrieveOldest |  30 | 1,215.5 ns |  30.84 ns |  82.86 ns | 1,200.0 ns |
//| RetrieveOldest |  40 | 1,252.0 ns |  28.71 ns |  57.99 ns | 1,200.0 ns |
//| RetrieveOldest |  50 | 1,355.6 ns |  30.80 ns |  58.60 ns | 1,400.0 ns |
//| RetrieveOldest |  60 | 1,418.4 ns |  31.79 ns |  63.49 ns | 1,400.0 ns |
//| RetrieveOldest |  70 | 1,589.9 ns |  35.61 ns |  86.00 ns | 1,600.0 ns |
//| RetrieveOldest |  80 | 1,769.0 ns | 111.04 ns | 303.97 ns | 1,700.0 ns |
//| RetrieveOldest |  90 | 1,713.3 ns |  38.17 ns |  57.13 ns | 1,700.0 ns |
//| RetrieveOldest | 100 | 1,770.4 ns |  33.19 ns |  46.53 ns | 1,800.0 ns |

        // Move after 30

//|         Method |   N |       Mean |    Error |   StdDev |     Median |
//|--------------- |---- |-----------:|---------:|---------:|-----------:|
//| RetrieveOldest |   1 |   562.6 ns | 22.96 ns | 64.37 ns |   600.0 ns |
//| RetrieveOldest |  10 |   744.9 ns | 20.37 ns | 59.42 ns |   700.0 ns |
//| RetrieveOldest |  20 |   815.6 ns | 24.59 ns | 68.55 ns |   800.0 ns |
//| RetrieveOldest |  30 |   875.0 ns | 21.17 ns | 43.72 ns |   900.0 ns |
//| RetrieveOldest |  40 | 1,268.3 ns | 28.92 ns | 52.15 ns | 1,300.0 ns |
//| RetrieveOldest |  50 | 1,351.8 ns | 30.68 ns | 66.03 ns | 1,300.0 ns |
//| RetrieveOldest |  60 | 1,470.6 ns | 32.90 ns | 67.21 ns | 1,500.0 ns |
//| RetrieveOldest |  70 | 1,520.0 ns | 30.58 ns | 40.82 ns | 1,500.0 ns |
//| RetrieveOldest |  80 | 1,613.2 ns | 36.09 ns | 62.26 ns | 1,600.0 ns |
//| RetrieveOldest |  90 | 1,706.7 ns | 31.42 ns | 79.41 ns | 1,700.0 ns |
//| RetrieveOldest | 100 | 1,782.4 ns | 38.26 ns | 39.30 ns | 1,800.0 ns |

//|         Method |   N |       Mean |    Error |    StdDev |     Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
//|--------------- |---- |-----------:|---------:|----------:|-----------:|------:|------:|------:|----------:|
//| RetrieveOldest |   1 |   667.4 ns | 29.32 ns |  82.69 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  10 |   892.9 ns | 34.08 ns |  91.55 ns |   900.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  20 | 1,055.8 ns | 24.96 ns |  67.92 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  30 | 1,182.9 ns | 26.75 ns |  68.09 ns | 1,200.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  40 | 1,439.4 ns | 31.94 ns |  78.34 ns | 1,400.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  50 | 1,746.5 ns | 41.73 ns | 112.81 ns | 1,750.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  60 | 1,851.4 ns | 38.32 ns |  65.07 ns | 1,800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  70 | 2,061.5 ns | 39.80 ns |  93.03 ns | 2,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  80 | 2,074.3 ns | 45.12 ns |  74.13 ns | 2,100.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  90 | 2,259.1 ns | 48.07 ns |  59.03 ns | 2,300.0 ns |     - |     - |     - |         - |
//| RetrieveOldest | 100 | 2,403.6 ns | 51.91 ns |  74.45 ns | 2,400.0 ns |     - |     - |     - |         - |

// Agressive inlining
//|         Method |   N |       Mean |    Error |    StdDev |     Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
//|--------------- |---- |-----------:|---------:|----------:|-----------:|------:|------:|------:|----------:|
//| RetrieveOldest |   1 |   623.6 ns | 24.94 ns |  69.12 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  10 |   822.3 ns | 52.20 ns | 148.94 ns |   800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  20 |   886.0 ns | 18.92 ns |  35.06 ns |   900.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  30 |   937.5 ns | 22.69 ns |  48.85 ns |   900.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  40 | 1,149.3 ns | 26.87 ns |  66.92 ns | 1,100.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  50 | 1,234.6 ns | 28.49 ns |  73.55 ns | 1,200.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  60 | 1,330.0 ns | 30.42 ns |  61.45 ns | 1,300.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  70 | 1,391.1 ns | 31.34 ns |  59.63 ns | 1,400.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  80 | 1,503.4 ns | 20.84 ns |  45.75 ns | 1,500.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  90 | 1,632.4 ns | 33.12 ns |  53.49 ns | 1,600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest | 100 | 1,705.0 ns | 33.59 ns |  59.70 ns | 1,700.0 ns |     - |     - |     - |         - |

        // Agressive and final return bool
//|         Method |   N |       Mean |    Error |   StdDev |     Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
//|--------------- |---- |-----------:|---------:|---------:|-----------:|------:|------:|------:|----------:|
//| RetrieveOldest |   1 |   498.5 ns | 28.51 ns | 188.7 ns |   500.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  10 |   597.8 ns | 26.79 ns | 173.1 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  20 |   671.0 ns | 31.07 ns | 203.2 ns |   700.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  30 |   696.6 ns | 35.21 ns | 230.0 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  40 |   896.8 ns | 43.30 ns | 283.2 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  50 |   947.6 ns | 49.25 ns | 320.4 ns |   800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  60 | 1,017.3 ns | 60.66 ns | 395.8 ns |   900.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  70 | 1,083.8 ns | 55.74 ns | 364.9 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  80 | 1,180.6 ns | 68.41 ns | 447.4 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  90 | 1,181.3 ns | 62.32 ns | 408.5 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest | 100 | 1,382.0 ns | 71.34 ns | 462.0 ns | 1,600.0 ns |     - |     - |     - |         - |

        // No set local
//|         Method |   N |       Mean |    Error |   StdDev |     Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
//|--------------- |---- |-----------:|---------:|---------:|-----------:|------:|------:|------:|----------:|
//| RetrieveOldest |   1 |   460.8 ns | 28.86 ns | 192.0 ns |   400.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  10 |   609.7 ns | 25.90 ns | 166.6 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  20 |   672.9 ns | 33.64 ns | 222.6 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  30 |   764.3 ns | 38.05 ns | 247.8 ns |   700.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  40 |   910.0 ns | 45.38 ns | 296.8 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  50 |   988.3 ns | 46.46 ns | 301.9 ns | 1,100.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  60 | 1,067.1 ns | 60.05 ns | 391.0 ns | 1,100.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  70 | 1,242.8 ns | 63.70 ns | 408.4 ns | 1,400.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  80 | 1,176.4 ns | 63.04 ns | 412.7 ns | 1,200.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  90 | 1,284.9 ns | 70.66 ns | 459.1 ns | 1,200.0 ns |     - |     - |     - |         - |
//| RetrieveOldest | 100 | 1,396.3 ns | 68.90 ns | 445.2 ns | 1,600.0 ns |     - |     - |     - |         - |

        // Cache return end
//|         Method |   N |       Mean |    Error |   StdDev |     Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
//|--------------- |---- |-----------:|---------:|---------:|-----------:|------:|------:|------:|----------:|
//| RetrieveOldest |   1 |   385.8 ns | 24.37 ns | 154.9 ns |   300.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  10 |   738.4 ns | 43.82 ns | 291.2 ns |   700.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  20 |   665.9 ns | 38.20 ns | 247.7 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  30 |   744.8 ns | 37.04 ns | 242.2 ns |   700.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  40 |   897.2 ns | 48.64 ns | 314.0 ns |   800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  50 |   993.5 ns | 53.34 ns | 346.2 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  60 | 1,069.9 ns | 51.75 ns | 338.1 ns | 1,200.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  70 | 1,170.3 ns | 58.66 ns | 377.0 ns | 1,350.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  80 | 1,182.9 ns | 65.16 ns | 425.7 ns | 1,100.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  90 | 1,282.5 ns | 68.97 ns | 444.6 ns | 1,300.0 ns |     - |     - |     - |         - |
//| RetrieveOldest | 100 | 1,419.3 ns | 85.14 ns | 548.3 ns | 1,500.0 ns |     - |     - |     - |         - |

//|         Method |   N |       Mean |    Error |   StdDev |     Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
//|--------------- |---- |-----------:|---------:|---------:|-----------:|------:|------:|------:|----------:|
//| RetrieveOldest |   1 |   413.1 ns | 15.34 ns | 144.0 ns |   400.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  10 |   538.7 ns | 18.41 ns | 173.8 ns |   500.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  20 |   583.4 ns | 22.09 ns | 208.2 ns |   500.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  30 |   656.1 ns | 22.49 ns | 212.9 ns |   500.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  40 |   763.9 ns | 27.69 ns | 260.6 ns |   600.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  50 |   919.3 ns | 35.35 ns | 331.6 ns |   800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  60 |   950.4 ns | 35.85 ns | 334.8 ns |   800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  70 | 1,021.9 ns | 40.95 ns | 384.0 ns |   800.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  80 | 1,100.9 ns | 42.49 ns | 394.3 ns | 1,000.0 ns |     - |     - |     - |         - |
//| RetrieveOldest |  90 | 1,125.2 ns | 46.47 ns | 432.8 ns |   900.0 ns |     - |     - |     - |         - |
//| RetrieveOldest | 100 | 1,248.1 ns | 48.84 ns | 454.3 ns |   950.0 ns |     - |     - |     - |         - |

        [Benchmark]
        public Color RetrieveOldest()
        {
            return _cache.GetEntry(0);
        }

        internal sealed class IntColorCache : RefCache<Color, Color, int>
        {
            public IntColorCache(int softLimit, int hardLimit) : base(softLimit, hardLimit) { }

            protected override CacheEntry CreateEntry(int key, bool cached)
                => new ColorData(Color.FromArgb(key), cached);
            protected override bool IsMatch(int key, CacheEntry data) => key == data.Data.ToArgb();

            internal class ColorData : CacheEntry
            {
                public ColorData(Color color, bool cached) : base(color, cached) { }
                public override Color Object => Data;
            }
        }
    }
}
