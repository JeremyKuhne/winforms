// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe class Brush : Resource, IPointer<ID2D1Brush>
{
    public unsafe new ID2D1Brush* Pointer => (ID2D1Brush*)base.Pointer;

    public Brush(ID2D1Brush* brush) : base((ID2D1Resource*)brush)
    {
    }

    public static implicit operator ID2D1Brush*(Brush brush) => brush.Pointer;
}
