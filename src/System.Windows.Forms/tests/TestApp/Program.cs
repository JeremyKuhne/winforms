// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BenchmarkDotNet.Running;
using WinFormsPerf;
using static Interop;

namespace TestApp
{
    public class Program
    {
        private unsafe static void Main(string[] args)
        {
            // This project is to facilitate running sample "apps" against the current enlistment, like so:

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //var form = new OneControl();
            //form.ShowDialog();

            //User32.RedrawWindow(form.Handle, null, default, User32.RDW.INVALIDATE | User32.RDW.ERASE | User32.RDW.ALLCHILDREN);
            //User32.UpdateWindow(form.Handle);

            //User32.RedrawWindow(form.Handle, null, default, User32.RDW.INVALIDATE | User32.RDW.ERASE | User32.RDW.ALLCHILDREN);
            //User32.UpdateWindow(form.Handle);

            //using var hdc = GdiCache.GetScreenDC();
            //Application.Run(new Form1());

            //var perf = new FontCachePerf();
            //perf.GlobalSetup();
            //perf.RetrieveExisting_FromEnd();

            //var perf = new ScreenDcCachePerf();
            //perf.RentPresent();
            //perf.RentPresent();
            //perf.RentPresent();

            // It also can be used to test performance, like so:
            var summary = BenchmarkRunner.Run<RefCachePerf>();
        }
    }
}
