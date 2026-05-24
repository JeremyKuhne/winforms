// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.ScrollWindowEx(HWND, int, int, RECT*, RECT*, HRGN, RECT*, SCROLL_WINDOW_FLAGS)"/>
        public static unsafe int ScrollWindowEx<T>(
            T hWnd,
            int dx,
            int dy,
            RECT* prcScroll,
            RECT* prcClip,
            HRGN hrgnUpdate,
            RECT* prcUpdate,
            SCROLL_WINDOW_FLAGS flags) where T : IHandle<HWND>
        {
            int result = PInvoke.ScrollWindowEx(hWnd.Handle, dx, dy, prcScroll, prcClip, hrgnUpdate, prcUpdate, flags);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
