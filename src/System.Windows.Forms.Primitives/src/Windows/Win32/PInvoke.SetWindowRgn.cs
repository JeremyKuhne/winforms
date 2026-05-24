// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.SetWindowRgn(HWND, HRGN, BOOL)"/>
        public static int SetWindowRgn<T>(T hwnd, HRGN hrgn, BOOL fRedraw)
            where T : IHandle<HWND>
        {
            int result = PInvoke.SetWindowRgn(hwnd.Handle, hrgn, fRedraw);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
