// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe partial struct ID2D1RenderTarget
{
    public D2D_SIZE_F GetSizeHack()
    {
        return ((delegate* unmanaged[Stdcall, MemberFunction]<ID2D1RenderTarget*, D2D_SIZE_F>)lpVtbl[53])((ID2D1RenderTarget*)Unsafe.AsPointer(ref this));
    }
}
