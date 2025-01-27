// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Formats.Nrbf;
using System.Reflection.Metadata;

namespace System.Private.Windows.Nrbf;

internal interface INrbfSerializer
{
    static abstract bool TryWriteObject(Stream stream, object value);
    static abstract bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value);

    static abstract bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type);

    //static abstract IEnumerable<Type> GetSupportedTypes();

    //static abstract bool IsSupportedType<T>();
}

internal class CoreNrbfSerializer : INrbfSerializer
{
    private static Dictionary<TypeName, Type>? s_knownTypes;

    // These types are read from and written to serialized stream manually, accessing record field by field.
    // Thus they are re-hydrated with no formatters and are safe. The default resolver should recognize them
    // to resolve primitive types or fields of the specified type T.
    private static readonly Type[] s_intrinsicTypes =
    [
        // Primitive types.
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(double),
        typeof(float),
        typeof(char),
        typeof(bool),
        typeof(string),
        typeof(decimal),
        typeof(DateTime),
        typeof(TimeSpan),
        typeof(IntPtr),
        typeof(UIntPtr),
        // Special type we use to report that binary formatting is disabled.
        typeof(NotSupportedException),
        // Lists of primitive types
        typeof(List<byte>),
        typeof(List<sbyte>),
        typeof(List<short>),
        typeof(List<ushort>),
        typeof(List<int>),
        typeof(List<uint>),
        typeof(List<long>),
        typeof(List<ulong>),
        typeof(List<float>),
        typeof(List<double>),
        typeof(List<char>),
        typeof(List<bool>),
        typeof(List<string>),
        typeof(List<decimal>),
        typeof(List<DateTime>),
        typeof(List<TimeSpan>),
        // Arrays of primitive types.
        typeof(byte[]),
        typeof(sbyte[]),
        typeof(short[]),
        typeof(ushort[]),
        typeof(int[]),
        typeof(uint[]),
        typeof(long[]),
        typeof(ulong[]),
        typeof(float[]),
        typeof(double[]),
        typeof(char[]),
        typeof(bool[]),
        typeof(string[]),
        typeof(decimal[]),
        typeof(DateTime[]),
        typeof(TimeSpan[]),

        // Exchange types, they are serialized with the .NET Framework assembly name.
        // In .NET they are located in System.Drawing.Primitives.
        typeof(RectangleF),
        typeof(PointF),
        typeof(SizeF),
        typeof(Rectangle),
        typeof(Point),
        typeof(Size),
        typeof(Color)
    ];

    public static bool TryWriteObject(Stream stream, object value)
    {
        throw new NotImplementedException();
    }

    public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetFrameworkObject(out value)
        // While these shouldn't normally be in a ResX file, it doesn't hurt to read them and simplifies the code.
        || record.TryGetDrawingPrimitivesObject(out value);

    public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
    {
        if (s_knownTypes is null)
        {
            s_knownTypes = new(s_intrinsicTypes.Length, TypeNameComparer.Default);
            foreach (Type intrinsic in s_intrinsicTypes)
            {
                s_knownTypes.Add(intrinsic.ToTypeName(), intrinsic);
            }
        }

        return s_knownTypes.TryGetValue(typeName, out type);
    }

    public static IEnumerable<Type> GetSupportedTypes()
    {
        throw new NotImplementedException();
    }

    public static bool IsSupportedType<T>()
    {
        throw new NotImplementedException();
    }
}
