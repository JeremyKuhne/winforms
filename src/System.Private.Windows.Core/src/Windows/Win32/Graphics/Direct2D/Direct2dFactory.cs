// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Direct2D;

/// <summary>
///  <see cref="ID2D1Factory"/> wrapper.
/// </summary>
/// <returns/>
/// <inheritdoc cref="PInvokeCore.D2D1CreateFactory(D2D1_FACTORY_TYPE, Guid*, D2D1_FACTORY_OPTIONS*, void**)"/>
internal unsafe class Direct2dFactory : DirectDrawBase<ID2D1Factory>
{
    public Direct2dFactory(
        D2D1_FACTORY_TYPE factoryType = D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED,
        D2D1_DEBUG_LEVEL factoryOptions = D2D1_DEBUG_LEVEL.D2D1_DEBUG_LEVEL_NONE)
        : base(Create(factoryType, factoryOptions))
    {
    }

    private static ID2D1Factory* Create(D2D1_FACTORY_TYPE factoryType, D2D1_DEBUG_LEVEL factoryOptions)
    {
        ID2D1Factory* factory;
        PInvokeCore.D2D1CreateFactory(
            factoryType,
            IID.Get<ID2D1Factory>(),
            (D2D1_FACTORY_OPTIONS*)&factoryOptions,
            (void**)&factory).ThrowOnFailure();

        return factory;
    }

    private static Direct2dFactory? s_direct2DFactory;

    internal static Direct2dFactory Shared => s_direct2DFactory ??= new Direct2dFactory();
}
