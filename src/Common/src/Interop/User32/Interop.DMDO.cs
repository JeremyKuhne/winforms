// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum DMDO : uint
        {
            DEFAULT   = 0,
            _90       = 1,
            _180      = 2,
            _270      = 3
        }
    }
}
