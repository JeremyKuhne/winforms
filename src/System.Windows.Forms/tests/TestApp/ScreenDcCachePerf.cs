// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using System.Windows.Forms;
using System.Windows.Forms.Internal;
using static Interop;
using System.Collections.Generic;

namespace TestApp
{
    [MemoryDiagnoser]
    public class ScreenDcCachePerf
    {
        //ScreenDcCache _cache = new ScreenDcCache(0);
        ScreenDcCache _cache2 = new ScreenDcCache(1);

        //[Benchmark]
        //public void CreateNewAndReturn()
        //{
        //    using var hdc = _cache.Acquire();
        //}

        [Benchmark]
        public void RentPresent()
        {
            using var hdc = _cache2.Acquire();
        }
    }
}
