// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.NotifyWinEvent(uint, HWND, int, int)"/>
        public static void NotifyWinEvent<T>(uint @event, T hwnd, int idObject, int idChild)
            where T : IHandle<HWND>
        {
            PInvoke.NotifyWinEvent(@event, hwnd.Handle, idObject, idChild);
            GC.KeepAlive(hwnd.Wrapper);
        }
    }
}
