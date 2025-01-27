// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Private.Windows.Graphics;

internal interface IBitmap
{
    HBITMAP GetHbitmap();
    Size Size { get; }
}
