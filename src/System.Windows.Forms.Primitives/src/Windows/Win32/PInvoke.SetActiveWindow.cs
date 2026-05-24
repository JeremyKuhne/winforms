// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.SetActiveWindow(HWND)"/>
        public static HWND SetActiveWindow<T>(T hWnd) where T : IHandle<HWND>
        {
            HWND result = PInvoke.SetActiveWindow(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
