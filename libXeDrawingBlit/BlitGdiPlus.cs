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
    public class BlitGdiPlus : IBlit, IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;
        private readonly bool _isIndexed;

        public BlitGdiPlus(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new NullReferenceException("Bitmap cannot be null.");
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Indexed:
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    _isIndexed = true;
                    break;
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                case PixelFormat.Max:
                    _isIndexed = false;
                    break;
                case PixelFormat.Gdi:
                case PixelFormat.Alpha:
                case PixelFormat.PAlpha:
                case PixelFormat.Extended:
                case PixelFormat.Canonical:
                case PixelFormat.Undefined:
                    _isIndexed = false;
                    break;
            }
            _bitmap = bitmap;
            _graphics = Graphics.FromImage(_bitmap);
        }

        public BlitGdiPlus(Size size) :
            this(size.Width, size.Height)
        {
        }

        public BlitGdiPlus(int width, int height) :
            this(new Bitmap(width, height))
        {
        }

        public BlitGdiPlus(int width, int height, PixelFormat pixelFormat) :
            this(new Bitmap(width, height, pixelFormat))
        {
        }
        ~BlitGdiPlus() { Dispose(); }

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
                _graphics.Flush();
                return _bitmap;
            }
        }

        public void DrawImage(IBlit blit, int x, int y)
        {
            var size = blit.GetSize();
            DrawImage(blit, x, y, new Rectangle(0, 0, size.Width, size.Height));
        }

        public void DrawImage(IBlit blit, int x, int y, int srcx, int srcy, int srcwidth, int srcheight)
        {
            DrawImage(blit, x, y, new Rectangle(srcx, srcy, srcwidth, srcheight));
        }

        public void DrawImage(IBlit blit, int x, int y, Rectangle srcrect)
        {
            var image = blit.GetBitmap();
            _graphics.DrawImage(image, x, y, srcrect, GraphicsUnit.Pixel);
        }

        public void DrawImage(IBlit blit, Point pos)
        {
            DrawImage(blit, pos.X, pos.Y);
        }

        public void DrawImage(IBlit blit, Point pos, int srcx, int srcy, int srcwidth, int srcheight)
        {
            DrawImage(blit, pos.X, pos.Y, new Rectangle(srcx, srcy, srcwidth, srcheight));
        }

        public void DrawImage(IBlit blit, Point pos, Rectangle srcrect)
        {
            DrawImage(blit, pos.X, pos.Y, srcrect);
        }

        public Bitmap GetBitmap()
        {
            return Bitmap;
        }

        public void Clear(int color)
        {
            Clear(Color.FromArgb(color));
        }
        public void Clear(Color color)
        {
            _graphics.Clear(color);
        }
        public int GetPixel(int x, int y)
        {
            return GetPixelColor(x, y).ToArgb();
        }

        public Color GetPixelColor(int x, int y)
        {
            return GetBitmap().GetPixel(x, y);
        }

        public Size GetSize()
        {
            return Size;
        }

        public bool IsIndexed()
        {
            return Indexed;
        }

        public void SetPixel(int x, int y, int data)
        {
            SetPixelColor(x, y, Color.FromArgb(data));
        }

        public void SetPixelColor(int x, int y, Color color)
        {
            GetBitmap().SetPixel(x, y, color);
        }

        public void MakeTransparent(int data)
        {
            MakeTransparent(Color.FromArgb(data));
        }

        public void MakeTransparent(Color color)
        {
            _bitmap.MakeTransparent(color);
        }

        public void Dispose()
        {
            _graphics.Dispose();
            _bitmap.Dispose();
        }
    }
}