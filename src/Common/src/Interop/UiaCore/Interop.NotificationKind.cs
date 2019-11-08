// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        // https://docs.microsoft.com/windows/win32/api/uiautomationcore/ne-uiautomationcore-notificationkind
        public enum NotificationKind
        {
            ItemAdded = 0,
            ItemRemoved = 1,
            ActionCompleted = 2,
            ActionAborted = 3,
            Other = 4
        };
    }
}
