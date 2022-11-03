// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2ComponentEditor : WindowsFormsComponentEditor
    {
        public static unsafe bool NeedsComponentEditor(object comObject)
        {
            using var browsing = ComHelpers.GetComScope<IPerPropertyBrowsing>(comObject, out bool success);
            if (success)
            {
                // Check for a property page.
                Guid guid = Guid.Empty;
                if (browsing.Value->MapPropertyToPage(PInvoke.MEMBERID_NIL, &guid).Succeeded && !guid.Equals(Guid.Empty))
                {
                    return true;
                }
            }

            using var specify = ComHelpers.GetComScope<ISpecifyPropertyPages>(comObject, out success);
            if (success)
            {
                CAUUID uuids = default;
                try
                {
                    return specify.Value->GetPages(&uuids).Succeeded && uuids.cElems > 0;
                }
                finally
                {
                    if (uuids.pElems is not null)
                    {
                        Marshal.FreeCoTaskMem((nint)uuids.pElems);
                    }
                }
            }

            return false;
        }

        public override unsafe bool EditComponent(ITypeDescriptorContext? context, object obj, IWin32Window? parent)
        {
            HWND hwnd = parent is null ? HWND.Null : (HWND)parent.Handle;

            // Try to get the page guid
            using var browsing = ComHelpers.GetComScope<IPerPropertyBrowsing>(obj, out bool success);
            if (success)
            {
                // Check for a property page.
                Guid guid = Guid.Empty;
                HRESULT hr = browsing.Value->MapPropertyToPage(PInvoke.MEMBERID_NIL, &guid);
                if (hr.Succeeded & !guid.Equals(Guid.Empty))
                {
                    PInvoke.OleCreatePropertyFrame(
                        hwnd,
                        x: 0,
                        y: 0,
                        lpszCaption: "PropertyPages",
                        cObjects: 1,
                        ppUnk: (IUnknown**)(void**)browsing,
                        cPages: 1,
                        pPageClsID: guid,
                        lcid: PInvoke.GetThreadLocale(),
                        dwReserved: 0,
                        pvReserved: null).ThrowOnFailure();

                    return true;
                }
            }

            using var pages = ComHelpers.GetComScope<ISpecifyPropertyPages>(obj, out success);
            if (!success)
            {
                return false;
            }

            CAUUID uuids = default;
            if (!pages.Value->GetPages(&uuids).Succeeded || uuids.cElems == 0)
            {
                return false;
            }

            try
            {
                try
                {
                    fixed (char* c = "PropertyPages")
                    {
                        PInvoke.OleCreatePropertyFrame(
                            hwnd,
                            x: 0,
                            y: 0,
                            lpszCaption: c,
                            cObjects: 1,
                            ppUnk: (IUnknown**)(void**)pages,
                            cPages: uuids.cElems,
                            pPageClsID: uuids.pElems,
                            lcid: PInvoke.GetThreadLocale(),
                            dwReserved: 0,
                            pvReserved: null).ThrowOnFailure();
                    }

                    return true;
                }
                finally
                {
                    if (uuids.pElems is not null)
                    {
                        Marshal.FreeCoTaskMem((nint)uuids.pElems);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!context.TryGetService(out IUIService? uiService))
                {
                    RTLAwareMessageBox.Show(
                        owner: null,
                        SR.ErrorPropertyPageFailed,
                        SR.PropertyGridTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        options: 0);
                }
                else if (ex is not null)
                {
                    uiService.ShowError(ex, SR.ErrorPropertyPageFailed);
                }
                else
                {
                    uiService.ShowError(SR.ErrorPropertyPageFailed);
                }
            }

            GC.KeepAlive(parent);
            return false;
        }
    }
}
