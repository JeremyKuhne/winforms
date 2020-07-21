// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Cache of GDI+ objects to reuse commonly created items.
    /// </summary>
    internal static class GdiPlusCache
    {
        [ThreadStatic]
        private static PenCache? s_penCache;
        [ThreadStatic]
        private static SolidBrushCache? s_brushCache;

        private static PenCache PenCache => s_penCache ??= new PenCache(softLimit: 40, hardLimit: 60);
        private static SolidBrushCache BrushCache => s_brushCache ??= new SolidBrushCache(softLimit: 30, hardLimit: 50);

        internal static PenCache.Scope GetPen(Color color)
        {
            if (color.IsKnownColor)
            {
                Pen? pen = color.IsSystemColor
                    ? SystemPens.FromSystemColor(color)
                    : PenFromKnownColor(color.ToKnownColor());

                if (pen != null)
                {
                    return new PenCache.Scope(pen);
                }
            }

            return PenCache.GetEntry(color);
        }

        internal static SolidBrushCache.Scope GetSolidBrush(Color color)
        {
            if (color.IsKnownColor)
            {
                SolidBrush? solidBrush = color.IsSystemColor
                    ? (SolidBrush?)SystemBrushes.FromSystemColor(color)
                    : (SolidBrush?)BrushFromKnownColor(color.ToKnownColor());

                if (solidBrush != null)
                {
                    return new SolidBrushCache.Scope(solidBrush);
                }
            }

            return BrushCache.GetEntry(color);
        }

        internal static PenCache.Scope GetCachedPen(this Color color, Graphics graphics)
        {
            // It is very rare that the color will change- we should look at removing this.
            Color newColor = graphics.GetNearestColor(color);
            return GetPen(newColor.ToArgb() == color.ToArgb() ? color : newColor);
        }

        internal static PenCache.Scope GetCachedPen(this Color color) => GetPen(color);
        internal static SolidBrushCache.Scope GetCachedSolidBrush(this Color color) => GetSolidBrush(color);

        internal static Brush? BrushFromKnownColor(KnownColor color) => color switch
        {
            // Starting with the expected most common
            KnownColor.Black => Brushes.Black,
            KnownColor.White => Brushes.White,
            KnownColor.Gray => Brushes.Gray,
            KnownColor.Red => Brushes.Red,
            KnownColor.Green => Brushes.Green,
            KnownColor.Blue => Brushes.Blue,
            KnownColor.Yellow => Brushes.Yellow,
            KnownColor.Brown => Brushes.Brown,
            KnownColor.LightGray => Brushes.LightGray,
            KnownColor.LightGreen => Brushes.LightGreen,
            KnownColor.LightBlue => Brushes.LightBlue,
            KnownColor.LightYellow => Brushes.LightYellow,
            KnownColor.DarkGray => Brushes.DarkGray,
            KnownColor.DarkRed => Brushes.DarkRed,
            KnownColor.DarkGreen => Brushes.DarkGreen,
            KnownColor.DarkBlue => Brushes.DarkBlue,
            KnownColor.Transparent => Brushes.Transparent,

            // Flip less common to secondary method so first can get inlined...
#if DEBUG
            KnownColor.AliceBlue => throw new NotImplementedException(),
            KnownColor.AntiqueWhite => throw new NotImplementedException(),
            KnownColor.Aqua => throw new NotImplementedException(),
            KnownColor.Aquamarine => throw new NotImplementedException(),
            KnownColor.Azure => throw new NotImplementedException(),
            KnownColor.Beige => throw new NotImplementedException(),
            KnownColor.Bisque => throw new NotImplementedException(),
            KnownColor.BlanchedAlmond => throw new NotImplementedException(),
            KnownColor.BlueViolet => throw new NotImplementedException(),
            KnownColor.BurlyWood => throw new NotImplementedException(),
            KnownColor.CadetBlue => throw new NotImplementedException(),
            KnownColor.Chartreuse => throw new NotImplementedException(),
            KnownColor.Chocolate => throw new NotImplementedException(),
            KnownColor.Coral => throw new NotImplementedException(),
            KnownColor.CornflowerBlue => throw new NotImplementedException(),
            KnownColor.Cornsilk => throw new NotImplementedException(),
            KnownColor.Crimson => throw new NotImplementedException(),
            KnownColor.Cyan => throw new NotImplementedException(),
            KnownColor.DarkCyan => throw new NotImplementedException(),
            KnownColor.DarkGoldenrod => throw new NotImplementedException(),
            KnownColor.DarkKhaki => throw new NotImplementedException(),
            KnownColor.DarkMagenta => throw new NotImplementedException(),
            KnownColor.DarkOliveGreen => throw new NotImplementedException(),
            KnownColor.DarkOrange => throw new NotImplementedException(),
            KnownColor.DarkOrchid => throw new NotImplementedException(),
            KnownColor.DarkSalmon => throw new NotImplementedException(),
            KnownColor.DarkSeaGreen => throw new NotImplementedException(),
            KnownColor.DarkSlateBlue => throw new NotImplementedException(),
            KnownColor.DarkSlateGray => throw new NotImplementedException(),
            KnownColor.DarkTurquoise => throw new NotImplementedException(),
            KnownColor.DarkViolet => throw new NotImplementedException(),
            KnownColor.DeepPink => throw new NotImplementedException(),
            KnownColor.DeepSkyBlue => throw new NotImplementedException(),
            KnownColor.DimGray => throw new NotImplementedException(),
            KnownColor.DodgerBlue => throw new NotImplementedException(),
            KnownColor.Firebrick => throw new NotImplementedException(),
            KnownColor.FloralWhite => throw new NotImplementedException(),
            KnownColor.ForestGreen => throw new NotImplementedException(),
            KnownColor.Fuchsia => throw new NotImplementedException(),
            KnownColor.Gainsboro => throw new NotImplementedException(),
            KnownColor.GhostWhite => throw new NotImplementedException(),
            KnownColor.Gold => throw new NotImplementedException(),
            KnownColor.Goldenrod => throw new NotImplementedException(),
            KnownColor.GreenYellow => throw new NotImplementedException(),
            KnownColor.Honeydew => throw new NotImplementedException(),
            KnownColor.HotPink => throw new NotImplementedException(),
            KnownColor.IndianRed => throw new NotImplementedException(),
            KnownColor.Indigo => throw new NotImplementedException(),
            KnownColor.Ivory => throw new NotImplementedException(),
            KnownColor.Khaki => throw new NotImplementedException(),
            KnownColor.Lavender => throw new NotImplementedException(),
            KnownColor.LavenderBlush => throw new NotImplementedException(),
            KnownColor.LawnGreen => throw new NotImplementedException(),
            KnownColor.LemonChiffon => throw new NotImplementedException(),
            KnownColor.LightCoral => throw new NotImplementedException(),
            KnownColor.LightCyan => throw new NotImplementedException(),
            KnownColor.LightGoldenrodYellow => throw new NotImplementedException(),
            KnownColor.LightPink => throw new NotImplementedException(),
            KnownColor.LightSalmon => throw new NotImplementedException(),
            KnownColor.LightSeaGreen => throw new NotImplementedException(),
            KnownColor.LightSkyBlue => throw new NotImplementedException(),
            KnownColor.LightSlateGray => throw new NotImplementedException(),
            KnownColor.LightSteelBlue => throw new NotImplementedException(),
            KnownColor.Lime => throw new NotImplementedException(),
            KnownColor.LimeGreen => throw new NotImplementedException(),
            KnownColor.Linen => throw new NotImplementedException(),
            KnownColor.Magenta => throw new NotImplementedException(),
            KnownColor.Maroon => throw new NotImplementedException(),
            KnownColor.MediumAquamarine => throw new NotImplementedException(),
            KnownColor.MediumBlue => throw new NotImplementedException(),
            KnownColor.MediumOrchid => throw new NotImplementedException(),
            KnownColor.MediumPurple => throw new NotImplementedException(),
            KnownColor.MediumSeaGreen => throw new NotImplementedException(),
            KnownColor.MediumSlateBlue => throw new NotImplementedException(),
            KnownColor.MediumSpringGreen => throw new NotImplementedException(),
            KnownColor.MediumTurquoise => throw new NotImplementedException(),
            KnownColor.MediumVioletRed => throw new NotImplementedException(),
            KnownColor.MidnightBlue => throw new NotImplementedException(),
            KnownColor.MintCream => throw new NotImplementedException(),
            KnownColor.MistyRose => throw new NotImplementedException(),
            KnownColor.Moccasin => throw new NotImplementedException(),
            KnownColor.NavajoWhite => throw new NotImplementedException(),
            KnownColor.Navy => throw new NotImplementedException(),
            KnownColor.OldLace => throw new NotImplementedException(),
            KnownColor.Olive => throw new NotImplementedException(),
            KnownColor.OliveDrab => throw new NotImplementedException(),
            KnownColor.Orange => throw new NotImplementedException(),
            KnownColor.OrangeRed => throw new NotImplementedException(),
            KnownColor.Orchid => throw new NotImplementedException(),
            KnownColor.PaleGoldenrod => throw new NotImplementedException(),
            KnownColor.PaleGreen => throw new NotImplementedException(),
            KnownColor.PaleTurquoise => throw new NotImplementedException(),
            KnownColor.PaleVioletRed => throw new NotImplementedException(),
            KnownColor.PapayaWhip => throw new NotImplementedException(),
            KnownColor.PeachPuff => throw new NotImplementedException(),
            KnownColor.Peru => throw new NotImplementedException(),
            KnownColor.Pink => throw new NotImplementedException(),
            KnownColor.Plum => throw new NotImplementedException(),
            KnownColor.PowderBlue => throw new NotImplementedException(),
            KnownColor.Purple => throw new NotImplementedException(),
            KnownColor.RosyBrown => throw new NotImplementedException(),
            KnownColor.RoyalBlue => throw new NotImplementedException(),
            KnownColor.SaddleBrown => throw new NotImplementedException(),
            KnownColor.Salmon => throw new NotImplementedException(),
            KnownColor.SandyBrown => throw new NotImplementedException(),
            KnownColor.SeaGreen => throw new NotImplementedException(),
            KnownColor.SeaShell => throw new NotImplementedException(),
            KnownColor.Sienna => throw new NotImplementedException(),
            KnownColor.Silver => throw new NotImplementedException(),
            KnownColor.SkyBlue => throw new NotImplementedException(),
            KnownColor.SlateBlue => throw new NotImplementedException(),
            KnownColor.SlateGray => throw new NotImplementedException(),
            KnownColor.Snow => throw new NotImplementedException(),
            KnownColor.SpringGreen => throw new NotImplementedException(),
            KnownColor.SteelBlue => throw new NotImplementedException(),
            KnownColor.Tan => throw new NotImplementedException(),
            KnownColor.Teal => throw new NotImplementedException(),
            KnownColor.Thistle => throw new NotImplementedException(),
            KnownColor.Tomato => throw new NotImplementedException(),
            KnownColor.Turquoise => throw new NotImplementedException(),
            KnownColor.Violet => throw new NotImplementedException(),
            KnownColor.Wheat => throw new NotImplementedException(),
            KnownColor.WhiteSmoke => throw new NotImplementedException(),
            KnownColor.YellowGreen => throw new NotImplementedException(),
#endif
            _ => null
        };

        internal static Pen? PenFromKnownColor(KnownColor color) => color switch
        {
            // Starting with the expected most common
            KnownColor.Black => Pens.Black,
            KnownColor.White => Pens.White,
            KnownColor.Gray => Pens.Gray,
            KnownColor.Red => Pens.Red,
            KnownColor.Green => Pens.Green,
            KnownColor.Blue => Pens.Blue,
            KnownColor.Yellow => Pens.Yellow,
            KnownColor.Brown => Pens.Brown,
            KnownColor.LightGray => Pens.LightGray,
            KnownColor.LightGreen => Pens.LightGreen,
            KnownColor.LightBlue => Pens.LightBlue,
            KnownColor.LightYellow => Pens.LightYellow,
            KnownColor.DarkGray => Pens.DarkGray,
            KnownColor.DarkRed => Pens.DarkRed,
            KnownColor.DarkGreen => Pens.DarkGreen,
            KnownColor.DarkBlue => Pens.DarkBlue,
            KnownColor.Transparent => Pens.Transparent,

            // Flip less common to secondary method so first can get inlined...
#if DEBUG
            KnownColor.AliceBlue => throw new NotImplementedException(),
            KnownColor.AntiqueWhite => throw new NotImplementedException(),
            KnownColor.Aqua => throw new NotImplementedException(),
            KnownColor.Aquamarine => throw new NotImplementedException(),
            KnownColor.Azure => throw new NotImplementedException(),
            KnownColor.Beige => throw new NotImplementedException(),
            KnownColor.Bisque => throw new NotImplementedException(),
            KnownColor.BlanchedAlmond => throw new NotImplementedException(),
            KnownColor.BlueViolet => throw new NotImplementedException(),
            KnownColor.BurlyWood => throw new NotImplementedException(),
            KnownColor.CadetBlue => throw new NotImplementedException(),
            KnownColor.Chartreuse => throw new NotImplementedException(),
            KnownColor.Chocolate => throw new NotImplementedException(),
            KnownColor.Coral => throw new NotImplementedException(),
            KnownColor.CornflowerBlue => throw new NotImplementedException(),
            KnownColor.Cornsilk => throw new NotImplementedException(),
            KnownColor.Crimson => throw new NotImplementedException(),
            KnownColor.Cyan => throw new NotImplementedException(),
            KnownColor.DarkCyan => throw new NotImplementedException(),
            KnownColor.DarkGoldenrod => throw new NotImplementedException(),
            KnownColor.DarkKhaki => throw new NotImplementedException(),
            KnownColor.DarkMagenta => throw new NotImplementedException(),
            KnownColor.DarkOliveGreen => throw new NotImplementedException(),
            KnownColor.DarkOrange => throw new NotImplementedException(),
            KnownColor.DarkOrchid => throw new NotImplementedException(),
            KnownColor.DarkSalmon => throw new NotImplementedException(),
            KnownColor.DarkSeaGreen => throw new NotImplementedException(),
            KnownColor.DarkSlateBlue => throw new NotImplementedException(),
            KnownColor.DarkSlateGray => throw new NotImplementedException(),
            KnownColor.DarkTurquoise => throw new NotImplementedException(),
            KnownColor.DarkViolet => throw new NotImplementedException(),
            KnownColor.DeepPink => throw new NotImplementedException(),
            KnownColor.DeepSkyBlue => throw new NotImplementedException(),
            KnownColor.DimGray => throw new NotImplementedException(),
            KnownColor.DodgerBlue => throw new NotImplementedException(),
            KnownColor.Firebrick => throw new NotImplementedException(),
            KnownColor.FloralWhite => throw new NotImplementedException(),
            KnownColor.ForestGreen => throw new NotImplementedException(),
            KnownColor.Fuchsia => throw new NotImplementedException(),
            KnownColor.Gainsboro => throw new NotImplementedException(),
            KnownColor.GhostWhite => throw new NotImplementedException(),
            KnownColor.Gold => throw new NotImplementedException(),
            KnownColor.Goldenrod => throw new NotImplementedException(),
            KnownColor.GreenYellow => throw new NotImplementedException(),
            KnownColor.Honeydew => throw new NotImplementedException(),
            KnownColor.HotPink => throw new NotImplementedException(),
            KnownColor.IndianRed => throw new NotImplementedException(),
            KnownColor.Indigo => throw new NotImplementedException(),
            KnownColor.Ivory => throw new NotImplementedException(),
            KnownColor.Khaki => throw new NotImplementedException(),
            KnownColor.Lavender => throw new NotImplementedException(),
            KnownColor.LavenderBlush => throw new NotImplementedException(),
            KnownColor.LawnGreen => throw new NotImplementedException(),
            KnownColor.LemonChiffon => throw new NotImplementedException(),
            KnownColor.LightCoral => throw new NotImplementedException(),
            KnownColor.LightCyan => throw new NotImplementedException(),
            KnownColor.LightGoldenrodYellow => throw new NotImplementedException(),
            KnownColor.LightPink => throw new NotImplementedException(),
            KnownColor.LightSalmon => throw new NotImplementedException(),
            KnownColor.LightSeaGreen => throw new NotImplementedException(),
            KnownColor.LightSkyBlue => throw new NotImplementedException(),
            KnownColor.LightSlateGray => throw new NotImplementedException(),
            KnownColor.LightSteelBlue => throw new NotImplementedException(),
            KnownColor.Lime => throw new NotImplementedException(),
            KnownColor.LimeGreen => throw new NotImplementedException(),
            KnownColor.Linen => throw new NotImplementedException(),
            KnownColor.Magenta => throw new NotImplementedException(),
            KnownColor.Maroon => throw new NotImplementedException(),
            KnownColor.MediumAquamarine => throw new NotImplementedException(),
            KnownColor.MediumBlue => throw new NotImplementedException(),
            KnownColor.MediumOrchid => throw new NotImplementedException(),
            KnownColor.MediumPurple => throw new NotImplementedException(),
            KnownColor.MediumSeaGreen => throw new NotImplementedException(),
            KnownColor.MediumSlateBlue => throw new NotImplementedException(),
            KnownColor.MediumSpringGreen => throw new NotImplementedException(),
            KnownColor.MediumTurquoise => throw new NotImplementedException(),
            KnownColor.MediumVioletRed => throw new NotImplementedException(),
            KnownColor.MidnightBlue => throw new NotImplementedException(),
            KnownColor.MintCream => throw new NotImplementedException(),
            KnownColor.MistyRose => throw new NotImplementedException(),
            KnownColor.Moccasin => throw new NotImplementedException(),
            KnownColor.NavajoWhite => throw new NotImplementedException(),
            KnownColor.Navy => throw new NotImplementedException(),
            KnownColor.OldLace => throw new NotImplementedException(),
            KnownColor.Olive => throw new NotImplementedException(),
            KnownColor.OliveDrab => throw new NotImplementedException(),
            KnownColor.Orange => throw new NotImplementedException(),
            KnownColor.OrangeRed => throw new NotImplementedException(),
            KnownColor.Orchid => throw new NotImplementedException(),
            KnownColor.PaleGoldenrod => throw new NotImplementedException(),
            KnownColor.PaleGreen => throw new NotImplementedException(),
            KnownColor.PaleTurquoise => throw new NotImplementedException(),
            KnownColor.PaleVioletRed => throw new NotImplementedException(),
            KnownColor.PapayaWhip => throw new NotImplementedException(),
            KnownColor.PeachPuff => throw new NotImplementedException(),
            KnownColor.Peru => throw new NotImplementedException(),
            KnownColor.Pink => throw new NotImplementedException(),
            KnownColor.Plum => throw new NotImplementedException(),
            KnownColor.PowderBlue => throw new NotImplementedException(),
            KnownColor.Purple => throw new NotImplementedException(),
            KnownColor.RosyBrown => throw new NotImplementedException(),
            KnownColor.RoyalBlue => throw new NotImplementedException(),
            KnownColor.SaddleBrown => throw new NotImplementedException(),
            KnownColor.Salmon => throw new NotImplementedException(),
            KnownColor.SandyBrown => throw new NotImplementedException(),
            KnownColor.SeaGreen => throw new NotImplementedException(),
            KnownColor.SeaShell => throw new NotImplementedException(),
            KnownColor.Sienna => throw new NotImplementedException(),
            KnownColor.Silver => throw new NotImplementedException(),
            KnownColor.SkyBlue => throw new NotImplementedException(),
            KnownColor.SlateBlue => throw new NotImplementedException(),
            KnownColor.SlateGray => throw new NotImplementedException(),
            KnownColor.Snow => throw new NotImplementedException(),
            KnownColor.SpringGreen => throw new NotImplementedException(),
            KnownColor.SteelBlue => throw new NotImplementedException(),
            KnownColor.Tan => throw new NotImplementedException(),
            KnownColor.Teal => throw new NotImplementedException(),
            KnownColor.Thistle => throw new NotImplementedException(),
            KnownColor.Tomato => throw new NotImplementedException(),
            KnownColor.Turquoise => throw new NotImplementedException(),
            KnownColor.Violet => throw new NotImplementedException(),
            KnownColor.Wheat => throw new NotImplementedException(),
            KnownColor.WhiteSmoke => throw new NotImplementedException(),
            KnownColor.YellowGreen => throw new NotImplementedException(),
#endif
            _ => null
        };
    }
}
