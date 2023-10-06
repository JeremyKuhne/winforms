// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Microsoft.Office;

/// <inheritdoc cref="Interface"/>
internal unsafe partial struct IMsoComponentManager : IComIID, IVTable<IMsoComponentManager, IMsoComponentManager.Vtbl>
{
    static ref readonly Guid IComIID.Guid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[]
            {
                0x01, 0x06, 0x0c, 0x00,
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
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, Guid*, void**, HRESULT> QueryInterface_1;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, uint> AddRef_2;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, uint> Release_3;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, Guid*, Guid*, void**, HRESULT> QueryService_4;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nint, uint, WPARAM, LPARAM, BOOL> FDebugMessage_5;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, IMsoComponent*, MSOCRINFO*, nuint*, BOOL> FRegisterComponent_6;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, BOOL> FRevokeComponent_7;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, MSOCRINFO*, BOOL> FUpdateComponentRegistration_8;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, BOOL> FOnComponentActivate_9;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, BOOL, BOOL> FSetTrackingComponent_10;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, msocstate, msoccontext, uint, IMsoComponentManager**, uint, void> OnComponentEnterState_11;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, msocstate, msoccontext, uint, IMsoComponentManager**, BOOL> FOnComponentExitState_12;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, msocstate, void*, BOOL> FInState_13;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, BOOL> FContinueIdle_14;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, msoloop, void*, BOOL> FPushMessageLoop_15;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, IUnknown*, IUnknown*, Guid*, void**, BOOL> FCreateSubComponentManager_16;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, IMsoComponentManager**, BOOL> FGetParentComponentManager_17;
        internal delegate* unmanaged[Stdcall]<IMsoComponentManager*, msogac, IMsoComponent**, MSOCRINFO*, uint, BOOL> FGetActiveComponent_18;
    }

