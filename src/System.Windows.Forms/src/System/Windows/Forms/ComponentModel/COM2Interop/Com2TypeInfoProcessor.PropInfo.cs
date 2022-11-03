// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal static partial class Com2TypeInfoProcessor
    {
        private class PropertyInfo
        {
            public enum ReadOnlyState
            {
                Unknown,
                True,
                False
            }

#pragma warning disable IDE0036 // required must come first
            required public string Name { get; init; }
#pragma warning restore IDE0036

            public int DispId { get; set; } = PInvoke.DISPID_UNKNOWN;

            /// <summary>
            ///  The managed <see cref="Type"/> for the property.
            /// </summary>
            public Type? ValueType { get; set; }

            public List<Attribute> Attributes { get; } = new();

            public ReadOnlyState ReadOnly { get; set; } = ReadOnlyState.Unknown;

            public bool IsDefault { get; set; }

            /// <summary>
            ///  For <see cref="VARIANT"/>s of type <see cref="VARENUM.VT_DISPATCH"/> or <see cref="VARENUM.VT_UNKNOWN"/>,
            ///  this is the Guid as returned from <see cref="ITypeInfo"/>.
            /// </summary>
            public Guid? TypeGuid { get; set; }

            public bool NonBrowsable { get; set; }

            public int Index { get; set; }

            public override int GetHashCode() => Name?.GetHashCode() ?? base.GetHashCode();
        }
    }
}
