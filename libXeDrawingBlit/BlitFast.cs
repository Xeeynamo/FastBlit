// Copyright (c) 2017, Luciano (Xeeynamo) Ciccariello
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Xe.Drawing
{
    public partial class BlitFast : IBlit, IDisposable
    {
        private const string ErrorIndexed = "Unable to operate with colors on an indexed bitmap.";
        private const string OutOfRange = "x or y is out of bounds.";

        private readonly Bitmap _bitmap;
        private readonly int _bpp;
        private readonly bool _isIndexed;
        private readonly PixelFormat _pixelFormat;

        private BitmapData _bitmapData;
        private bool _canBeTransparent;
        private bool _isLocked;

        public BlitFast(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new NullReferenceException("Bitmap cannot be null.");
            _bitmap = bitmap;
            _pixelFormat = GetCompatiblePixelFormat(_bitmap.PixelFormat, out _bpp, out _canBeTransparent);
            _isIndexed = _bpp <= 8;
            _isLocked = false;
            Lock();
        }

        public BlitFast(Size size) :
            this(size.Width, size.Height)
        {
        }

        public BlitFast(int width, int height) :
            this(new Bitmap(width, height))
        {
        }

        public BlitFast(int width, int height, PixelFormat pixelFormat) :
            this(new Bitmap(width, height, pixelFormat))
        {
        }
        ~BlitFast() { Dispose(); }

        public Size Size
        {
            get { return new Size(_bitmap.Width, _bitmap.Height); }
        }

        public bool Indexed
        {
            get { return _isIndexed; }
        }

        public Bitmap Bitmap
        {
            get
            {
                Unlock();
                return _bitmap;
            }
        }

        public Size GetSize()
        {
            return Size;
        }

        public bool IsIndexed()
        {
            return Indexed;
        }

        public Bitmap GetBitmap()
        {
            return Bitmap;
        }

        public unsafe int GetPixel(int x, int y)
        {
            if (_isIndexed)
                throw new InvalidOperationException(ErrorIndexed);
            if (x < 0 || x >= _bitmap.Width ||
                y < 0 || y >= _bitmap.Height)
                throw new ArgumentOutOfRangeException(OutOfRange);
            var pos = x * _bpp / 8 + y * _bitmapData.Stride;
            if (_isIndexed)
                return ((byte*) _bitmapData.Scan0)[pos];
            var b = ((byte*) _bitmapData.Scan0)[pos + 0];
            var g = ((byte*) _bitmapData.Scan0)[pos + 1];
            var r = ((byte*) _bitmapData.Scan0)[pos + 2];
            var a = ((byte*) _bitmapData.Scan0)[pos + 3];
            return b | (g << 8) | (r << 16) | (a << 24);
        }

        public unsafe void SetPixel(int x, int y, int data)
        {
            if (_isIndexed)
                throw new InvalidOperationException(ErrorIndexed);
            if (x < 0 || x >= _bitmap.Width ||
                y < 0 || y >= _bitmap.Height)
                throw new ArgumentOutOfRangeException(OutOfRange);
            var pos = x * _bpp / 8 + y * _bitmapData.Stride;
            if (!_isIndexed)
            {
                ((byte*) _bitmapData.Scan0)[pos + 0] = (byte) (data >> 0);
                ((byte*) _bitmapData.Scan0)[pos + 1] = (byte) (data >> 8);
                ((byte*) _bitmapData.Scan0)[pos + 2] = (byte) (data >> 16);
                var a = (byte) (data >> 24);
                _canBeTransparent |= a < 255;
                ((byte*) _bitmapData.Scan0)[pos + 3] = a;
            }
            else
            {
                ((byte*) _bitmapData.Scan0)[pos] = (byte) data;
            }
        }

        public unsafe void MakeTransparent(int data)
        {
            if (_isIndexed)
                throw new InvalidOperationException(ErrorIndexed);
            var ptr = (int*) _bitmapData.Scan0;
            var end = (int*) (_bitmapData.Scan0 + _bitmapData.Stride * _bitmapData.Height);
            data &= 0xFFFFFF; // Non mi serve il canale di trasparenza.
            while (ptr < end)
            {
                if ((*ptr & 0xFFFFFF) == data)
                    // Azzero l'alpha channel senza togliere il colore.
                    *ptr = data;
                ptr++;
            }
            _canBeTransparent = true;
        }

        public unsafe void Clear(int color)
        {
            if (_isIndexed)
                throw new InvalidOperationException(ErrorIndexed);
            var ptr = (int*)_bitmapData.Scan0;
            var end = (int*)(_bitmapData.Scan0 + _bitmapData.Stride * _bitmapData.Height);
            while (ptr < end)
                *ptr++ = color;
        }
        public void Clear(Color color)
        {
            Clear(color.ToArgb());
        }

        public Color GetPixelColor(int x, int y)
        {
            return Color.FromArgb(GetPixel(x, y));
        }

        public void SetPixelColor(int x, int y, Color color)
        {
            SetPixel(x, y, color.ToArgb());
        }

        public void MakeTransparent(Color color)
        {
            MakeTransparent(color.ToArgb());
        }

        public void DrawImage(IBlit blit, int x, int y)
        {
            var size = blit.GetSize();
            DrawImage(blit, x, y, 0, 0, size.Width, size.Height);
        }

        public void DrawImage(IBlit blit, int x, int y, int srcx, int srcy,
            int srcwidth, int srcheight)
        {
            var fast = blit as BlitFast;
            var internalBlit = fast ?? new BlitFast(blit.GetBitmap());

            Lock();
            if (internalBlit._canBeTransparent)
                InternalDrawImageAlpha(internalBlit, x, y, srcx, srcy, srcwidth, srcheight);
            else
                InternalDrawImage(internalBlit, x, y, srcx, srcy, srcwidth, srcheight);
        }

        public void DrawImage(IBlit blit, int x, int y, Rectangle srcrect)
        {
            DrawImage(blit, x, y, srcrect.X, srcrect.Y, srcrect.Width, srcrect.Height);
        }

        public void DrawImage(IBlit blit, Point pos)
        {
            DrawImage(blit, pos.X, pos.Y);
        }

        public void DrawImage(IBlit blit, Point pos, int srcx, int srcy,
            int srcwidth, int srcheight)
        {
            DrawImage(blit, pos.X, pos.Y, srcx, srcy, srcwidth, srcheight);
        }

        public void DrawImage(IBlit blit, Point pos, Rectangle srcrect)
        {
            DrawImage(blit, pos.X, pos.Y, srcrect.X, srcrect.Y,
                srcrect.Width, srcrect.Height);
        }

        public void Dispose()
        {
            Unlock();
            _bitmap.Dispose();
        }

        private static PixelFormat GetCompatiblePixelFormat(PixelFormat pixelFormat, out int bpp, out bool isAlpha)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    bpp = 8;
                    isAlpha = false;
                    return PixelFormat.Format8bppIndexed;
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    bpp = 32;
                    isAlpha = false;
                    return PixelFormat.Format32bppArgb;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format32bppArgb:
                    bpp = 32;
                    isAlpha = true;
                    return PixelFormat.Format32bppArgb;
                default:
                    throw new NotImplementedException(pixelFormat.ToString());
            }
        }

        private void Lock()
        {
            if (_isLocked) return;
            _isLocked = true;
            var rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            _bitmapData = _bitmap.LockBits(rect, ImageLockMode.ReadWrite, _pixelFormat);
        }

        private void Unlock()
        {
            if (!_isLocked) return;
            _isLocked = false;
            _bitmap.UnlockBits(_bitmapData);
        }

    }
}