// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.GetWindow(HWND, GET_WINDOW_CMD)"/>
        public static HWND GetWindow<T>(T hWnd, GET_WINDOW_CMD uCmd) where T : IHandle<HWND>
        {
            HWND result = PInvoke.GetWindow(hWnd.Handle, uCmd);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
