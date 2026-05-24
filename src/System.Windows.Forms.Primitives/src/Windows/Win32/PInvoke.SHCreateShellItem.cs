// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PrimitivesPInvokeExtensions
{
    extension(PInvoke)
    {
        /// <inheritdoc cref="PInvoke.SHCreateShellItem(ITEMIDLIST*, IShellFolder*, ITEMIDLIST*, IShellItem**)"/>
        public static unsafe IShellItem* SHCreateShellItem(string path)
        {
            IShellItem* ppsi = default;
            if (PInvoke.SHParseDisplayName(path, pbc: null, out ITEMIDLIST* ppidl, sfgaoIn: 0, psfgaoOut: out _).Succeeded)
            {
                // No parent specified
                PInvoke.SHCreateShellItem(pidlParent: null, psfParent: null, ppidl, &ppsi);
            }

            return ppsi;
        }
    }
}
