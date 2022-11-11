// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Windows.Win32.System.Com
{
    internal unsafe partial struct IUnknown : INativeGuid
    {
        public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));

        // https://github.com/microsoft/CsWin32/issues/724
        internal interface Interface
        {
            // Can't do this yet as the generated members aren't public. Creating it anyway to help constrain our
            // helpers a bit more.
            // https://github.com/microsoft/CsWin32/issues/723

            //internal unsafe HRESULT QueryInterface(Guid* riid, void** ppvObject);
            //internal uint AddRef();
            //internal uint Release();
        }
    }
}
