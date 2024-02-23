// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Graphics.Direct2D.Common;

internal partial struct D2D1_COLOR_F
{
    public D2D1_COLOR_F(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static explicit operator D2D1_COLOR_F(Color value) =>
        new(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);

    public static explicit operator Color(D2D1_COLOR_F value) =>
        Color.FromArgb(
            (int)(value.a * 255.0f),
            (int)(value.r * 255.0f),
            (int)(value.g * 255.0f),
            (int)(value.b * 255.0f));
}