    static void IVTable<IMsoComponentManager, Vtbl>.PopulateVTable(Vtbl* vtable)
    {
        vtable->QueryService_4 = &QueryService;
        vtable->FDebugMessage_5 = &FDebugMessage;
        vtable->FRegisterComponent_6 = &FRegisterComponent;
        vtable->FRevokeComponent_7 = &FRevokeComponent;
        vtable->FUpdateComponentRegistration_8 = &FUpdateComponentRegistration;
        vtable->FOnComponentActivate_9 = &FOnComponentActivate;
        vtable->FSetTrackingComponent_10 = &FSetTrackingComponent;
        vtable->OnComponentEnterState_11 = &OnComponentEnterState;
        vtable->FOnComponentExitState_12 = &FOnComponentExitState;
        vtable->FInState_13 = &FInState;
        vtable->FContinueIdle_14 = &FContinueIdle;
        vtable->FCreateSubComponentManager_16 = &FCreateSubComponentManager;
        vtable->FGetParentComponentManager_17 = &FGetParentComponentManager;
        vtable->FGetActiveComponent_18 = &FGetActiveComponent;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT QueryService(IMsoComponentManager* @this, Guid* guidService, Guid* iid, void** ppvObj)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface>(
            @this,
            o => o.QueryService(guidService, iid, ppvObj));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FDebugMessage(
        IMsoComponentManager* @this,
        nint dwReserved,
        uint msg,
        WPARAM wParam,
        LPARAM lParam)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FDebugMessage(dwReserved, msg, wParam, lParam));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FRegisterComponent(
        IMsoComponentManager* @this,
        IMsoComponent* piComponent,
        MSOCRINFO* pcrinfo,
        nuint* dwComponentID)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FRegisterComponent(piComponent, pcrinfo, dwComponentID));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FRevokeComponent(IMsoComponentManager* @this, nuint dwComponentID)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(@this, o => o.FRevokeComponent(dwComponentID));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FUpdateComponentRegistration(
        IMsoComponentManager* @this,
        nuint dwComponentID,
        MSOCRINFO* pcrinfo)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FUpdateComponentRegistration(dwComponentID, pcrinfo));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FOnComponentActivate(IMsoComponentManager* @this, nuint dwComponentID)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FOnComponentActivate(dwComponentID));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FSetTrackingComponent(
        IMsoComponentManager* @this,
        nuint dwComponentID,
        BOOL fTrack)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FSetTrackingComponent(dwComponentID, fTrack));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void OnComponentEnterState(
        IMsoComponentManager* @this,
        nuint dwComponentID,
        msocstate uStateID,
        msoccontext uContext,
        uint cpicmExclude,
        IMsoComponentManager** rgpicmExclude,
        uint dwReserved)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface>(
            @this,
            o => o.OnComponentEnterState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FOnComponentExitState(
        IMsoComponentManager* @this,
        nuint dwComponentID,
        msocstate uStateID,
        msoccontext uContext,
        uint cpicmExclude,
        IMsoComponentManager** rgpicmExclude)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FOnComponentExitState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FInState(
        IMsoComponentManager* @this,
        msocstate uStateID,
        void* pvoid)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FInState(uStateID, pvoid));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FContinueIdle(IMsoComponentManager* @this)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FContinueIdle());

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FPushMessageLoop(
        IMsoComponentManager* @this,
        nuint dwComponentID,
        msoloop uReason,
        void* pvLoopData)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FPushMessageLoop(dwComponentID, uReason, pvLoopData));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FCreateSubComponentManager(
        IMsoComponentManager* @this,
        IUnknown* punkOuter,
        IUnknown* punkServProv,
        Guid* riid,
        void** ppvObj)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FCreateSubComponentManager(punkOuter, punkServProv, riid, ppvObj));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FGetParentComponentManager(
        IMsoComponentManager* @this,
        IMsoComponentManager** ppicm)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FGetParentComponentManager(ppicm));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static BOOL FGetActiveComponent(
        IMsoComponentManager* @this,
        msogac dwgac,
        IMsoComponent** ppic,
        MSOCRINFO* pcrinfo,
        uint dwReserved)
        => WinFormsComWrappers.UnwrapAndInvoke<IMsoComponentManager, Interface, BOOL>(
            @this,
            o => o.FGetActiveComponent(dwgac, ppic, pcrinfo, dwReserved));
    #endregion

    /// <inheritdoc cref="IUnknown.QueryInterface(Guid*, void**)"/>
    internal HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, Guid*, void**, HRESULT>)_lpVtbl[0])(pThis, riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef"/>
    internal uint AddRef()
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, uint>)_lpVtbl[1])(pThis);
    }

    /// <inheritdoc cref="IUnknown.Release"/>
    internal uint Release()
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, uint>)_lpVtbl[2])(pThis);
    }

    /// <inheritdoc cref="Interface.QueryService(Guid*, Guid*, void**)"/>
    internal HRESULT QueryService(
        Guid* guidService,
        Guid* iid,
        void** ppvObj)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, Guid*, Guid*, void**, HRESULT>)_lpVtbl[3])(pThis, guidService, iid, ppvObj);
    }

    /// <inheritdoc cref="Interface.FDebugMessage(nint, uint, WPARAM, LPARAM)"/>
    internal BOOL FDebugMessage(
        nint dwReserved,
        uint msg,
        WPARAM wParam,
        LPARAM lParam)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nint, uint, WPARAM, LPARAM, BOOL>)_lpVtbl[4])(pThis, dwReserved, msg, wParam, lParam);
    }

    /// <inheritdoc cref="Interface.FRegisterComponent(IMsoComponent*, MSOCRINFO*, nuint*)"/>
    internal BOOL FRegisterComponent(
        IMsoComponent* piComponent,
        MSOCRINFO* pcrinfo,
        nuint* dwComponentID)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, IMsoComponent*, MSOCRINFO*, nuint*, BOOL>)_lpVtbl[5])(pThis, piComponent, pcrinfo, dwComponentID);
    }

    /// <inheritdoc cref="Interface.FRevokeComponent(nuint)"/>
    internal BOOL FRevokeComponent(nuint dwComponentID)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, BOOL>)_lpVtbl[6])(pThis, dwComponentID);
    }

    /// <inheritdoc cref="Interface.FUpdateComponentRegistration(nuint, MSOCRINFO*)"/>
    internal BOOL FUpdateComponentRegistration(
        nuint dwComponentID,
        MSOCRINFO* pcrinfo)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, MSOCRINFO*, BOOL>)_lpVtbl[7])(pThis, dwComponentID, pcrinfo);
    }

    /// <inheritdoc cref="Interface.FOnComponentActivate(nuint)"/>
    internal BOOL FOnComponentActivate(nuint dwComponentID)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, BOOL>)_lpVtbl[8])(pThis, dwComponentID);
    }

    /// <inheritdoc cref="Interface.FSetTrackingComponent(nuint, BOOL)"/>
    internal BOOL FSetTrackingComponent(
        nuint dwComponentID,
        BOOL fTrack)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, BOOL, BOOL>)_lpVtbl[9])(pThis, dwComponentID, fTrack);
    }

    /// <inheritdoc cref="Interface.OnComponentEnterState(nuint, msocstate, msoccontext, uint, IMsoComponentManager**, uint)"/>
    internal void OnComponentEnterState(
        nuint dwComponentID,
        msocstate uStateID,
        msoccontext uContext,
        uint cpicmExclude,
        IMsoComponentManager** rgpicmExclude,
        uint dwReserved)
    {
        fixed (IMsoComponentManager* pThis = &this)
            ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, msocstate, msoccontext, uint, IMsoComponentManager**, uint, void>)_lpVtbl[10])
                (pThis, dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved);
    }

    /// <inheritdoc cref="Interface.FOnComponentExitState(nuint, msocstate, msoccontext, uint, IMsoComponentManager**)"/>
    internal BOOL FOnComponentExitState(
        nuint dwComponentID,
        msocstate uStateID,
        msoccontext uContext,
        uint cpicmExclude,
        IMsoComponentManager** rgpicmExclude)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, msocstate, msoccontext, uint, IMsoComponentManager**, bool>)_lpVtbl[11])
                (pThis, dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude);
    }

    /// <inheritdoc cref="Interface.FInState(msocstate, void*)"/>
    internal BOOL FInState(
        msocstate uStateID,
        void* pvoid)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, msocstate, void*, BOOL>)_lpVtbl[12])(pThis, uStateID, pvoid);
    }

    /// <inheritdoc cref="Interface.FContinueIdle"/>
    internal BOOL FContinueIdle()
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, BOOL>)_lpVtbl[13])(pThis);
    }

    /// <inheritdoc cref="Interface.FPushMessageLoop(nuint, msoloop, void*)"/>
    internal BOOL FPushMessageLoop(
        nuint dwComponentID,
        msoloop uReason,
        void* pvLoopData)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, nuint, msoloop, void*, BOOL>)_lpVtbl[14])
                (pThis, dwComponentID, uReason, pvLoopData);
    }

    /// <inheritdoc cref="Interface.FCreateSubComponentManager(IUnknown*, IUnknown*, Guid*, void**)"/>
    internal BOOL FCreateSubComponentManager(
        IUnknown* punkOuter,
        IUnknown* punkServProv,
        Guid* riid,
        void** ppvObj)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, IUnknown*, IUnknown*, Guid*, void**, BOOL>)_lpVtbl[15])
                (pThis, punkOuter, punkServProv, riid, ppvObj);
    }

    /// <inheritdoc cref="Interface.FGetParentComponentManager(IMsoComponentManager**)"/>
    internal BOOL FGetParentComponentManager(
        IMsoComponentManager** ppicm)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, IMsoComponentManager**, BOOL>)_lpVtbl[16])(pThis, ppicm);
    }

    /// <inheritdoc cref="Interface.FGetActiveComponent(msogac, IMsoComponent**, MSOCRINFO*, uint)"/>
    internal BOOL FGetActiveComponent(
        msogac dwgac,
        IMsoComponent** ppic,
        MSOCRINFO* pcrinfo,
        uint dwReserved)
    {
        fixed (IMsoComponentManager* pThis = &this)
            return ((delegate* unmanaged[Stdcall]<IMsoComponentManager*, msogac, IMsoComponent**, MSOCRINFO*, uint, BOOL>)_lpVtbl[17])
                (pThis, dwgac, ppic, pcrinfo, dwReserved);
    }
}
