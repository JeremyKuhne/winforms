// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Private.Windows.Ole;
using System.Reflection.Metadata;

namespace System.Private.Windows;

internal interface IRuntime<TDataFormat>
    : INrbfSerializer, IOleServices
    where TDataFormat : IDataFormat<TDataFormat>
{
}

internal abstract class Runtime<TDataFormat, TNrbfSerializer, TOleServices>
    : IRuntime<TDataFormat>
    where TDataFormat : IDataFormat<TDataFormat>
    where TNrbfSerializer : INrbfSerializer
    where TOleServices : IOleServices
{
    static void IOleServices.EnsureThreadState() => TOleServices.EnsureThreadState();
    static bool INrbfSerializer.TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type) =>
        TNrbfSerializer.TryBindToType(typeName, out type);
    static bool INrbfSerializer.TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        TNrbfSerializer.TryGetObject(record, out value);
    static bool INrbfSerializer.TryWriteObject(Stream stream, object value) =>
        TNrbfSerializer.TryWriteObject(stream, value);
}
