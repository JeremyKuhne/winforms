﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal class SolidBrushCache : RefCache<SolidBrush, Color, Color>
    {
        public SolidBrushCache(int softLimit = 30, int hardLimit = 50) : base(softLimit, hardLimit) { }

        protected override CacheEntry CreateEntry(Color key, bool cached) => new SolidBrushData(key, cached);
        protected override bool IsMatch(Color key, CacheEntry data) => key == data.Data;

        private class SolidBrushData : CacheEntry
        {
            private readonly SolidBrush _brush;
            public SolidBrushData(Color color, bool cached) : base(color, cached) => _brush = new SolidBrush(color);
            public override SolidBrush Object => _brush;
        }
    }
}
