// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Microsoft.VisualStudio.Shell;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  Top level mapping layer between COM Object and TypeDescriptor.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   .NET uses this class for COM object type browsing. <see cref="PropertyGrid"/> is an indirect consumer of this
    ///   through the <see cref="TypeDescriptor"/> support in .NET.
    ///  </para>
    ///  <para>
    ///   The instance of this type is found via reflection in <see cref="TypeConverter"/> and exposed as
    ///   <see cref="TypeDescriptor.ComObjectType"/>.
    ///  </para>
    /// </remarks>
    internal unsafe partial class ComNativeDescriptor : TypeDescriptionProvider
    {
        private readonly AttributeCollection _staticAttributes = new(new Attribute[] { BrowsableAttribute.Yes, DesignTimeVisibleAttribute.No });

        // Our collection of Object managers (Com2Properties) for native properties
        private readonly WeakHashtable _nativeProperties = new();

        // Our collection of browsing handlers, which are stateless and shared across objects.
        private readonly Hashtable extendedBrowsingHandlers = new();

        // We increment this every time we look at an Object, then at specified intervals, we run through the
        // properties list to see if we should delete any.
        private int _clearCount;
        private const int ClearInterval = 25;

        // Called via reflection in the AutomationExtenderManager for the property browser. Don't delete or rename.
        // https://devdiv.visualstudio.com/DevDiv/_git/VS?path=/src/vsip/Packages/Core/PropertyBrowser/AutomationExtenderManager.cs&version=GBmain&line=377&lineEnd=378&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents
        // https://devdiv.visualstudio.com/DevDiv/_git/VS?path=/src/wizard/vsdesigner/designer/microsoft/VisualStudio/Editors/PropertyPages/AutomationExtenderManager.cs&version=GBmain&line=434&lineEnd=435&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents
        public static object? GetNativePropertyValue(object component, string propertyName, ref bool succeeded)
        {
            object? result = null;
            using var dispatch = ComHelpers.GetComScope<IDispatch>(component, out succeeded);
            if (succeeded)
            {
                succeeded = TryGetPropertyValue(dispatch, propertyName, out result);
            }

            return result;
        }

        public override ICustomTypeDescriptor? GetTypeDescriptor(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type objectType,
            object? instance)
            => new ComTypeDescriptor(this, instance);

        internal static unsafe string GetClassName(IUnknown* unknown)
        {
            // Check IVsPerPropertyBrowsing for a name.
            using var browsing = unknown->QueryInterface<IVsPerPropertyBrowsing>(out bool success);
            if (success)
            {
                using BSTR name = default;
                if (browsing.Value->GetClassName(&name).Succeeded && !name.IsNull)
                {
                    return name.ToString();
                }
            }

            using var typeInfo = Com2TypeInfoProcessor.FindTypeInfo(unknown, preferIProvideClassInfo: true);
            if (typeInfo.IsNull)
            {
                return string.Empty;
            }

            using BSTR nameBstr = default;
            typeInfo.Value->GetDocumentation(
                PInvoke.MEMBERID_NIL,
                &nameBstr,
                pBstrDocString: null,
                pdwHelpContext: null,
                pBstrHelpFile: null);
            return nameBstr.AsSpan().TrimStart('_').ToString();
        }

        internal static TypeConverter GetConverter() => TypeDescriptor.GetConverter(typeof(IComponent));

        internal static object? GetEditor(object component, Type baseEditorType)
            => TypeDescriptor.GetEditor(component.GetType(), baseEditorType);

        internal static string GetName(IDispatch* dispatch)
        {
            if (dispatch is null)
            {
                return string.Empty;
            }

            int dispid = Com2TypeInfoProcessor.GetNameDispId(dispatch);
            if (dispid != PInvoke.DISPID_UNKNOWN
                && TryGetPropertyValue(dispatch, dispid, out object? value)
                && value is not null)
            {
                return value.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        internal static bool TryGetPropertyValue(IDispatch* dispatch, string propertyName, out object? value)
        {
            value = null;
            if (dispatch is null)
            {
                return false;
            }

            int dispid = PInvoke.DISPID_UNKNOWN;
            Guid guid = Guid.Empty;
            try
            {
                fixed (char* c = propertyName)
                {
                    return dispatch->GetIDsOfNames(&guid, (PWSTR*)c, 1, PInvoke.GetThreadLocale(), &dispid).Succeeded
                        && dispid != PInvoke.DISPID_UNKNOWN
                        && TryGetPropertyValue(dispatch, dispid, out value);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                return false;
            }
        }

        internal static bool TryGetPropertyValue(IDispatch* dispatch, int dispid, out object? value)
        {
            value = null;

            if (dispatch is null)
            {
                return false;
            }

            if (GetPropertyValue(dispatch, dispid, out VARIANT result) == HRESULT.S_OK)
            {
                value = result.ToObject();
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static HRESULT GetPropertyValue(IDispatch* dispatch, int dispid, out VARIANT result)
        {
            result = default;

            if (dispatch is null)
            {
                return HRESULT.E_NOINTERFACE;
            }

            Guid guid = Guid.Empty;
            EXCEPINFO pExcepInfo = default;
            DISPPARAMS dispParams = default;
            try
            {
                fixed (VARIANT* pVarResult = &result)
                {
                    HRESULT hr = dispatch->Invoke(
                        dispid,
                        &guid,
                        PInvoke.GetThreadLocale(),
                        DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                        &dispParams,
                        pVarResult,
                        &pExcepInfo,
                        puArgErr: null);

                    return hr == HRESULT.DISP_E_EXCEPTION ? (HRESULT)pExcepInfo.scode : hr;
                }
            }
            catch (ExternalException ex)
            {
                return (HRESULT)ex.ErrorCode;
            }
        }

        /// <summary>
        ///  Checks if the given <paramref name="dispid"/> matches the dispid that the <paramref name="obj"/> would
        ///  like to specify as its identification property (Name, ID, etc).
        /// </summary>
        internal static bool IsNameDispId(object obj, int dispid)
        {
            if (obj is null || !obj.GetType().IsCOMObject)
            {
                return false;
            }

            using var dispatch = ComHelpers.GetComScope<IDispatch>(obj, out bool success);
            return success && dispid == Com2TypeInfoProcessor.GetNameDispId(dispatch);
        }

        /// <summary>
        ///  Checks all our property manages to see if any have become invalid.
        /// </summary>
        private void CheckClear()
        {
            // Walk the list every so many calls.
            if ((++_clearCount % ClearInterval) != 0)
            {
                return;
            }

            lock (_nativeProperties)
            {
                _clearCount = 0;

                List<object>? disposeList = null;
                Com2Properties? entry;

                // First walk the list looking for items that need to be cleaned out.
                foreach (DictionaryEntry de in _nativeProperties)
                {
                    entry = de.Value as Com2Properties;

                    if (entry is not null && entry.NeedsRefreshed)
                    {
                        disposeList ??= new List<object>(3);
                        disposeList.Add(de.Key);
                    }
                }

                // Now run through the ones that are dead and dispose them.
                // There's going to be a very small number of these.
                if (disposeList is not null)
                {
                    object oldKey;
                    for (int i = disposeList.Count - 1; i >= 0; i--)
                    {
                        oldKey = disposeList[i];
                        entry = _nativeProperties[oldKey] as Com2Properties;

                        if (entry is not null)
                        {
                            entry.Disposed -= new EventHandler(OnPropsInfoDisposed);
                            entry.Dispose();
                            _nativeProperties.Remove(oldKey);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the properties manager for an Object.
        /// </summary>
        private Com2Properties? GetPropertiesInfo(object component)
        {
            // Check caches if necessary.
            CheckClear();

            // Get the property info Object.
            Com2Properties? properties = (Com2Properties?)_nativeProperties[component];

            // If we don't have one, create one and set it up.
            if (properties is null || !properties.CheckValidity())
            {
                using var unknown = ComHelpers.GetComScope<IUnknown>(component, out bool _);
                properties = Com2TypeInfoProcessor.GetProperties(unknown);
                if (properties is not null)
                {
                    properties.Disposed += OnPropsInfoDisposed;
                    _nativeProperties.SetWeak(component, properties);
                    properties.AddExtendedBrowsingHandlers(extendedBrowsingHandlers);
                }
            }

            return properties;
        }

        internal AttributeCollection GetAttributes(object? component)
        {
            if (component is null)
            {
                return _staticAttributes;
            }

            List<Attribute> attributes = new();

            using var browsing = ComHelpers.GetComScope<IVSMDPerPropertyBrowsing>(component, out bool success);
            if (success)
            {
                attributes.AddRange(Com2IManagedPerPropertyBrowsingHandler.GetComponentAttributes(browsing, PInvoke.MEMBERID_NIL));
            }

            if (Com2ComponentEditor.NeedsComponentEditor(component))
            {
                attributes.Add(new EditorAttribute(typeof(Com2ComponentEditor), typeof(ComponentEditor)));
            }

            return attributes.Count == 0 ? _staticAttributes : new(attributes.ToArray());
        }

        internal PropertyDescriptor? GetDefaultProperty(object component)
        {
            CheckClear();
            return GetPropertiesInfo(component)?.DefaultProperty;
        }

        internal static EventDescriptorCollection GetEvents() => new EventDescriptorCollection(null);

        internal static EventDescriptor? GetDefaultEvent() => null;

        internal PropertyDescriptorCollection GetProperties(object component)
        {
            Com2Properties? properties = GetPropertiesInfo(component);

            if (properties is null)
            {
                return PropertyDescriptorCollection.Empty;
            }

            try
            {
                properties.AlwaysValid = true;
                return new PropertyDescriptorCollection(properties.Properties);
            }
            finally
            {
                properties.AlwaysValid = false;
            }
        }

        /// <summary>
        ///  Fired when the property info gets disposed.
        /// </summary>
        private void OnPropsInfoDisposed(object? sender, EventArgs e)
        {
            if (sender is not Com2Properties propsInfo)
            {
                return;
            }

            propsInfo.Disposed -= OnPropsInfoDisposed;

            lock (_nativeProperties)
            {
                // Find the key.
                object? key = propsInfo.TargetObject;

                if (key is null && _nativeProperties.ContainsValue(propsInfo))
                {
                    // Need to find it - the target object has probably been cleaned out of the Com2Properties object
                    // already, so we run through the hashtable looking for the value, so we know what key to remove.
                    foreach (DictionaryEntry de in _nativeProperties)
                    {
                        if (de.Value == propsInfo)
                        {
                            key = de.Key;
                            break;
                        }
                    }

                    if (key is null)
                    {
                        Debug.Fail("Failed to find Com2 properties key on dispose.");
                        return;
                    }
                }

                if (key is not null)
                {
                    _nativeProperties.Remove(key);
                }
            }
        }

        /// <summary>
        ///  Looks at at value's type and creates an editor based on that.  We use this to decide which editor to use
        ///  for a generic variant.
        /// </summary>
        internal static void ResolveVariantTypeConverterAndTypeEditor(
            object? propertyValue,
            ref TypeConverter currentConverter,
            Type editorType,
            ref object? currentEditor)
        {
            if (propertyValue is not null && !Convert.IsDBNull(propertyValue))
            {
                Type type = propertyValue.GetType();
                TypeConverter subConverter = TypeDescriptor.GetConverter(type);
                if (subConverter is not null && subConverter.GetType() != typeof(TypeConverter))
                {
                    currentConverter = subConverter;
                }

                object? subEditor = TypeDescriptor.GetEditor(type, editorType);
                if (subEditor is not null)
                {
                    currentEditor = subEditor;
                }
            }
        }
    }
}
