// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Graphics.Direct2D.Common;

internal partial struct D2D_POINT_2F
{
    public D2D_POINT_2F(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator D2D_POINT_2F(PointF value) =>
        new(value.X, value.Y);
}
