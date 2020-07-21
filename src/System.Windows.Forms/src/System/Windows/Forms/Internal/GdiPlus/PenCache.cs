// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal sealed class PenCache : RefCache<Pen, Color, Color>
    {
        public PenCache(int softLimit = 40, int hardLimit = 60) : base(softLimit, hardLimit) { }

        protected override CacheEntry CreateEntry(Color key, bool cached) => new PenNodeData(key, cached);
        protected override bool IsMatch(Color key, CacheEntry data) => key == data.Data;

        private class PenNodeData : CacheEntry
        {
            private readonly Pen _pen;
            public PenNodeData(Color color, bool cached) : base(color, cached) => _pen = new Pen(color);
            public override Pen Object => _pen;
        }
    }
}
