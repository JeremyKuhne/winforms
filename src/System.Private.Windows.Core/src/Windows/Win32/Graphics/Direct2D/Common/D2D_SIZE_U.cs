// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Graphics.Direct2D.Common;

internal partial struct D2D_SIZE_U
{
    public D2D_SIZE_U(uint width, uint height)
    {
        this.width = width;
        this.height = height;
    }

    public static explicit operator D2D_SIZE_U(Size value) =>
        new(checked((uint)value.Width), checked((uint)value.Height));
}
