// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace System.Drawing;

internal static class CachedBitmapCache
{
    private static readonly ConditionalWeakTable<Bitmap, Bitmap> s_cache = [];

    public static Bitmap? GetCachedBitmap(Graphics graphics, Image image)
    {
        if (image is not Bitmap bitmap || bitmap.PixelFormat == PixelFormat.Format32bppArgb)
        {
            return null;
        }

        Bitmap? cachedBitmap = null;
        while (cachedBitmap is null)
        {
            if (!s_cache.TryGetValue(bitmap, out cachedBitmap))
            {
                cachedBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                if (!s_cache.TryAdd(bitmap, cachedBitmap))
                {
                    // Already have one, try again.
                    cachedBitmap.Dispose();
                }
            }
        }

        return cachedBitmap;
    }
}
