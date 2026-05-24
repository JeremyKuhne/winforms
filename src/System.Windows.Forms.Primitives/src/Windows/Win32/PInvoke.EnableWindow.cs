// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.EnableWindow(HWND, BOOL)"/>
        public static BOOL EnableWindow<T>(T hWnd, BOOL bEnable)
            where T : IHandle<HWND>
        {
            BOOL result = PInvoke.EnableWindow(hWnd.Handle, bEnable);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
