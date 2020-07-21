using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsPerf;
// using Slow471Repro;

namespace SlowCoreRepro
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new DataGridViewForm();
            form.Show();

            while (true)
            {
                Interop.RedrawWindow(form.Handle, default, default, Interop.RDW.INVALIDATE | Interop.RDW.ERASE | Interop.RDW.ALLCHILDREN);
                Interop.UpdateWindow(form.Handle);
            }
            // Application.Run(new DataGridViewForm());
        }
    }

    public static class Interop
    {
        [Flags]
        public enum RDW : uint
        {
            INVALIDATE = 0x0001,
            INTERNALPAINT = 0x0002,
            ERASE = 0x0004,
            VALIDATE = 0x0008,
            NOINTERNALPAINT = 0x0010,
            NOERASE = 0x0020,
            NOCHILDREN = 0x0040,
            ALLCHILDREN = 0x0080,
            UPDATENOW = 0x0100,
            ERASENOW = 0x0200,
            FRAME = 0x0400,
            NOFRAME = 0x0800,
        }

        [DllImport("User32.dll", ExactSpelling = true)]
        public static extern int RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RDW flags);

        [DllImport("User32.dll", ExactSpelling = true)]
        public static extern int UpdateWindow(IntPtr hWnd);
    }
}
