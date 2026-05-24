// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.SHAutoComplete(HWND, SHELL_AUTOCOMPLETE_FLAGS)"/>
        public static HRESULT SHAutoComplete<T>(T hwndEdit, SHELL_AUTOCOMPLETE_FLAGS flags) where T : IHandle<HWND>
        {
            HRESULT result = PInvoke.SHAutoComplete(hwndEdit.Handle, flags);
            GC.KeepAlive(hwndEdit.Wrapper);
            return result;
        }
    }
}
