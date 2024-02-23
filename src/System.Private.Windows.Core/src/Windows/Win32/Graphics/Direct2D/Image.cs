// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe class Image : Resource, IPointer<ID2D1Image>
{
    public unsafe new ID2D1Image* Pointer => (ID2D1Image*)base.Pointer;

    public Image(ID2D1Image* image) : base((ID2D1Resource*)image)
    {
    }

    public static implicit operator ID2D1Image*(Image image) => image.Pointer;
}
