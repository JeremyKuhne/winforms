// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static partial class StringExtensions
{
    extension(string stringValue)
    {
        /// <summary>
        ///  Copies the contents of this string into the destination span.
        /// </summary>
        /// <param name="destination">The span into which to copy this string's contents.</param>
        /// <exception cref="ArgumentException">The destination span is shorter than the source string.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<char> destination)
        {
            if (destination.Length < stringValue.Length)
            {
                throw new ArgumentException("Destination span is too short to copy the string.", nameof(destination));
            }

            stringValue.AsSpan().CopyTo(destination);
        }
    }
}
