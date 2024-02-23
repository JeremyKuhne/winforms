// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Dxgi.Common;
using Windows.Win32.Graphics.GdiPlus;

namespace Windows.Win32.Graphics.Direct2D;

internal unsafe class RenderTarget : Resource, IPointer<ID2D1RenderTarget>
{
    public unsafe new ID2D1RenderTarget* Pointer => (ID2D1RenderTarget*)base.Pointer;

    private int _beginPaintCount;

    public RenderTarget(ID2D1RenderTarget* renderTarget) : base((ID2D1Resource*)renderTarget)
    {
    }

    public bool InBeginDraw => _beginPaintCount > 0;

    public Bitmap CreateBitmapFromGdiPlusBitmap<TBitmap>(TBitmap bitmap)
       where TBitmap : IPointer<GpBitmap>, IPointer<GpImage>
    {
        PixelFormat pixelFormat = bitmap.GetPixelFormat();
        RectangleF bounds = bitmap.GetImageBounds();

        const int BytesPerPixel = 4;

        // We could let GDI+ do the buffer allocation, but for illustrative purposes I've done it here.
        // Note that GDI+ always copies the data, even if it internally is in the desired format.
        using BufferScope<byte> buffer = new((int)bounds.Width * (int)bounds.Height * BytesPerPixel);

        fixed (byte* b = buffer)
        {
            BitmapData bitmapData = new()
            {
                Width = (uint)bounds.Width,
                Height = (uint)bounds.Height,
                Stride = (int)bounds.Width * BytesPerPixel,
                PixelFormat = (int)PixelFormat.Format32bppArgb,
                Scan0 = b
            };

            bitmap.LockBits(
                new((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height),
                ImageLockMode.ImageLockModeUserInputBuf | ImageLockMode.ImageLockModeRead,
                PixelFormat.Format32bppArgb,
                ref bitmapData);

            D2D1_BITMAP_PROPERTIES bitmapProperties = new()
            {
                pixelFormat = new D2D1_PIXEL_FORMAT
                {
                    format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                    alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_IGNORE
                },
                dpiX = 96,
                dpiY = 96
            };

            ID2D1Bitmap* newBitmap;
            HRESULT result = Pointer->CreateBitmap(
                new((uint)bounds.Width, (uint)bounds.Height),
                b,
                (uint)bitmapData.Stride,
                &bitmapProperties,
                &newBitmap);

            bitmap.UnlockBits(ref bitmapData);
            result.ThrowOnFailure();

            GC.KeepAlive(this);
            return new Bitmap(newBitmap);
        }
    }

    public void BeginDraw()
    {
        _beginPaintCount++;

        // Maybe a better answer here is to allow this, just return and call .Flush
        // instead of .EndDraw when "nested"
        Debug.Assert(_beginPaintCount == 1, "BeginDraw must be balanced by EndDraw");

        Pointer->BeginDraw();
        DebugFlush();
        GC.KeepAlive(this);
    }

    public void EndDraw(out bool recreateTarget)
    {
        _beginPaintCount--;

        ulong tag1;
        ulong tag2;
        HRESULT result = Pointer->EndDraw(&tag1, &tag2);

        if (result == HRESULT.D2DERR_RECREATE_TARGET)
        {
            recreateTarget = true;
        }
        else
        {
            result.ThrowOnFailure();
            recreateTarget = false;
        }

        GC.KeepAlive(this);
    }

    public void Flush()
    {
        Pointer->Flush().ThrowOnFailure();
        GC.KeepAlive(this);
    }

    [Conditional("DEBUG")]
    public void DebugFlush() => Flush();

    public void Clear(Color color)
    {
        D2D1_COLOR_F colorf = (D2D1_COLOR_F)color;
        Pointer->Clear(&colorf);
        DebugFlush();
        GC.KeepAlive(this);
    }

    public void FillRectangle(RectangleF rect, Brush brush)
    {
        D2D_RECT_F rectf = (D2D_RECT_F)rect;
        Pointer->FillRectangle(&rectf, brush.Pointer);
        DebugFlush();
        GC.KeepAlive(this);
        GC.KeepAlive(brush);
    }

    public SolidColorBrush CreateSolidColorBrush(Color color)
    {
        ID2D1SolidColorBrush* solidColorBrush;
        D2D1_COLOR_F colorf = (D2D1_COLOR_F)color;
        Pointer->CreateSolidColorBrush(&colorf, null, &solidColorBrush).ThrowOnFailure();
        GC.KeepAlive(this);
        return new SolidColorBrush(solidColorBrush);
    }

    public void DrawBitmap(
        Bitmap bitmap,
        RectangleF destinationRectangle = default,
        RectangleF sourceRectangle = default,
        float opacity = 1.0f,
        D2D1_BITMAP_INTERPOLATION_MODE interpolationMode = D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR)
    {
        D2D_RECT_F destination = (D2D_RECT_F)destinationRectangle;
        if (destinationRectangle.IsEmpty)
        {
            D2D_SIZE_F size = Pointer->GetSizeHack();
            destination = new D2D_RECT_F { left = 0, top = 0, right = size.width, bottom = size.height };
        }
        else
        {
            destination = (D2D_RECT_F)destinationRectangle;
        }

        D2D_RECT_F source = (D2D_RECT_F)sourceRectangle;
        Pointer->DrawBitmap(
            bitmap.Pointer,
            &destination,
            opacity,
            interpolationMode,
            sourceRectangle.IsEmpty ? null : &source);

        DebugFlush();

        GC.KeepAlive(this);
        GC.KeepAlive(bitmap);
    }

    public static implicit operator ID2D1RenderTarget*(RenderTarget renderTarget) => renderTarget.Pointer;
}
