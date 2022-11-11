// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class AxHostVisualBasic6Tests
    {
        [WinFormsFact]
        public void HostME()
        {
            using Form form = new();
            using SimpleControl control = new();
            ((ISupportInitialize)control).BeginInit();
            form.Controls.Add(control);
            ((ISupportInitialize)control).EndInit();
            form.ShowDialog();
        }
    }

    public class SimpleControl : DynamicHost
    {
        public SimpleControl()
            : base(
                  "366D0F1F-2D8A-4C9B-897D-CE1E74EDD6E5",
                  Path.GetFullPath(@"TestResources\VB6\SimpleControl.ocx"))
        {
        }
    }

    public unsafe class DynamicHost : AxHost
    {
        private const string ExportMethodName = "DllGetClassObject";
        private readonly string _path;
        private readonly HINSTANCE _instance;

        // private delegate HRESULT DllGetClassObjectDelegate

        public DynamicHost(string clsid, string path) : base(clsid, 0)
        {
            _path = path;
            _instance = PInvoke.LoadLibraryEx(path, HANDLE.Null, default);
        }

        protected override object CreateInstanceCore(Guid clsid)
        {
            FARPROC proc = PInvoke.GetProcAddress(_instance, ExportMethodName);

            //HRESULT DllGetClassObject(
            //  [in] REFCLSID rclsid,
            //  [in] REFIID riid,
            //  [out] LPVOID* ppv
            //);

            IClassFactory* factory;
            Guid IID_IClassFactory = IClassFactory.Guid;
            HRESULT result = ((delegate* unmanaged<Guid*, Guid*, void**, HRESULT>)proc.Value)(&clsid, &IID_IClassFactory, (void**)&factory);
            IUnknown* unknown;
            factory->CreateInstance(null, IUnknown.NativeGuid, (void**)&unknown);
            object obj = Marshal.GetObjectForIUnknown((nint)unknown);
            return obj;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
