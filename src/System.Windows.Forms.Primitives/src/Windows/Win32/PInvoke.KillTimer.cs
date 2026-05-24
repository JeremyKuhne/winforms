// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.KillTimer(HWND, nuint)"/>
        public static BOOL KillTimer<T>(T hWnd, IntPtr uIDEvent)
            where T : IHandle<HWND>
        {
            BOOL result = PInvoke.KillTimer(hWnd.Handle, uIDEvent);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
