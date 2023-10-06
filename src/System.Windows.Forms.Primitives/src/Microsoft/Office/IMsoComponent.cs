// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Microsoft.Office;

/// <inheritdoc cref="Interface"/>
internal unsafe partial struct IMsoComponent : IComIID, IVTable<IMsoComponent, IMsoComponent.Vtbl>
{
    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[]
            {
                0x00, 0x06, 0x0c, 0x00,
                0x00, 0x00,
                0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46
            };

            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    #region VTable
    private readonly void** _lpVtbl;

    internal struct Vtbl
    {
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, Guid*, void**, HRESULT> QueryInterface_1;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, uint> AddRef_2;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, uint> Release_3;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, HANDLE, uint, WPARAM, LPARAM, BOOL> FDebugMessage_4;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, MSG*, BOOL> FPreTranslateMessage_5;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, msocstate, BOOL, void> OnEnterState_6;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, BOOL, uint, void> OnAppActivate_7;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, void> OnLoseActivation_8;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, IMsoComponent*, BOOL, MSOCRINFO*, BOOL, nint, uint, void> OnActivationChange_9;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, msoidlef, BOOL> FDoIdle_10;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, msoloop, void*, MSG*, BOOL> FContinueMessageLoop_11;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, BOOL, BOOL> FQueryTerminate_12;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, void> Terminate_13;
        internal delegate* unmanaged[Stdcall]<IMsoComponent*, msocWindow, uint, HWND> HwndGetWindow_14;
    }

    static void IVTable<IMsoComponent, Vtbl>.PopulateVTable(Vtbl* vtable)
    {
        vtable->FDebugMessage_4 = &FDebugMessage;
        vtable->FPreTranslateMessage_5 = &FPreTranslateMessage;
        vtable->OnEnterState_6 = &OnEnterState;
        vtable->OnAppActivate_7 = &OnAppActivate;
        vtable->OnLoseActivation_8 = &OnLoseActivation;
        vtable->OnActivationChange_9 = &OnActivationChange;
        vtable->FDoIdle_10 = &FDoIdle;
        vtable->FContinueMessageLoop_11 = &FContinueMessageLoop;
        vtable->FQueryTerminate_12 = &FQueryTerminate;
        vtable->Terminate_13 = &Terminate;
        vtable->HwndGetWindow_14 = &HwndGetWindow;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FDebugMessage(
        IMsoComponent* @this,
        HANDLE hInst,
        uint msg,
        WPARAM wParam,
        LPARAM lParam)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface, BOOL>(
            @this,
            o => o.FDebugMessage(hInst, msg, wParam, lParam));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FPreTranslateMessage(IMsoComponent* @this, MSG* msg)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface, BOOL>(
            @this,
            o => o.FPreTranslateMessage(msg));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void OnEnterState(
        IMsoComponent* @this,
        msocstate uStateID,
        BOOL fEnter)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface>(
            @this,
            o => o.OnEnterState(uStateID, fEnter));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void OnAppActivate(
        IMsoComponent* @this,
        BOOL fActive,
        uint dwOtherThreadID)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface>(
            @this,
            o => o.OnAppActivate(fActive, dwOtherThreadID));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void OnLoseActivation(IMsoComponent* @this)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface>(
            @this,
            o => o.OnLoseActivation());

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void OnActivationChange(
        IMsoComponent* @this,
        IMsoComponent* pic,
        BOOL fSameComponent,
        MSOCRINFO* pcrinfo,
        BOOL fHostIsActivating,
        nint pchostinfo,
        uint dwReserved)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface>(
            @this,
            o => o.OnActivationChange(pic, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FDoIdle(
        IMsoComponent* @this,
        msoidlef grfidlef)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface, BOOL>(
            @this,
            o => o.FDoIdle(grfidlef));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FContinueMessageLoop(
        IMsoComponent* @this,
        msoloop uReason,
        void* pvLoopData,
        MSG* pMsgPeeked)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface, BOOL>(
            @this,
            o => o.FContinueMessageLoop(uReason, pvLoopData, pMsgPeeked));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FQueryTerminate(
        IMsoComponent* @this,
        BOOL fPromptUser)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface, BOOL>(
            @this,
            o => o.FQueryTerminate(fPromptUser));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void Terminate(IMsoComponent* @this)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface>(
            @this,
            o => o.Terminate());

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HWND HwndGetWindow(
        IMsoComponent* @this,
        msocWindow dwWhich,
        uint dwReserved)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponent, Interface, HWND>(
            @this,
            o => o.HwndGetWindow(dwWhich, dwReserved));
    #endregion

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    internal HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, Guid*, void**, HRESULT>)_lpVtbl[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef"/>
    internal uint AddRef()
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, uint>)_lpVtbl[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release"/>
    internal uint Release()
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, uint>)_lpVtbl[2])(pThis);
    }

