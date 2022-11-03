// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IDispatch : INativeGuid
{
    public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));

    [ComImport]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface Interface
    {
        [PreserveSig]
        HRESULT GetTypeInfoCount(
            uint* pctinfo);

        [PreserveSig]
        HRESULT GetTypeInfo(
            uint iTInfo,
            PInvoke.LCID lcid,
            ITypeInfo** ppTInfo);

        [PreserveSig]
        HRESULT GetIDsOfNames(
            Guid* riid,
            PWSTR* rgszNames,
            uint cNames,
            PInvoke.LCID lcid,
            int* rgDispId);

        [PreserveSig]
        HRESULT Invoke(
            int dispIdMember,
            Guid* riid,
            PInvoke.LCID lcid,
            DISPATCH_FLAGS dwFlags,
            DISPPARAMS* pDispParams,
            VARIANT* pVarResult,
            EXCEPINFO* pExcepInfo,
            uint* pArgErr);
    }
}
