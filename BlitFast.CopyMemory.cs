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
using System.Runtime.InteropServices;

namespace Xe.Drawing
{
    public partial class BlitFast
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private static void CopyMemory(IntPtr ptrdst, IntPtr ptrsrc, uint length, int stridedst, int stridesrc,
            int height)
        {
            for (var i = 0; i < height; i++)
            {
                CopyMemory(ptrdst, ptrsrc, length);
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }

        private static unsafe void CopyMemory128(IntPtr ptrdst, IntPtr ptrsrc, int stridedst, int stridesrc, int height)
        {
            for (var i = 0; i < height; i++)
            {
                var dst = (long*)ptrdst;
                var src = (long*)ptrsrc;
                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                dst[4] = src[4];
                dst[5] = src[5];
                dst[6] = src[6];
                dst[7] = src[7];
                dst[8] = src[8];
                dst[9] = src[9];
                dst[10] = src[10];
                dst[11] = src[11];
                dst[12] = src[12];
                dst[13] = src[13];
                dst[14] = src[14];
                dst[15] = src[15];
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }

        private static unsafe void CopyMemory64(IntPtr ptrdst, IntPtr ptrsrc, int stridedst, int stridesrc, int height)
        {
            for (var i = 0; i < height; i++)
            {
                var dst = (long*)ptrdst;
                var src = (long*)ptrsrc;
                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                dst[4] = src[4];
                dst[5] = src[5];
                dst[6] = src[6];
                dst[7] = src[7];
                dst[8] = src[8];
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }

        private static unsafe void CopyMemory32(IntPtr ptrdst, IntPtr ptrsrc, int stridedst, int stridesrc, int height)
        {
            for (var i = 0; i < height; i++)
            {
                var dst = (long*)ptrdst;
                var src = (long*)ptrsrc;
                dst[0] = src[0];
                dst[1] = src[1];
                dst[2] = src[2];
                dst[3] = src[3];
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }

        private static unsafe void CopyMemory16(IntPtr ptrdst, IntPtr ptrsrc, int stridedst, int stridesrc, int height)
        {
            for (var i = 0; i < height; i++)
            {
                var dst = (long*)ptrdst;
                var src = (long*)ptrsrc;
                dst[0] = src[0];
                dst[1] = src[1];
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }

        private static unsafe void CopyMemory8(IntPtr ptrdst, IntPtr ptrsrc, int stridedst, int stridesrc, int height)
        {
            for (var i = 0; i < height; i++)
            {
                var dst = (long*)ptrdst;
                var src = (long*)ptrsrc;
                *dst = *src;
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }

        private static unsafe void CopyMemory4(IntPtr ptrdst, IntPtr ptrsrc, int stridedst, int stridesrc, int height)
        {
            for (var i = 0; i < height; i++)
            {
                var dst = (int*)ptrdst;
                var src = (int*)ptrsrc;
                *dst = *src;
                ptrdst += stridedst;
                ptrsrc += stridesrc;
            }
        }
    }
}
