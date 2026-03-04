// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static partial class TypeExtensions
{
    extension(Type? type)
    {
        /// <summary>
        ///  Determines whether the current type can be assigned to a variable of the specified <paramref name="targetType"/>.
        /// </summary>
        public bool IsAssignableTo(Type? targetType) => targetType?.IsAssignableFrom(type) ?? false;
    }
}
