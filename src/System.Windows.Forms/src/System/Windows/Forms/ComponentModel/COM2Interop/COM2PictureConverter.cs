// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an IPicture to a System.Drawing.Image.
    /// </summary>
    internal sealed unsafe class Com2PictureConverter : Com2DataTypeToManagedDataTypeConverter, IDisposable
    {
        // For round tripping performance we try to keep track of the last thing we converted.

        private object? _lastManaged;

        // OLE_HANDLE
        private uint _lastNativeHandle;
        private IPicture* _lastIPicture;

        private Type _pictureType = typeof(Bitmap);

        public Com2PictureConverter(Com2PropertyDescriptor pd)
        {
            if (pd.DISPID == PInvoke.DISPID_MOUSEICON || pd.Name.Contains("Icon"))
            {
                _pictureType = typeof(Icon);
            }
        }

        public override Type ManagedType => _pictureType;

        public override object? ConvertNativeToManaged(object? nativeValue, Com2PropertyDescriptor pd)
        {
            if (nativeValue is null)
            {
                return null;
            }

            if (!ComHelpers.TryGetComPointer(nativeValue, out IPicture* picture))
            {
                Debug.Fail("nativevalue is not IPicture");
            }

            if (picture->get_Handle(out uint handle).Failed)
            {
                Debug.Fail("Could not get handle");
                picture->Release();
                return null;
            }

            if (_lastManaged is not null && handle == _lastNativeHandle)
            {
                return _lastManaged;
            }

            if (handle != 0)
            {
                // GDI handles are sign extended 32 bit values.
                // We need to first cast to int so sign extension happens correctly.
                nint extendedHandle = (int)handle;

                if (picture->get_Type(out short type).Failed)
                {
                    Debug.Fail("Could not get type");
                    picture->Release();
                    return null;
                }

                switch ((PICTYPE)type)
                {
                    case PICTYPE.PICTYPE_ICON:
                        _pictureType = typeof(Icon);
                        _lastManaged = Icon.FromHandle(extendedHandle);
                        break;
                    case PICTYPE.PICTYPE_BITMAP:
                        _pictureType = typeof(Bitmap);
                        _lastManaged = Image.FromHbitmap(extendedHandle);
                        break;
                    default:
                        Debug.Fail("Unknown picture type");
                        return null;
                }

                _lastNativeHandle = handle;
                _lastIPicture = picture;
            }
            else
            {
                _lastManaged = null;
            }

            return _lastManaged;
        }

        public override VARIANT ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // Don't cancel the set.
            cancelSet = false;

            if (_lastManaged?.Equals(managedValue) == true && _lastIPicture is not null)
            {
                return new()
                {
                    vt = VARENUM.VT_UNKNOWN,
                    data = new() { punkVal = (IUnknown*)_lastIPicture }
                };
            }

            // We have to build an IPicture.
            if (managedValue is not null)
            {
                BOOL own = false;

                PICTDESC pictdesc;
                if (managedValue is Icon icon)
                {
                    pictdesc = PICTDESC.FromIcon(icon, copy: false);
                }
                else if (managedValue is Bitmap bitmap)
                {
                    pictdesc = PICTDESC.FromBitmap(bitmap);
                    own = true;
                }
                else
                {
                    Debug.Fail($"Unknown Image type: {managedValue.GetType().Name}");
                    return null;
                }

                IPicture* picture = default;
                PInvoke.OleCreatePictureIndirect(&pictdesc, IPicture.NativeGuid, own, (void**)&picture).ThrowOnFailure();
                _lastManaged = managedValue;
                _lastIPicture = picture;
                picture->get_Handle(out _lastNativeHandle).ThrowOnFailure();
                return new()
                {
                    vt = VARENUM.VT_UNKNOWN,
                    data = new() { punkVal = (IUnknown*)_lastIPicture }
                };
            }
            else
            {
                _lastManaged = null;
                _lastNativeHandle = 0;
                ReleaseResources();
                return null;
            }
        }

        private void ReleaseResources()
        {
            if (_lastIPicture is not null)
            {
                _lastIPicture->Release();
                _lastIPicture = null;
            }
        }

        private void Dispose(bool disposing)
        {
            ReleaseResources();
        }

        ~Com2PictureConverter()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
