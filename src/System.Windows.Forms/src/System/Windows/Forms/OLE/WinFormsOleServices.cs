// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Ole;

namespace System.Windows.Forms.Ole;

/// <summary>
///  Provides Windows Forms specific OLE services.
/// </summary>
internal sealed class WinFormsOleServices : IOleServices
{
    private WinFormsOleServices() { }

    static void IOleServices.EnsureThreadState()
    {
        if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }
    }
}
