// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

using Windows.Win32.Graphics.Direct2D.Common;

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe class SolidColorBrush : Brush, IPointer<ID2D1SolidColorBrush>
{
    public unsafe new ID2D1SolidColorBrush* Pointer => (ID2D1SolidColorBrush*)base.Pointer;

    public SolidColorBrush(ID2D1SolidColorBrush* brush) : base((ID2D1Brush*)brush)
    {
    }

    public static implicit operator ID2D1SolidColorBrush*(SolidColorBrush brush) => brush.Pointer;

    public Color Color
    {
        get
        {
            D2D1_COLOR_F color = Pointer->GetColorHack();
            return (Color)color;
        }
        set
        {
            D2D1_COLOR_F color = (D2D1_COLOR_F)value;
            Pointer->SetColor(&color);
        }
    }
}
