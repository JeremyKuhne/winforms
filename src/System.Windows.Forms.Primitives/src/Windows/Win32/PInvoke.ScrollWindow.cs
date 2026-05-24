// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.ScrollWindow(HWND, int, int, RECT*, RECT*)"/>
        public static unsafe BOOL ScrollWindow<T>(T hWnd, int XAmount, int YAmount, RECT* lpRect, RECT* rectClip)
            where T : IHandle<HWND>
        {
            BOOL result = PInvoke.ScrollWindow(hWnd.Handle, XAmount, YAmount, lpRect, rectClip);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
