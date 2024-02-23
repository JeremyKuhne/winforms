// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Direct2d = Windows.Win32.Graphics.Direct2D;

namespace System.Drawing;

internal static class CachedBitmapCache
{
    private static readonly ConditionalWeakTable<Bitmap, Direct2d.Bitmap> s_cache = [];

    public static Direct2d.Bitmap? GetCachedBitmap(Direct2d.RenderTarget renderTarget, Image image)
    {
        if (image is not Bitmap bitmap)
        {
            return null;
        }

        Direct2d.Bitmap? cachedBitmap = null;
        while (cachedBitmap is null)
        {
            if (!s_cache.TryGetValue(bitmap, out cachedBitmap))
            {
                cachedBitmap = renderTarget.CreateBitmapFromGdiPlusBitmap(bitmap);
                if (!s_cache.TryAdd(bitmap, cachedBitmap))
                {
                    // Already have one, try again.
                    cachedBitmap.Dispose();
                }
            }
        }

        return cachedBitmap;
    }

    public static void ClearEntry(Image image)
    {
        if (image is not Bitmap bitmap)
        {
            return;
        }

        s_cache.Remove(bitmap);
    }
}
