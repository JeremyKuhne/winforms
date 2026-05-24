// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.EndDialog(HWND, nint)"/>
        public static BOOL EndDialog<T>(T hDlg, IntPtr nResult)
            where T : IHandle<HWND>
        {
            BOOL result = PInvoke.EndDialog(hDlg.Handle, nResult);
            GC.KeepAlive(hDlg.Wrapper);
            return result;
        }
    }
}
