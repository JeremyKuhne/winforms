// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.GetDlgItem(HWND, int)"/>
        public static HWND GetDlgItem<T>(T hDlg, int nIDDlgItem)
            where T : IHandle<HWND>
        {
            HWND result = PInvoke.GetDlgItem(hDlg.Handle, nIDDlgItem);
            GC.KeepAlive(hDlg.Wrapper);
            return result;
        }
    }
}