    /// <inheritdoc cref="Interface.FDebugMessage(HANDLE, uint, WPARAM, LPARAM)"/>
    internal BOOL FDebugMessage(
        HANDLE hInst,
        uint msg,
        WPARAM wParam,
        LPARAM lParam)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, HANDLE, uint, WPARAM, LPARAM, BOOL>)_lpVtbl[3])(pThis, hInst, msg, wParam, lParam);
    }

    /// <inheritdoc cref="Interface.FPreTranslateMessage(MSG*)"/>
    internal BOOL FPreTranslateMessage(MSG* msg)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, MSG*, BOOL>)_lpVtbl[4])(pThis, msg);
    }

    /// <inheritdoc cref="Interface.OnEnterState(msocstate, BOOL)"/>
    internal void OnEnterState(
        msocstate uStateID,
        BOOL fEnter)
    {
        fixed (IMsoComponent* pThis = &this)
            ((delegate* unmanaged[Stdcall]<IMsoComponent*, msocstate, BOOL, void>)_lpVtbl[5])(pThis, uStateID, fEnter);
    }

    /// <inheritdoc cref="Interface.OnAppActivate(BOOL, uint)"/>
    internal void OnAppActivate(
        BOOL fActive,
        uint dwOtherThreadID)
    {
        fixed (IMsoComponent* pThis = &this)
            ((delegate* unmanaged[Stdcall]<IMsoComponent*, BOOL, uint, void>)_lpVtbl[6])(pThis, fActive, dwOtherThreadID);
    }

    /// <inheritdoc cref="Interface.OnLoseActivation"/>
    internal void OnLoseActivation()
    {
        fixed (IMsoComponent* pThis = &this)
            ((delegate* unmanaged[Stdcall]<IMsoComponent*, void>)_lpVtbl[7])(pThis);
    }

    /// <inheritdoc cref="Interface.OnActivationChange(IMsoComponent*, BOOL, MSOCRINFO*, BOOL, nint, uint)"/>
    internal void OnActivationChange(
        IMsoComponent* pic,
        BOOL fSameComponent,
        MSOCRINFO* pcrinfo,
        BOOL fHostIsActivating,
        nint pchostinfo,
        uint dwReserved)
    {
        fixed (IMsoComponent* pThis = &this)
            ((delegate* unmanaged[Stdcall]<IMsoComponent*, IMsoComponent*, BOOL, MSOCRINFO*, BOOL, nint, uint, void>)_lpVtbl[8])
                (pThis, pic, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);
    }

    /// <inheritdoc cref="Interface.FDoIdle(msoidlef)"/>
    internal BOOL FDoIdle(
        msoidlef grfidlef)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, msoidlef, BOOL>)_lpVtbl[9])(pThis, grfidlef);
    }

    /// <inheritdoc cref="Interface.FContinueMessageLoop(msoloop, void*, MSG*)"/>
    internal BOOL FContinueMessageLoop(
        msoloop uReason,
        void* pvLoopData,
        MSG* pMsgPeeked)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, msoloop, void*, MSG*, BOOL>)_lpVtbl[10])(pThis, uReason, pvLoopData, pMsgPeeked);
    }

    /// <inheritdoc cref="Interface.FQueryTerminate(BOOL)"/>
    internal BOOL FQueryTerminate(
        BOOL fPromptUser)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, BOOL, BOOL>)_lpVtbl[11])(pThis, fPromptUser);
    }

    /// <inheritdoc cref="Interface.Terminate"/>
    internal void Terminate()
    {
        fixed (IMsoComponent* pThis = &this)
            ((delegate* unmanaged[Stdcall]<IMsoComponent*, void>)_lpVtbl[12])(pThis);
    }

    /// <inheritdoc cref="Interface.HwndGetWindow(msocWindow, uint)"/>
    public HWND HwndGetWindow(
        msocWindow dwWhich,
        uint dwReserved)
    {
        fixed (IMsoComponent* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponent*, msocWindow, uint, HWND>)_lpVtbl[12])(pThis, dwWhich, dwReserved);
    }
}
