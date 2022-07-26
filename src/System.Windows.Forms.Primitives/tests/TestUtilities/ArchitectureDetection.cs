// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    public static class ArchitectureDetection
    {
        public static bool Is32bit => !Environment.Is64BitProcess;
        public static bool Is64bit => Environment.Is64BitProcess;
    }
}
