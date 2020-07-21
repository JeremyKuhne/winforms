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
    public class Scopes
    {
        private Bitmap _bitmap;
        private Graphics _graphics;
        private Gdi32.HDC _hdc;
        private Gdi32.HBITMAP _hbitmap;
        private Gdi32.HPEN _pen1;
        private Gdi32.HPEN _pen2;
        private int _count;

        // Global gets run once for the class
        [GlobalSetup]
        public void GlobalSetup()
        {
            _bitmap = new Bitmap(10, 10);
            _graphics = Graphics.FromImage(_bitmap);
            _hdc = Gdi32.CreateCompatibleDC(default);
            _hbitmap = Gdi32.CreateCompatibleBitmap(_hdc, 10, 10);
            Gdi32.SelectObject(_hdc, _hbitmap);

            _pen1 = Gdi32.CreatePen(Gdi32.PS.SOLID, 1, 42);
            _pen2 = Gdi32.CreatePen(Gdi32.PS.SOLID, 1, 1988);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _graphics?.Dispose();
            _bitmap?.Dispose();
            Gdi32.DeleteObject(_hbitmap);
            Gdi32.DeleteDC(_hdc);
        }

        //[Benchmark]
        //public void SaveDc()
        //{
        //    using var saveDC = new Gdi32.SaveDcScope(_hdc);
        //}

        //[Benchmark]
        //public void CreateDCScope()
        //{
        //    using var hdc = new Gdi32.CreateDcScope(default);
        //}

        [Benchmark]
        public void CreatePenScope()
        {
            using var pen = new Gdi32.CreatePenScope(Color.AliceBlue);
        }

        [Benchmark]
        public void CreateBrushScope()
        {
            using var brush = new Gdi32.CreateBrushScope(Color.Aqua);
        }

        [Benchmark]
        public void CreatePenScope_AndSelect()
        {
            using var pen = new Gdi32.CreatePenScope(Color.AliceBlue);
            using var select = new Gdi32.SelectObjectScope(_hdc, pen);
        }

        [Benchmark]
        public void CreateBrushScope_AndSelect()
        {
            using var brush = new Gdi32.CreateBrushScope(Color.Aqua);
            using var select = new Gdi32.SelectObjectScope(_hdc, brush);
        }

        [Benchmark]
        public void SelectPen_Alternating()
        {
            using var select = new Gdi32.SelectObjectScope(
                _hdc,
                (_count++ % 2) == 0 ? _pen1 : _pen2);
        }
    }
}
