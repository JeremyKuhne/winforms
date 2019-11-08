﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [DllImport(Libraries.Comctl32, ExactSpelling = true, EntryPoint = "ImageList_Duplicate")]
            public static extern IntPtr Duplicate(IntPtr himl);

            public static IntPtr Duplicate(IHandle himl)
            {
                IntPtr result = Duplicate(himl.Handle);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
