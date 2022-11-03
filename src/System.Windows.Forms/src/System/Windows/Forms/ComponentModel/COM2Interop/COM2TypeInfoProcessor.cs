// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static Windows.Win32.System.Com.VARENUM;
using static Windows.Win32.System.Com.VARFLAGS;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  <para>
    ///   This is the main worker class of Com2 property interop. It takes an IDispatch Object
    ///   and translates it's ITypeInfo into Com2PropertyDescriptor objects that are understandable
    ///   by managed code.
    ///  </para>
    ///  <para>
    ///   This class only knows how to process things that are natively in the typeinfo. Other property
    ///   information such as IPerPropertyBrowsing is handled elsewhere.
    ///  </para>
    /// </summary>
    internal static unsafe partial class Com2TypeInfoProcessor
    {
        private static readonly TraceSwitch DbgTypeInfoProcessorSwitch = new(
            "DbgTypeInfoProcessor",
             "Com2TypeInfoProcessor: debug Com2 type info processing");

        private static ModuleBuilder? s_moduleBuilder;

        private static ModuleBuilder ModuleBuilder
        {
            get
            {
                if (s_moduleBuilder is null)
                {
                    AssemblyName assemblyName = new()
                    {
                        Name = "COM2InteropEmit"
                    };

                    AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                    s_moduleBuilder = builder.DefineDynamicModule("COM2Interop.Emit");
                }

                return s_moduleBuilder;
            }
        }

        private static Hashtable? s_builtEnums;
        private static Dictionary<Guid, CachedProperties>? s_processedLibraries;

        /// <summary>
        ///  Given a COM object attempt to locate its type info.
        /// </summary>
        public static ComScope<ITypeInfo> FindTypeInfo(IUnknown* unknown, bool preferIProvideClassInfo)
        {
            ComScope<ITypeInfo> typeInfo = new(null);

            // This is kind of odd. What's going on here is that if we want the CoClass (e.g. for
            // the interface name), we need to look for IProvideClassInfo first, then look for the
            // typeinfo from the IDispatch. In the case of many Oleaut32 operations, the CoClass
            // doesn't have the interface members on it, although in the shell it usually does, so
            // we need to re-order the lookup if we *actually* want the CoClass if it's available.
            for (int i = 0; i < 2; i++)
            {
                if (preferIProvideClassInfo == (i == 0))
                {
                    using var provideClassInfo = unknown->QueryInterface<IProvideClassInfo>(out bool success);
                    if (success && provideClassInfo.Value->GetClassInfo(typeInfo).Succeeded)
                    {
                        return typeInfo;
                    }
                }
                else
                {
                    using var dispatch = unknown->QueryInterface<IDispatch>(out bool success);
                    if (success && dispatch.Value->GetTypeInfo(0, PInvoke.GetThreadLocale(), typeInfo).Succeeded)
                    {
                        return typeInfo;
                    }
                }
            }

            return typeInfo;
        }

        /// <summary>
        ///  Given an object, this attempts to locate its type info. If it implements IProvideMultipleClassInfo
        ///  all available type infos will be returned, otherwise the primary one will be called.
        /// </summary>
        internal unsafe ref struct TypeInfoEnumerator
        {
            private IProvideMultipleClassInfo* _classInfo;
            private ComScope<ITypeInfo> _current;
            private uint _index;
            private readonly uint _count;

            public TypeInfoEnumerator(IUnknown* unknown, bool preferIProvideClassInfo)
            {
                IProvideMultipleClassInfo* classInfo;
                if (unknown->QueryInterface(IProvideMultipleClassInfo.NativeGuid, (void**)&classInfo).Succeeded)
                {
                    _classInfo = classInfo;
                    _classInfo->GetMultiTypeInfoCount(out _count);
                }
                else
                {
                    _current = FindTypeInfo(unknown, preferIProvideClassInfo);
                }
            }

            public bool MoveNext()
            {
                bool succeeded = false;
                if (!_current.IsNull)
                {
                    succeeded = true;
                }
                else if (_classInfo is not null && _index < _count)
                {
                    Debug.Assert(_current.IsNull);
                    succeeded = _classInfo->GetInfoOfIndex(
                        _index++,
                        MULTICLASSINFO_FLAGS.MULTICLASSINFO_GETTYPEINFO,
                        _current,
                        pdwTIFlags: null,
                        pcdispidReserved: null,
                        piidPrimary: null,
                        piidSource: null).Succeeded;
                }

                return succeeded;
            }

            [UnscopedRef]
            public ref ComScope<ITypeInfo> Current => ref _current;

            public void Dispose()
            {
                if (_classInfo is not null)
                {
                    ((IUnknown*)_classInfo)->Release();
                    _classInfo = null;
                }

                if (!_current.IsNull)
                {
                    Debug.Fail("Caller should have disposed current");
                    _current.Dispose();
                }
            }
        }

        ///// <summary>
        /////  Given an Object, this attempts to locate its type info. If it implements IProvideMultipleClassInfo
        /////  all available type infos will be returned, otherwise the primary one will be called.
        ///// </summary>
        //public static unsafe ReadOnlySpan<ITypeInfo*> FindTypeInfos(object comObject, bool preferIProvideClassInfo)
        //{
        //    if (comObject is IProvideMultipleClassInfo.Interface classInfo)
        //    {
        //        uint count = 0;

        //        if (classInfo.GetMultiTypeInfoCount(&count).Succeeded && count > 0)
        //        {
        //            ITypeInfo*[] typeInfos = new ITypeInfo*[count];

        //            ITypeInfo* typeInfo =
        //            var typeInfos = new Oleaut32.ITypeInfo[count];
        //            for (uint i = 0; i < count; i++)
        //            {
        //                if (classInfo.GetInfoOfIndex(
        //                    i,
        //                    MULTICLASSINFO_FLAGS.MULTICLASSINFO_GETTYPEINFO,
        //                    out Oleaut32.ITypeInfo typeInfo,
        //                    pdwTIFlags: null,
        //                    pcdispidReserved: null,
        //                    piidPrimary: null,
        //                    piidSource: null).Failed)
        //                {
        //                    continue;
        //                }

        //                Debug.Assert(typeInfo is not null);
        //                if (typeInfo is not null)
        //                {
        //                    typeInfos[i] = typeInfo;
        //                }
        //            }

        //            return typeInfos;
        //        }
        //    }

        //    Oleaut32.ITypeInfo? temp = FindTypeInfo(comObject, preferIProvideClassInfo);
        //    return temp is not null ? (new Oleaut32.ITypeInfo[] { temp }) : Array.Empty<Oleaut32.ITypeInfo>();
        //}

        /// <summary>
        ///  Retrieve the dispid of the property that we are to use as the name member. In this case, the grid
        ///  will put parenthesis around the name.
        /// </summary>
        public static int GetNameDispId(IDispatch* dispatch)
        {
            int dispid = PInvoke.DISPID_UNKNOWN;
            string? name = null;

            // First try to find one with a valid value.
            if (ComNativeDescriptor.TryGetPropertyValue(dispatch, "__id", out _))
            {
                name = "__id";
            }
            else
            {
                if (ComNativeDescriptor.TryGetPropertyValue(dispatch, PInvoke.DISPID_Name, out _))
                {
                    dispid = PInvoke.DISPID_Name;
                }
                else if (ComNativeDescriptor.TryGetPropertyValue(dispatch, "Name", out _))
                {
                    name = "Name";
                }
            }

            // Now get the dispid of the one that worked.
            if (name is not null)
            {
                int pDispid = PInvoke.DISPID_UNKNOWN;
                Guid guid = Guid.Empty;

                fixed (char* n = name)
                {
                    if (dispatch->GetIDsOfNames(&guid, (PWSTR*)n, 1, PInvoke.GetThreadLocale(), &pDispid).Succeeded)
                    {
                        dispid = pDispid;
                    }
                }
            }

            return dispid;
        }

        /// <summary>
        ///  Gets the properties for a given Com2 Object. The returned Com2Properties
        ///  Object contains the properties and relevant data about them.
        /// </summary>
        public static Com2Properties? GetProperties(IUnknown* unknown)
        {
            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties");

            if (unknown is null)
            {
                Debug.WriteLineIf(
                    DbgTypeInfoProcessorSwitch.TraceVerbose,
                    "Com2TypeInfoProcessor.GetProperties returning null: Object is not a COM object");

                return null;
            }

            int defaultProperty = -1;
            List<Com2PropertyDescriptor> propList = new();
            List<Guid> typeGuids = new();

            using var infos = new TypeInfoEnumerator(unknown, preferIProvideClassInfo: false);

            int i = 0;
            while (infos.MoveNext())
            {
                TYPEATTR* pTypeAttr = null;
                using var info = infos.Current;
                Guid typeGuid = GetGuidForTypeInfo(info, out var versions);

                Com2PropertyDescriptor[]? properties = null;
                s_processedLibraries ??= new();
                bool wasProcessed = typeGuid != Guid.Empty && s_processedLibraries.ContainsKey(typeGuid);

                if (wasProcessed)
                {
                    CachedProperties cachedProperties = s_processedLibraries[typeGuid];

                    if (versions.Major == cachedProperties.MajorVersion && versions.Minor == cachedProperties.MinorVersion)
                    {
                        properties = cachedProperties.Properties;
                        if (i == 0 && cachedProperties.DefaultIndex != -1)
                        {
                            defaultProperty = cachedProperties.DefaultIndex;
                        }
                    }
                    else
                    {
                        // Updated version, mark for reprocessing.
                        wasProcessed = false;
                    }
                }

                if (!wasProcessed)
                {
                    properties = InternalGetProperties(comObject, typeInfo, Ole32.DispatchID.MEMBERID_NIL);

                    // Only save the default property from the first type Info.
                    if (i == 0)
                    {
                        defaultProperty = -1;
                    }

                    if (typeGuid != Guid.Empty)
                    {
                        s_processedLibraries[typeGuid] = new CachedProperties(properties, i == 0 ? defaultProperty : -1, versions[0], versions[1]);
                    }
                }

                if (properties is not null)
                {
                    propList.AddRange(properties);
                }

                i++;
            }

            //if (typeInfos.Length == 0)
            //{
            //    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "Com2TypeInfoProcessor.GetProperties :: Didn't get typeinfo");
            //    return null;
            //}

            for (int i = 0; i < typeInfos.Length; i++)
            {
                Oleaut32.ITypeInfo typeInfo = typeInfos[i];

                uint[] versions = new uint[2];
                Guid typeGuid = GetGuidForTypeInfo(typeInfo, versions);
                Com2PropertyDescriptor[]? properties = null;

                s_processedLibraries ??= new();

                bool wasProcessed = typeGuid != Guid.Empty && s_processedLibraries.ContainsKey(typeGuid);

                if (wasProcessed)
                {
                    CachedProperties cachedProperties = s_processedLibraries[typeGuid];

                    if (versions[0] == cachedProperties.MajorVersion && versions[1] == cachedProperties.MinorVersion)
                    {
                        properties = cachedProperties.Properties;
                        if (i == 0 && cachedProperties.DefaultIndex != -1)
                        {
                            defaultProperty = cachedProperties.DefaultIndex;
                        }
                    }
                    else
                    {
                        // Updated version, mark for reprocessing.
                        wasProcessed = false;
                    }
                }

                if (!wasProcessed)
                {
                    properties = InternalGetProperties(comObject, typeInfo, Ole32.DispatchID.MEMBERID_NIL);

                    // Only save the default property from the first type Info.
                    if (i == 0)
                    {
                        defaultProperty = -1;
                    }

                    if (typeGuid != Guid.Empty)
                    {
                        s_processedLibraries[typeGuid] = new CachedProperties(properties, i == 0 ? defaultProperty : -1, versions[0], versions[1]);
                    }
                }

                if (properties is not null)
                {
                    propList.AddRange(properties);
                }
            }

            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"Com2TypeInfoProcessor.GetProperties : returning {propList.Count} properties");

            // Done!
            Com2PropertyDescriptor[] temp2 = new Com2PropertyDescriptor[propList.Count];
            propList.CopyTo(temp2, 0);

            return new Com2Properties(comObject, propList.ToArray(), defaultProperty);
        }

        private static Guid GetGuidForTypeInfo(ITypeInfo* typeInfo, out (uint Major, uint Minor) versions)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo->GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            versions = (pTypeAttr->wMajorVerNum, pTypeAttr->wMinorVerNum);
            Guid guid = pTypeAttr->guid;
            typeInfo->ReleaseTypeAttr(pTypeAttr);
            return guid;
        }

        /// <summary>
        ///  Resolves a value type for a property from a TYPEDESC. Value types can be user defined, which and may be
        ///  aliased into other type infos. This function will recursively walk the ITypeInfos to resolve the type to
        ///  a CLR Type.
        /// </summary>
        private static unsafe Type? GetValueTypeFromTypeDesc(TYPEDESC typeDesc, ITypeInfo* typeInfo, out Guid? typeGuid)
        {
            uint hreftype;
            HRESULT hr = HRESULT.S_OK;
            typeGuid = null;

            switch (typeDesc.vt)
            {
                default:
                    return VTToType(typeDesc.vt);

                case VT_UNKNOWN:
                case VT_DISPATCH:
                    // Get the guid.
                    typeGuid = GetGuidForTypeInfo(typeInfo, out _);

                    // Return the type.
                    return VTToType(typeDesc.vt);

                case VT_USERDEFINED:
                    // We'll need to recurse into a user defined reference typeinfo.
                    Debug.Assert(typeDesc.Anonymous.hreftype != 0u, "typeDesc doesn't contain an hreftype!");
                    hreftype = typeDesc.Anonymous.hreftype;
                    break;

                case VT_PTR:
                    // We'll need to recurse into a user defined reference typeinfo.
                    Debug.Assert(typeDesc.Anonymous.lptdesc is not null, "typeDesc doesn't contain an refTypeDesc!");
                    if (typeDesc.Anonymous.lptdesc->vt == VT_VARIANT)
                    {
                        return VTToType(typeDesc.Anonymous.lptdesc->vt);
                    }

                    hreftype = typeDesc.Anonymous.lptdesc->Anonymous.hreftype;
                    break;
            }

            // Get the reference type info.
            ComScope<ITypeInfo> refTypeInfo = new(null);
            hr = typeInfo->GetRefTypeInfo(hreftype, refTypeInfo);
            if (!hr.Succeeded)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetRefTypeInfoFailed, hr), (int)hr);
            }

            // Here is where we look at the next level type info. If we get an enum, process it, otherwise we will
            // recurse or get a dispatch.
            if (refTypeInfo.IsNull)
            {
                return null;
            }

            TYPEATTR* pTypeAttr = null;
            hr = refTypeInfo.Value->GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                Guid guid = pTypeAttr->guid;

                // Save the guid if we've got one here.
                if (!Guid.Empty.Equals(guid))
                {
                    typeGuid = guid;
                }

                return pTypeAttr->typekind switch
                {
                    TYPEKIND.TKIND_ENUM => ProcessTypeInfoEnum(refTypeInfo),
                    // Recurse here.
                    TYPEKIND.TKIND_ALIAS => GetValueTypeFromTypeDesc(pTypeAttr->tdescAlias, refTypeInfo, out typeGuid),
                    TYPEKIND.TKIND_DISPATCH => VTToType(VT_DISPATCH),
                    TYPEKIND.TKIND_INTERFACE or TYPEKIND.TKIND_COCLASS => VTToType(VT_UNKNOWN),
                    _ => null,
                };
            }
            finally
            {
                refTypeInfo.Value->ReleaseTypeAttr(pTypeAttr);
            }
        }

        private static Com2PropertyDescriptor[] InternalGetProperties(
            IDispatch* dispatch,
            ITypeInfo* typeInfo,
            int dispidToGet)
        {
            Dictionary<string, PropertyInfo> propertyInfo = new();

            int nameDispID = GetNameDispId(dispatch);
            bool addAboutBox = false;

            // Properties can live as functions with get_ and put_ or as variables, so we do two steps here.
            try
            {
                // Do FUNCDESC things.
                ProcessFunctions(typeInfo, propertyInfo, dispidToGet, nameDispID, ref addAboutBox);
            }
            catch (ExternalException ex)
            {
                Debug.Fail($"ProcessFunctions failed with hr={ex.ErrorCode}, message={ex}");
            }

            try
            {
                // Do VARDESC things.
                ProcessVariables(typeInfo, propertyInfo, dispidToGet, nameDispID);
            }
            catch (ExternalException ex)
            {
                Debug.Fail($"ProcessVariables failed with hr={ex.ErrorCode}, message={ex}");
            }

            // Now we take the propertyInfo structures we built up and use them to create the actual descriptors.
            int propertyCount = propertyInfo.Count;

            if (addAboutBox)
            {
                propertyCount++;
            }

            Com2PropertyDescriptor[] properties = new Com2PropertyDescriptor[propertyCount];
            int defaultProperty = -1;

            HRESULT hr = HRESULT.S_OK;
            object[] pvar = new object[1];

            // For each item in our list, create the descriptor an check if it's the default one.
            foreach (PropertyInfo info in propertyInfo.Values)
            {
                if (!info.NonBrowsable)
                {
                    // Finally, for each property, make sure we can get the value
                    // if we can't then we should mark it non-browsable.

                    try
                    {
                        hr = ComNativeDescriptor.GetPropertyValue(dispatch, info.DispId, pvar);
                    }
                    catch (ExternalException ex)
                    {
                        hr = (HRESULT)ex.ErrorCode;
                        Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"IDispatch::Invoke(PROPGET, {info.Name}) threw an exception :{ex}");
                    }

                    if (!hr.Succeeded)
                    {
                        Debug.WriteLineIf(
                            DbgTypeInfoProcessorSwitch.TraceVerbose,
                            $"Adding Browsable(false) to property '{info.Name}' because Invoke(dispid=0x{info.DispId:X}, DISPATCH_PROPERTYGET) returned hr=0x{hr:X}. Properties that do not return S_OK are hidden by default.");
                        info.Attributes.Add(new BrowsableAttribute(false));
                        info.NonBrowsable = true;
                    }
                }
                else
                {
                    hr = HRESULT.S_OK;
                }

                properties[info.Index] = new Com2PropertyDescriptor(
                    info.DispId,
                    info.Name,
                    info.Attributes.ToArray(),
                    info.ReadOnly != PropertyInfo.ReadOnlyState.False,
                    info.ValueType,
                    info.TypeGuid,
                    !hr.Succeeded);

                if (info.IsDefault)
                {
                    defaultProperty = info.Index;
                }
            }

            if (addAboutBox)
            {
                properties[^1] = new Com2AboutBoxPropertyDescriptor();
            }

            return properties;
        }

        private static unsafe PropertyInfo ProcessDataCore(
            ITypeInfo* typeInfo,
            IDictionary<string, PropertyInfo> propertyInfo,
            int dispid,
            int nameDispID,
            in TYPEDESC typeDesc,
            VARFLAGS flags)
        {
            // Get the name and the helpstring.
            using var nameBstr = default(BSTR);
            using var helpStringBstr = default(BSTR);
            HRESULT hr = typeInfo->GetDocumentation(dispid, &nameBstr, &helpStringBstr, pdwHelpContext: null, pBstrHelpFile: null);
            if (!hr.Succeeded)
            {
                throw new COMException(string.Format(SR.TYPEINFOPROCESSORGetDocumentationFailed, dispid, hr, ComNativeDescriptor.GetClassName((IUnknown*)typeInfo)), (int)hr);
            }

            string name = nameBstr.AsSpan().ToString();
            if (string.IsNullOrEmpty(name))
            {
                Debug.Fail($"ITypeInfo::GetDocumentation didn't return a name for DISPID 0x{dispid:X} but returned SUCCEEDED(hr),  Component={ComNativeDescriptor.GetClassName((IUnknown*)typeInfo)}");
                return null;
            }

            // Now we can create our struct. Make sure we don't already have one.
            if (!propertyInfo.TryGetValue(name, out PropertyInfo? info))
            {
                info = new()
                {
                    Index = propertyInfo.Count,
                    Name = name,
                    DispId = dispid
                };

                propertyInfo.Add(name, info);
                info.Attributes.Add(new DispIdAttribute(info.DispId));
            }

            var helpString = helpStringBstr.AsSpan();
            if (!helpString.IsEmpty)
            {
                info.Attributes.Add(new DescriptionAttribute(helpString.ToString()));
            }

            // Figure out the value type.
            if (info.ValueType is null)
            {
                Guid? typeData = default;
                try
                {
                    info.ValueType = GetValueTypeFromTypeDesc(typeDesc, typeInfo, out typeData);
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(
                        DbgTypeInfoProcessorSwitch.TraceVerbose,
                         $"Hiding property {info.Name} because value Type could not be resolved: {ex}");
                }

                // If we can't resolve the type, mark the property as nonbrowsable.
                if (info.ValueType is null)
                {
                    info.NonBrowsable = true;
                }

                if (info.NonBrowsable)
                {
                    flags |= VARFLAG_FNONBROWSABLE;
                }

                info.TypeGuid = typeData.Value;
            }

            // Check the flags.
            if ((flags & VARFLAG_FREADONLY) != 0)
            {
                info.ReadOnly = PropertyInfo.ReadOnlyState.True;
            }

            if ((flags & VARFLAG_FHIDDEN) != 0 ||
                (flags & VARFLAG_FNONBROWSABLE) != 0 ||
                info.Name[0] == '_' ||
                dispid == Ole32.DispatchID.HWND)
            {
                info.Attributes.Add(new BrowsableAttribute(false));
                info.NonBrowsable = true;
            }

            if ((flags & VARFLAG_FUIDEFAULT) != 0)
            {
                info.IsDefault = true;
            }

            if ((flags & VARFLAG_FBINDABLE) != 0 &&
                (flags & VARFLAG_FDISPLAYBIND) != 0)
            {
                info.Attributes.Add(new BindableAttribute(true));
            }

            // Lastly, if it's DISPID_Name, add the ParenthesizeNameAttribute.
            if (dispid == nameDispID)
            {
                info.Attributes.Add(new ParenthesizePropertyNameAttribute(true));

                // Don't allow merges on the name.
                info.Attributes.Add(new MergablePropertyAttribute(false));
            }

            return info;
        }

        private static unsafe void ProcessFunctions(
            ITypeInfo* typeInfo,
            IDictionary<string, PropertyInfo> propertyInfo,
            int dispidToGet,
            int nameDispID,
            ref bool addAboutBox)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo->GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded || pTypeAttr is null)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                bool isPropGet;
                PropertyInfo? info;

                for (uint i = 0; i < pTypeAttr->cFuncs; i++)
                {
                    FUNCDESC* pFuncDesc = null;
                    hr = typeInfo->GetFuncDesc(i, &pFuncDesc);
                    if (!hr.Succeeded || pFuncDesc is null)
                    {
                        Debug.WriteLineIf(
                            DbgTypeInfoProcessorSwitch.TraceVerbose,
                            $"ProcessTypeInfoEnum: ignoring function item 0x{i:X} because Oleaut32.ITypeInfo::GetFuncDesc returned hr=0x{hr:X} or NULL");
                        continue;
                    }

                    try
                    {
                        if (pFuncDesc->invkind == INVOKEKIND.INVOKE_FUNC
                            || (dispidToGet != PInvoke.MEMBERID_NIL && pFuncDesc->memid != dispidToGet))
                        {
                            if (pFuncDesc->memid == PInvoke.DISPID_ABOUTBOX)
                            {
                                addAboutBox = true;
                            }

                            continue;
                        }

                        TYPEDESC typeDesc;

                        // Is this a get or a put?
                        isPropGet = pFuncDesc->invkind == INVOKEKIND.INVOKE_PROPERTYGET;

                        if (isPropGet)
                        {
                            if (pFuncDesc->cParams != 0)
                            {
                                continue;
                            }

                            typeDesc = pFuncDesc->elemdescFunc.tdesc;
                        }
                        else
                        {
                            Debug.Assert(pFuncDesc->lprgelemdescParam is not null, "ELEMDESC param is null!");
                            if (pFuncDesc->lprgelemdescParam is null || pFuncDesc->cParams != 1)
                            {
                                continue;
                            }

                            typeDesc = pFuncDesc->lprgelemdescParam->tdesc;
                        }

                        info = ProcessDataCore(typeInfo, propertyInfo, (Ole32.DispatchID)pFuncDesc->memid, nameDispID, in typeDesc, (VARFLAGS)pFuncDesc->wFuncFlags);

                        // Ff we got a set method, it's not readonly.
                        if (info is not null && !isPropGet)
                        {
                            info.ReadOnly = PropertyInfo.ReadOnlyState.False;
                        }
                    }
                    finally
                    {
                        typeInfo.ReleaseFuncDesc(pFuncDesc);
                    }
                }
            }
            finally
            {
                typeInfo.ReleaseTypeAttr(pTypeAttr);
            }
        }

        /// <summary>
        ///  This converts a type info that describes a IDL defined enum into one we can use
        /// </summary>
        private static Type? ProcessTypeInfoEnum(ITypeInfo* enumTypeInfo)
        {
            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum entered");

            if (enumTypeInfo is null)
            {
                Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, "ProcessTypeInfoEnum got a NULL enumTypeInfo");
                return null;
            }

            try
            {
                TYPEATTR* pTypeAttr = null;
                HRESULT hr = enumTypeInfo->GetTypeAttr(&pTypeAttr);
                if (!hr.Succeeded || pTypeAttr is null)
                {
                    throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
                }

                try
                {
                    uint nItems = pTypeAttr->cVars;

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"ProcessTypeInfoEnum: processing {nItems} variables");

                    List<string> strings = new();
                    List<object?> vars = new();

                    object? varValue = null;

                    using var enumNameBstr = default(BSTR);
                    using var enumHelpStringBstr = default(BSTR);
                    enumTypeInfo->GetDocumentation(PInvoke.MEMBERID_NIL, &enumNameBstr, &enumHelpStringBstr, null, null);

                    // For each item in the enum type info, we just need it's name and value and helpstring if it's there.
                    for (uint i = 0; i < nItems; i++)
                    {
                        VARDESC* pVarDesc = null;
                        hr = enumTypeInfo->GetVarDesc(i, &pVarDesc);
                        if (!hr.Succeeded || pVarDesc is null)
                        {
                            Debug.WriteLineIf(
                                DbgTypeInfoProcessorSwitch.TraceVerbose,
                                $"ProcessTypeInfoEnum: ignoring item 0x{i:X} because Oleaut32.ITypeInfo::GetVarDesc returned hr=0x{hr:X} or NULL");
                            continue;
                        }

                        try
                        {
                            if (pVarDesc->varkind != VARKIND.VAR_CONST || pVarDesc->Anonymous.lpvarValue == null)
                            {
                                continue;
                            }

                            varValue = null;

                            // Get the name and the helpstring
                            using BSTR nameBstr = default;
                            using BSTR helpBstr = default;
                            hr = enumTypeInfo->GetDocumentation(pVarDesc->memid, &nameBstr, &helpBstr, null, null);
                            if (!hr.Succeeded)
                            {
                                Debug.WriteLineIf(
                                    DbgTypeInfoProcessorSwitch.TraceVerbose,
                                    $"ProcessTypeInfoEnum: ignoring item 0x{i:X} because Oleaut32.ITypeInfo::GetDocumentation returned hr=0x{(int)hr:X} or NULL");
                                continue;
                            }

                            var name = nameBstr.AsSpan();
                            var helpString = helpBstr.AsSpan();
                            Debug.WriteLineIf(
                                DbgTypeInfoProcessorSwitch.TraceVerbose,
                                $"ProcessTypeInfoEnum got name={name}, helpstring={helpString}");

                            // Get the value.
                            try
                            {
                                varValue = Marshal.GetObjectForNativeVariant((nint)(void*)pVarDesc->Anonymous.lpvarValue);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLineIf(
                                    DbgTypeInfoProcessorSwitch.TraceVerbose,
                                    $"ProcessTypeInfoEnum: PtrtoStructFailed {ex.GetType().Name},{ex.Message}");
                            }

                            Debug.WriteLineIf(
                                DbgTypeInfoProcessorSwitch.TraceVerbose,
                                $"ProcessTypeInfoEnum: adding variable value={Convert.ToString(varValue, CultureInfo.InvariantCulture)}");
                            vars.Add(varValue);

                            // If we have a helpstring, use it, otherwise use name.
                            string nameString;
                            if (!helpString.IsEmpty)
                            {
                                nameString = helpString.ToString();
                            }
                            else
                            {
                                Debug.Assert(!name.IsEmpty, "No name for VARDESC member, but GetDocumentation returned S_OK!");
                                nameString = name.ToString();
                            }

                            Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"ProcessTypeInfoEnum: adding name value={nameString}");
                            strings.Add(nameString);
                        }
                        finally
                        {
                            enumTypeInfo->ReleaseVarDesc(pVarDesc);
                        }
                    }

                    Debug.WriteLineIf(DbgTypeInfoProcessorSwitch.TraceVerbose, $"ProcessTypeInfoEnum: returning enum with {strings.Count} items");

                    if (strings.Count <= 0)
                    {
                        return null;
                    }

                    // Get the IUnknown value of the Oleaut32.ITypeInfo.
                    string enumName = $"{(nint)enumTypeInfo}_{enumNameBstr.AsSpan()}";

                    if (s_builtEnums is null)
                    {
                        s_builtEnums = new Hashtable();
                    }
                    else if (s_builtEnums.ContainsKey(enumName))
                    {
                        return (Type?)s_builtEnums[enumName];
                    }

                    Type enumType = typeof(int);

                    if (vars.Count > 0 && vars[0] is { } var)
                    {
                        enumType = var.GetType();
                    }

                    EnumBuilder enumBuilder = ModuleBuilder.DefineEnum(enumName, TypeAttributes.Public, enumType);
                    for (int i = 0; i < strings.Count; i++)
                    {
                        enumBuilder.DefineLiteral(strings[i], vars[i]);
                    }

                    Type type = enumBuilder.CreateTypeInfo().AsType();
                    s_builtEnums[enumName] = type;
                    return type;
                }
                finally
                {
                    enumTypeInfo->ReleaseTypeAttr(pTypeAttr);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }

            return null;
        }

        private static unsafe void ProcessVariables(
            ITypeInfo* typeInfo,
            IDictionary<string, PropertyInfo> propertyInfo,
            int dispidToGet,
            int nameDispID)
        {
            TYPEATTR* pTypeAttr = null;
            HRESULT hr = typeInfo->GetTypeAttr(&pTypeAttr);
            if (!hr.Succeeded || pTypeAttr is null)
            {
                throw new ExternalException(string.Format(SR.TYPEINFOPROCESSORGetTypeAttrFailed, hr), (int)hr);
            }

            try
            {
                for (uint i = 0; i < pTypeAttr->cVars; i++)
                {
                    VARDESC* pVarDesc = null;
                    hr = typeInfo->GetVarDesc(i, &pVarDesc);
                    if (!hr.Succeeded || pVarDesc is null)
                    {
                        Debug.WriteLineIf(
                            DbgTypeInfoProcessorSwitch.TraceVerbose,
                            $"ProcessTypeInfoEnum: ignoring variable item 0x{i:X} because Oleaut32.ITypeInfo::GetFuncDesc returned hr=0x{hr:X} or NULL");
                        continue;
                    }

                    try
                    {
                        if (pVarDesc->varkind == VARKIND.VAR_CONST
                            || (dispidToGet != PInvoke.MEMBERID_NIL && pVarDesc->memid != dispidToGet))
                        {
                            continue;
                        }

                        PropertyInfo pi = ProcessDataCore(
                            typeInfo,
                            propertyInfo,
                            pVarDesc->memid,
                            nameDispID,
                            in pVarDesc->elemdescVar.tdesc,
                            pVarDesc->wVarFlags);

                        if (pi.ReadOnly != PropertyInfo.ReadOnlyState.True)
                        {
                            pi.ReadOnly = PropertyInfo.ReadOnlyState.False;
                        }
                    }
                    finally
                    {
                        typeInfo->ReleaseVarDesc(pVarDesc);
                    }
                }
            }
            finally
            {
                typeInfo->ReleaseTypeAttr(pTypeAttr);
            }
        }

        private static Type? VTToType(VARENUM vt) => vt switch
        {
            VT_EMPTY or VT_NULL => null,
            VT_I1 => typeof(sbyte),
            VT_UI1 => typeof(byte),
            VT_I2 => typeof(short),
            VT_UI2 => typeof(ushort),
            VT_I4 or VT_INT => typeof(int),
            VT_UI4 or VT_UINT => typeof(uint),
            VT_I8 => typeof(long),
            VT_UI8 => typeof(ulong),
            VT_R4 => typeof(float),
            VT_R8 => typeof(double),
            VT_CY => typeof(decimal),
            VT_DATE => typeof(DateTime),
            VT_BSTR or VT_LPSTR or VT_LPWSTR => typeof(string),
            VT_DISPATCH => typeof(IDispatch),
            VT_UNKNOWN => typeof(object),
            VT_ERROR or VT_HRESULT => typeof(int),
            VT_BOOL => typeof(bool),
            VT_VARIANT => typeof(Com2Variant),
            VT_CLSID => typeof(Guid),
            VT_FILETIME => typeof(Runtime.InteropServices.ComTypes.FILETIME),
            VT_USERDEFINED => throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "VT_USERDEFINED")),
            _ => throw new ArgumentException(string.Format(SR.COM2UnhandledVT, ((int)vt).ToString(CultureInfo.InvariantCulture))),
        };
    }
}
