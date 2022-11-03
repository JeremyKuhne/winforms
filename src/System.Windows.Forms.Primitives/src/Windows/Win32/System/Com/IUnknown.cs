// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IUnknown : INativeGuid, IUnknown.Interface
{
    public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));

    public ComScope<T> QueryInterface<T>(out HRESULT hr) where T : unmanaged, INativeGuid
    {
        fixed (IUnknown* unknown = &this)
        {
            ComScope<T> scope = new(null);
            hr = unknown->QueryInterface(T.NativeGuid, scope);
            return scope;
        }
    }

    public ComScope<T> QueryInterface<T>(out bool success) where T : unmanaged, INativeGuid
    {
        ComScope<T> scope = QueryInterface<T>(out HRESULT hr);
        success = hr.Succeeded;
        return scope;
    }

    internal interface Interface
    {
        public HRESULT QueryInterface(Guid* riid, void** ppvObject);
        public uint AddRef();
        public uint Release();
    }
}
