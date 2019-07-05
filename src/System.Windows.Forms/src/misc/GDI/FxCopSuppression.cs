// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.CreateDC(System.String,System.String,System.String,System.Runtime.InteropServices.HandleRef):System.Windows.Forms.Internal.DeviceContext")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.CreateIC(System.String,System.String,System.String,System.Runtime.InteropServices.HandleRef):System.Windows.Forms.Internal.DeviceContext")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.get_DeviceContextType():System.Windows.Forms.Internal.DeviceContextType")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.get_DpiX():System.Int32")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.TranslateTransform(System.Single,System.Single):System.Void")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.ToString():System.String")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.DrawText(System.String,System.Drawing.Point):System.Void")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.DrawText(System.String,System.Drawing.Point,System.Drawing.Color):System.Void")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.get_Font():System.Windows.Forms.Internal.WindowsFont")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.FromHdc(System.IntPtr):System.Windows.Forms.Internal.WindowsFont")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.FromHfont(System.IntPtr):System.Windows.Forms.Internal.WindowsFont")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.FromHfont(System.IntPtr,System.Boolean):System.Windows.Forms.Internal.WindowsFont")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.get_Italic():System.Boolean")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.FromHwnd(System.IntPtr):System.Windows.Forms.Internal.WindowsGraphics")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.get_GraphicsMode():System.Windows.Forms.Internal.DeviceContextGraphicsMode")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Internal.DeviceContext.SetGraphicsMode(System.Windows.Forms.Internal.DeviceContextGraphicsMode):System.Windows.Forms.Internal.DeviceContextGraphicsMode")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Forms.Control+ImeModeConversion.get_ChineseTable():System.Windows.Forms.ImeMode[]")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphicsCacheManager..cctor()")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.FillRectangle(System.Windows.Forms.Internal.WindowsBrush,System.Int32,System.Int32,System.Int32,System.Int32):System.Void")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsBrush.FromBrush(System.Drawing.Brush):System.Windows.Forms.Internal.WindowsBrush")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.get_TextPadding():System.Windows.Forms.Internal.TextPaddingOptions")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.set_TextPadding(System.Windows.Forms.Internal.TextPaddingOptions):System.Void")]

[assembly: SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.Dispose():System.Void")]

[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.GetOverhangPadding(System.Windows.Forms.Internal.WindowsFont,System.Boolean):System.Int32")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.ToString():System.String")]

[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphics.Dispose(System.Boolean):System.Void")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphicsCacheManager.OnApplicationExit(System.Object,System.EventArgs):System.Void")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsGraphicsCacheManager.OnThreadExit(System.Object,System.EventArgs):System.Void")]

[assembly: SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsBrush.nativeHandle")]
[assembly: SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsFont.hFont")]
[assembly: SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsPen.nativeHandle")]
[assembly: SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "System.Windows.Forms.Internal.WindowsBitmap.nativeHandle")]

// These are debug calls
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control.get_CanEnableIme():System.Boolean")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control.ImeModeRestricted(System.Boolean,System.Boolean):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control.WmImeNotify(System.Windows.Forms.Message&):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control+ImeContext.SetOpenStatus(System.Boolean,System.IntPtr):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control+ImeContext.SetImeStatus(System.Windows.Forms.ImeMode,System.IntPtr):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control+ImeContext.TraceImeStatus(System.IntPtr):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "System.Windows.Forms.Control+ImeContext.TraceImeStatus(System.IntPtr):System.Void")]

