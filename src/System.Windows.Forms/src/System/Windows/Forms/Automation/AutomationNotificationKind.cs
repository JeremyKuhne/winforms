// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.Automation
{
    /// <summary>
    ///  Indicates the type of notification when raising the UIA Notification event.
    /// </summary>
    public enum AutomationNotificationKind
    {
        /// <summary>
        ///  The current element container has had something added to it that should be presented to the user.
        /// </summary>
        ItemAdded = UiaCore.NotificationKind.ItemAdded,

        /// <summary>
        ///  The current element has had something removed from inside it that should be presented to the user.
        /// </summary>
        ItemRemoved = UiaCore.NotificationKind.ItemRemoved,

        /// <summary>
        ///  The current element has a notification that an action was completed.
        /// </summary>
        ActionCompleted = UiaCore.NotificationKind.ActionCompleted,

        /// <summary>
        ///  The current element has a notification that an action was abandoned.
        /// </summary>
        ActionAborted = UiaCore.NotificationKind.ActionAborted,

        /// <summary>
        ///  The current element has a notification not an add, remove, completed, or aborted action.
        /// </summary>
        Other = UiaCore.NotificationKind.Other
    }
}
