// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.Layout;
using Xunit;

namespace System.Windows.Forms.ButtonInternal.Tests
{
    public class ButtonFlatInternalTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void PaintFlatLayout_UpAndCheckDoNotChangePreferredSize(int size)
        {
            var buttonFlatStatics = typeof(ButtonFlatAdapter).TestAccessor().Dynamic;

            ButtonBaseAdapter.LayoutOptions layout = buttonFlatStatics.PaintFlatLayout(
                up: false,
                check: true,
                borderSize: size);

            Size preferredSize = layout.GetPreferredSizeCore(LayoutUtils.MaxSize);

            Assert.Equal(
                preferredSize,
                buttonFlatStatics.PaintFlatLayout(up: false, check: false, borderSize: size)
                    .GetPreferredSizeCore(LayoutUtils.MaxSize));
            Assert.Equal(
                preferredSize,
                buttonFlatStatics.PaintFlatLayout(up: true, check: false, borderSize: size)
                    .GetPreferredSizeCore(LayoutUtils.MaxSize));
            Assert.Equal(
                preferredSize,
                buttonFlatStatics.PaintFlatLayout(up: true, check: true, borderSize: size)
                    .GetPreferredSizeCore(LayoutUtils.MaxSize));
        }
    }
}
