// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Private.Windows.Ole;
using System.Reflection.Metadata;
using Windows.Win32.System.Com;

namespace System.Private.Windows;

internal static unsafe class CoreContext
{
#pragma warning disable IDE1006 // Naming Styles
    [ThreadStatic]
    public static INrbfSerializer? NrbfSerializer;

    [ThreadStatic]
    public static IOleServices? OleServices;

    [ThreadStatic]
    public static object? DataObjectInternal;

    [ThreadStatic]
    public static object? DataFormat;
#pragma warning restore IDE1006 // Naming Styles

    /// <inheritdoc cref="INrbfSerializer.TryWriteObject(Stream, object)"/>
    public static bool TryWriteObject(Stream stream, object value) => NrbfSerializer!.TryWriteObject(stream, value);

    /// <inheritdoc cref="INrbfSerializer.TryGetObject(SerializationRecord, out object?)"/>
    public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) => NrbfSerializer!.TryGetObject(record, out value);

    /// <inheritdoc cref="INrbfSerializer.TryBindToType(TypeName, out Type?)"/>
    public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type) => NrbfSerializer!.TryBindToType(typeName, out type);

    /// <inheritdoc cref="INrbfSerializer.IsFullySupportedType(Type)"/>
    public static bool IsFullySupportedType(Type type) => NrbfSerializer!.IsFullySupportedType(type);

    /// <inheritdoc cref="IOleServices.EnsureThreadState"/>
    public static void EnsureThreadState() => OleServices!.EnsureThreadState();

    /// <inheritdoc cref="IOleServices.GetDataHere(string, object, FORMATETC*, STGMEDIUM*)"/>
    public static HRESULT GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium) =>
        OleServices!.GetDataHere(format, data, pformatetc, pmedium);

    /// <inheritdoc cref="IOleServices.TryGetObjectFromDataObject{T}(IDataObject*, string, out T)"/>
    public static bool TryGetObjectFromDataObject<T>(
        IDataObject* dataObject,
        string format,
        [NotNullWhen(true)] out T data) => OleServices!.TryGetObjectFromDataObject(dataObject, format, out data);

    /// <inheritdoc cref="IOleServices.IsValidTypeForFormat(Type, string)"/>
    public static bool IsValidTypeForFormat(Type type, string format) => OleServices!.IsValidTypeForFormat(type, format);

    /// <inheritdoc cref="IOleServices.AllowTypeWithoutResolver{T}"/>
    public static bool AllowTypeWithoutResolver<T>() => OleServices!.AllowTypeWithoutResolver<T>();

    /// <inheritdoc cref="IOleServices.ValidateDataStoreData(ref string, bool, object?)"/>
    public static void ValidateDataStoreData(ref string format, bool autoConvert, object? data) =>
        OleServices!.ValidateDataStoreData(ref format, autoConvert, data);

    /// <inheritdoc cref="IOleServices.CreateDataObject"/>
    public static IComVisibleDataObject CreateDataObject() => OleServices!.CreateDataObject();

    /// <inheritdoc cref="IOleServices.OleGetClipboard(IDataObject**)"/>
    public static HRESULT OleGetClipboard(IDataObject** dataObject) => OleServices!.OleGetClipboard(dataObject);

    /// <inheritdoc cref="IOleServices.OleSetClipboard(IDataObject*)"/>
    public static HRESULT OleSetClipboard(IDataObject* dataObject) => OleServices!.OleSetClipboard(dataObject);

    /// <inheritdoc cref="IOleServices.OleFlushClipboard"/>
    public static HRESULT OleFlushClipboard() => OleServices!.OleFlushClipboard();

    /// <inheritdoc cref="IDataObjectInternal{TDataObject, TIDataObject}.Create()"/>
    public static TDataObject CreateManagedDataObject<TDataObject, TIDataObject>()
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class =>
        ((IDataObjectInternal<TDataObject, TIDataObject>)DataObjectInternal!).Create();

    /// <inheritdoc cref="IDataObjectInternal{TDataObject, TIDataObject}.Create(IDataObject*)"/>
    public static TDataObject CreateManagedDataObject<TDataObject, TIDataObject>(IDataObject* dataObject)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class =>
        ((IDataObjectInternal<TDataObject, TIDataObject>)DataObjectInternal!).Create(dataObject);

    /// <inheritdoc cref="IDataObjectInternal{TDataObject, TIDataObject}.Create(object)"/>
    public static TDataObject CreateManagedDataObject<TDataObject, TIDataObject>(object data)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class =>
        ((IDataObjectInternal<TDataObject, TIDataObject>)DataObjectInternal!).Create(data);

    /// <inheritdoc cref="IDataObjectInternal{TDataObject, TIDataObject}.Wrap(TIDataObject)"/>
    public static IDataObjectInternal WrapDataObject<TDataObject, TIDataObject>(TIDataObject data)
        where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject
        where TIDataObject : class =>
        ((IDataObjectInternal<TDataObject, TIDataObject>)DataObjectInternal!).Wrap(data);

    /// <inheritdoc cref="IDataFormat{T}.Create(string, int)"/>
    public static T CreateDataFormat<T>(string name, int id)
        where T : IDataFormat =>
        ((IDataFormat<T>)DataFormat!).Create(name, id);
}
