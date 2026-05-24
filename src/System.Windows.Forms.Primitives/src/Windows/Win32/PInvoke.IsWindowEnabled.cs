// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.IsWindowEnabled(HWND)"/>
        public static BOOL IsWindowEnabled<T>(T hWnd) where T : IHandle<HWND>
        {
            BOOL result = PInvoke.IsWindowEnabled(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
