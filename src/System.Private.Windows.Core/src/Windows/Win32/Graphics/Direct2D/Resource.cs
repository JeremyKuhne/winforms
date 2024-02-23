// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe class Resource : DirectDrawBase<ID2D1Resource>, IPointer<ID2D1Resource>
{
    public Resource(ID2D1Resource* resource) : base(resource)
    {
    }
}
