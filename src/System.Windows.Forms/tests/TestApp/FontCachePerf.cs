// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BenchmarkDotNet.Attributes;
using static Interop;

namespace TestApp
{
    // https://benchmarkdotnet.org/

    // [MemoryDiagnoser] does GC analysis (collections and allocations)
    [MemoryDiagnoser]
    public class FontCachePerf
    {
        private FontCache _cache1;
        private Font _font1 = new Font("Arial", 14.0f);
        private List<Font> _fonts = new List<Font>();

        // Global gets run once for the class
        [GlobalSetup]
        public void GlobalSetup()
        {
            _cache1 = new FontCache();
            _cache1.GetEntry(_font1, Gdi32.QUALITY.DEFAULT);
            for (int i = 1; i < 20; i++)
            {
                Font font = new Font("Times New Roman", i);
                _fonts.Add(font);
                _cache1.GetEntry(font, Gdi32.QUALITY.DEFAULT);
            }
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
        }

        [IterationSetup]
        public void IterationSetup()
        {
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
        }

        // [Benchmark]
        public void RetrieveNew()
        {
            _cache1.GetEntry(_font1, Gdi32.QUALITY.DEFAULT);
        }

        // [Benchmark]
        public void RetrieveExisting()
        {
            _cache1.GetEntry(_font1, Gdi32.QUALITY.DEFAULT);
        }

        // [Benchmark]
        public void ToHFONT()
        {
            _font1.ToHFONT();
        }

        [Benchmark]
        public void RetrieveExisting_FromEnd()
        {
            _cache1.GetEntry(_font1, Gdi32.QUALITY.DEFAULT);
        }
    }
}
