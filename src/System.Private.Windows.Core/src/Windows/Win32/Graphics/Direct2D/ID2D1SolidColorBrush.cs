// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.Graphics.Direct2D.Common;

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe partial struct ID2D1SolidColorBrush
{
    public D2D1_COLOR_F GetColorHack()
    {
        return ((delegate* unmanaged[Stdcall, MemberFunction]<ID2D1SolidColorBrush*, D2D1_COLOR_F>)lpVtbl[9])((ID2D1SolidColorBrush*)Unsafe.AsPointer(ref this));
    }
}
