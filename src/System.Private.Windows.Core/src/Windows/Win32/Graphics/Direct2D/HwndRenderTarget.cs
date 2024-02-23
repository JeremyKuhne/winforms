// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Windows.Win32.Graphics.Direct2D;

/// <summary>
///  <see cref="HWND"/> render target.
/// </summary>
/// <devdoc>
///  <see href="https://learn.microsoft.com/windows/win32/Direct2D/supported-pixel-formats-and-alpha-modes#supported-formats-for-id2d1hwndrendertarget">
///   Supported Formats for ID2D1HwndRenderTarget
///  </see>
/// </devdoc>
internal unsafe class HwndRenderTarget : RenderTarget, IPointer<ID2D1HwndRenderTarget>
{
    public new ID2D1HwndRenderTarget* Pointer => (ID2D1HwndRenderTarget*)base.Pointer;

    private HwndRenderTarget(ID2D1HwndRenderTarget* renderTarget)
        : base((ID2D1RenderTarget*)renderTarget)
    {
    }

    public HwndRenderTarget(IHandle<HWND> hwnd, Size size) : this(CreateForWindow(hwnd, size)) { }

    private static ID2D1HwndRenderTarget* CreateForWindow(IHandle<HWND> window, Size size) =>
        CreateForWindow(
            window,
            new D2D_SIZE_U() { width = checked((uint)size.Width), height = checked((uint)size.Height) });

    private static ID2D1HwndRenderTarget* CreateForWindow(IHandle<HWND> window, D2D_SIZE_U size)
    {
        // DXGI_FORMAT_B8G8R8A8_UNORM is the recommended pixel format for HwndRenderTarget for performance reasons.
        // DXGI_FORMAT_UNKNOWN and DXGI_FORMAT_UNKNOWN give DXGI_FORMAT_B8G8R8A8_UNORM and D2D1_ALPHA_MODE_IGNORE.
        D2D1_RENDER_TARGET_PROPERTIES properties = default;
        D2D1_HWND_RENDER_TARGET_PROPERTIES hwndProperties = new()
        {
            hwnd = window.Handle,
            pixelSize = size
        };

        ID2D1HwndRenderTarget* renderTarget;
        Direct2dFactory.Shared.Pointer->CreateHwndRenderTarget(
            &properties,
            &hwndProperties,
            &renderTarget).ThrowOnFailure();

        GC.KeepAlive(window.Wrapper);

        return new HwndRenderTarget(renderTarget);
    }

    public void Resize(Size size)
    {
        Pointer->Resize((D2D_SIZE_U)size).ThrowOnFailure();
        GC.KeepAlive(this);
    }

    public static implicit operator ID2D1HwndRenderTarget*(HwndRenderTarget target) => target.Pointer;
}
