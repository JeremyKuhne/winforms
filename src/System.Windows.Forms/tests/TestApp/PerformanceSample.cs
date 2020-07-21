// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using BenchmarkDotNet.Attributes;
using System.Windows.Forms;
using System.Windows.Forms.Internal;
using static Interop;

namespace TestApp
{
    // https://benchmarkdotnet.org/

    // [MemoryDiagnoser] does GC analysis (collections and allocations)
    [MemoryDiagnoser]
    public class PerformanceSample
    {
        private Bitmap _bitmap;
        private Graphics _graphics;
        private Gdi32.HDC _hdc;
        private Gdi32.HBITMAP _hbitmap;

        // Global gets run once for the class
        [GlobalSetup]
        public void GlobalSetup()
        {
            _bitmap = new Bitmap(10, 10);
            _graphics = Graphics.FromImage(_bitmap);
            _hdc = Gdi32.CreateCompatibleDC(default);
            _hbitmap = Gdi32.CreateCompatibleBitmap(_hdc, 10, 10);
            Gdi32.SelectObject(_hdc, _hbitmap);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _graphics?.Dispose();
            _bitmap?.Dispose();
            Gdi32.DeleteObject(_hbitmap);
            Gdi32.DeleteDC(_hdc);
        }

        // Iteration is done each method invocation
        //[IterationSetup]
        //public void IterationSetup()
        //{
        //}

        //[IterationCleanup]
        //public void IterationCleanup()
        //{
        //}

        // Baseline uses this method to compare against (e.g. this method will be 1.0x)

        [Benchmark]
        public void DCScope_Overhead()
        {
            using var hdc = new DeviceContextHdcScope(_graphics);
        }

        [Benchmark]
        public void DCScope_NoApply_Overhead()
        {
            using var hdc = new DeviceContextHdcScope(_graphics, applyGraphicsState: false);
        }

        [Benchmark]
        public void DCScope_SaveHdc_Overhead()
        {
            using var hdc = new DeviceContextHdcScope(_graphics, saveHdcState: true);
        }

        [Benchmark]
        public void SaveDc()
        {
            using var saveDC = new Gdi32.SaveDcScope(_hdc);
        }

        [Benchmark]
        public void CreateDCScope()
        {
            using var hdc = new Gdi32.CreateDcScope(default);
        }
    }
}
