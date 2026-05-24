// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.InvalidateRgn(HWND, HRGN, BOOL)"/>
        public static BOOL InvalidateRgn<T>(T hWnd, HRGN hrgn, BOOL erase)
            where T : IHandle<HWND>
        {
            BOOL result = PInvoke.InvalidateRgn(hWnd.Handle, hrgn, erase);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
