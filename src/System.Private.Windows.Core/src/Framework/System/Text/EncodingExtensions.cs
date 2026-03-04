// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Text;

internal static class EncodingExtensions
{
    extension(Encoding encoding)
    {
        public unsafe int GetBytes(ReadOnlySpan<char> source, Span<byte> destination)
        {
            fixed (char* sourcePointer = source)
            fixed (byte* destinationPointer = destination)
            {
                return encoding.GetBytes(sourcePointer, source.Length, destinationPointer, destination.Length);
            }
        }

        public unsafe string GetString(ReadOnlySpan<byte> source)
        {
            fixed (byte* sourcePointer = source)
            {
                return encoding.GetString(sourcePointer, source.Length);
            }
        }
    }
}
