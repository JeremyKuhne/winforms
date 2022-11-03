// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct ISpecifyPropertyPages : INativeGuid
{
    public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));
}

internal unsafe partial struct IPerPropertyBrowsing : INativeGuid
{
    public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));
}

internal unsafe partial struct IProvideClassInfo : INativeGuid
{
    public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));
}

internal unsafe partial struct IProvideMultipleClassInfo : INativeGuid
{
    public static Guid* NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));
}

internal unsafe partial struct IOleContainer : INativeGuid
{
    static Guid* INativeGuid.NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));
}
