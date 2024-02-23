// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe class Bitmap : Image, IPointer<ID2D1Bitmap>
{
    public unsafe new ID2D1Bitmap* Pointer => (ID2D1Bitmap*)base.Pointer;

    public Bitmap(ID2D1Bitmap* bitmap) : base((ID2D1Image*)bitmap)
    {
    }

    public static implicit operator ID2D1Bitmap*(Bitmap bitmap) => bitmap.Pointer;
}
