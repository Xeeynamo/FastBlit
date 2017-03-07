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

namespace Xe.Drawing
{
    public partial class BlitFast
    {
        private static byte InterpolateByteColor(byte dst, byte src, byte alpha)
        {
            var r = dst * alpha / byte.MaxValue +
                    src * (byte.MaxValue - alpha) / byte.MaxValue;
            return (byte)r;
        }

        private void InternalDrawImage(BlitFast blit, int x, int y,
            int srcx, int srcy, int srcwidth, int srcheight)
        {
            if (x < 0)
            {
                srcx += x;
                srcwidth += x;
                x = 0;
            }
            if (y < 0)
            {
                srcy += y;
                srcheight += y;
                y = 0;
            }
            if (srcx < 0)
            {
                srcwidth += srcx;
                srcx = 0;
            }
            if (srcy < 0)
            {
                srcheight += srcy;
                srcy = 0;
            }
            if (srcwidth <= 0 || srcheight <= 0) return;
            if (srcx + srcwidth > _bitmapData.Width)
                srcwidth = _bitmapData.Width - srcx;
            if (srcy + srcheight > _bitmapData.Height)
                srcheight = _bitmapData.Height - srcy;
            if (srcwidth <= 0 || srcheight <= 0) return;

            var blockLenght = srcwidth * blit._bpp / 8;
            if (_bitmapData.Stride == blit._bitmapData.Stride)
            {
                var ptrdst = _bitmapData.Scan0 + y * blockLenght;
                blit.Lock();
                var ptrsrc = blit._bitmapData.Scan0 + srcy * blockLenght;
                var count = (uint)(srcheight * blockLenght);
                CopyMemory(ptrdst, ptrsrc, count);
            }
            else
            {
                var ptrdst = _bitmapData.Scan0 + x * _bpp / 8 + y * _bitmapData.Stride;
                var ptrsrc = blit._bitmapData.Scan0 + srcx * _bpp / 8 + srcy * blit._bitmapData.Stride;
                var stridedst = _bitmapData.Stride;
                var stridesrc = blit._bitmapData.Stride;
                switch (blockLenght)
                {
                    case 128:
                        CopyMemory128(ptrdst, ptrsrc, stridedst, stridesrc, srcheight);
                        break;
                    case 64:
                        CopyMemory64(ptrdst, ptrsrc, stridedst, stridesrc, srcheight);
                        break;
                    case 32:
                        CopyMemory32(ptrdst, ptrsrc, stridedst, stridesrc, srcheight);
                        break;
                    case 16:
                        CopyMemory16(ptrdst, ptrsrc, stridedst, stridesrc, srcheight);
                        break;
                    case 8:
                        CopyMemory8(ptrdst, ptrsrc, stridedst, stridesrc, srcheight);
                        break;
                    case 4:
                        CopyMemory4(ptrdst, ptrsrc, stridedst, stridesrc, srcheight);
                        break;
                    default:
                        CopyMemory(ptrdst, ptrsrc, (uint)blockLenght,
                            stridedst, stridesrc, srcheight);
                        break;
                }
            }
        }

        private unsafe void InternalDrawImageAlpha(BlitFast blit, int x, int y,
            int srcx, int srcy, int srcwidth, int srcheight)
        {
            if (srcx + srcwidth > _bitmapData.Width)
                srcwidth = _bitmapData.Width - srcx;
            if (srcy + srcheight > _bitmapData.Height)
                srcheight = _bitmapData.Height - srcy;

            // Divido per 4 perché è un 32bpp
            var blockLenght = srcwidth * blit._bpp / 8 / 4;

            var ptrdst = _bitmapData.Scan0 + x * _bpp / 8 + y * _bitmapData.Stride;
            var ptrsrc = blit._bitmapData.Scan0 + srcx * _bpp / 8 + srcy * blit._bitmapData.Stride;
            var stridedst = _bitmapData.Stride;
            var stridesrc = blit._bitmapData.Stride;

            for (var i = 0; i < srcheight; i++)
            {
                var dst = (byte*)ptrdst;
                var src = (byte*)ptrsrc;
                for (var j = 0; j < blockLenght; j++)
                {
                    var a = src[3];
                    if (a > 0)
                        if (a == 0xFF)
                        {
                            dst[0] = src[0];
                            dst[1] = src[1];
                            dst[2] = src[2];
                            dst[3] = src[3];
                        }
                        else
                        {
                            dst[0] = InterpolateByteColor(src[0], dst[0], a);
                            dst[1] = InterpolateByteColor(src[1], dst[1], a);
                            dst[2] = InterpolateByteColor(src[2], dst[2], a);
                            if (dst[3] < a) dst[3] = a;
                        }
                    dst += 4;
                    src += 4;
                }
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }
    }
}
