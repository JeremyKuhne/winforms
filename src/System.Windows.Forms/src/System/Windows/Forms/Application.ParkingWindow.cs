﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides <see langword='static '/>
    /// methods and properties
    /// to manage an application, such as methods to run and quit an application,
    /// to process Windows messages, and properties to get information about an application. This
    /// class cannot be inherited.
    /// </summary>
    public sealed partial class Application
    {
        /// <summary>
        ///  This class embodies our parking window, which we create when the
        ///  first message loop is pushed onto the thread.
        /// </summary>
        internal sealed class ParkingWindow : ContainerControl, IArrangedElement
        {
            // WHIDBEY CHANGES
            //   in whidbey we now aggressively tear down the parking window
            //   when the last control has been removed off of it.

            private const int WM_CHECKDESTROY = Interop.WindowMessages.WM_USER + 0x01;

            private int childCount = 0;

            public ParkingWindow()
            {
                SetExtendedState(ExtendedStates.InterestedInUserPreferenceChanged, false);
                SetState(States.TopLevel, true);
                Text = "WindowsFormsParkingWindow";
                Visible = false;
                DpiHelper.FirstParkingWindowCreated = true;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;

                    // Message only windows are cheaper and have fewer issues than
                    // full blown invisible windows.
                    cp.Parent = (IntPtr)NativeMethods.HWND_MESSAGE;
                    return cp;
                }
            }

            internal override void AddReflectChild()
            {
                if (childCount < 0)
                {
                    Debug.Fail("How did parkingwindow childcount go negative???");
                    childCount = 0;
                }
                childCount++;
            }

            internal override void RemoveReflectChild()
            {
                childCount--;
                if (childCount < 0)
                {
                    Debug.Fail("How did parkingwindow childcount go negative???");
                    childCount = 0;
                }
                if (childCount == 0)
                {
                    if (IsHandleCreated)
                    {
                        //check to see if we are running on the thread that owns the parkingwindow.
                        //if so, we can destroy immediately.
                        //This is important for scenarios where apps leak controls until after the
                        //messagepump is gone and then decide to clean them up.  We should clean
                        //up the parkingwindow in this case and a postmessage won't do it.
                        //unused
                        int id = SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, HandleInternal), out int lpdwProcessId);
                        ThreadContext ctx = Application.ThreadContext.FromId(id);

                        //We only do this if the ThreadContext tells us that we are currently
                        //handling a window message.
                        if (ctx == null ||
                            !Object.ReferenceEquals(ctx, Application.ThreadContext.FromCurrent()))
                        {
                            UnsafeNativeMethods.PostMessage(new HandleRef(this, HandleInternal), WM_CHECKDESTROY, IntPtr.Zero, IntPtr.Zero);
                        }
                        else
                        {
                            CheckDestroy();
                        }
                    }
                }
            }

            private void CheckDestroy()
            {
                if (childCount == 0)
                {
                    IntPtr hwndChild = UnsafeNativeMethods.GetWindow(new HandleRef(this, Handle), NativeMethods.GW_CHILD);
                    if (hwndChild == IntPtr.Zero)
                    {
                        DestroyHandle();
                    }
                }
            }

            public void Destroy()
            {
                DestroyHandle();
            }

            /// <summary>
            ///  "Parks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
            ///  be parked.
            /// </summary>
            internal void ParkHandle(HandleRef handle)
            {
                if (!IsHandleCreated)
                {
                    CreateHandle();
                }

                UnsafeNativeMethods.SetParent(handle, new HandleRef(this, Handle));
            }

            /// <summary>
            ///  "Unparks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
            ///  be parked.
            /// </summary>
            internal void UnparkHandle(HandleRef handle)
            {
                if (IsHandleCreated)
                {
                    Debug.Assert(UnsafeNativeMethods.GetParent(handle) != Handle, "Always set the handle's parent to someone else before calling UnparkHandle");
                    // If there are no child windows in this handle any longer, destroy the parking window.
                    CheckDestroy();
                }
            }

            // Do nothing on layout to reduce the calls into the LayoutEngine while debugging.
            protected override void OnLayout(LayoutEventArgs levent) { }
            void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string affectedProperty) { }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg != Interop.WindowMessages.WM_SHOWWINDOW)
                {
                    base.WndProc(ref m);
                    if (m.Msg == Interop.WindowMessages.WM_PARENTNOTIFY)
                    {
                        if (NativeMethods.Util.LOWORD(unchecked((int)(long)m.WParam)) == Interop.WindowMessages.WM_DESTROY)
                        {
                            UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), WM_CHECKDESTROY, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    else if (m.Msg == WM_CHECKDESTROY)
                    {
                        CheckDestroy();
                    }
                }
            }
        }
    }
}
