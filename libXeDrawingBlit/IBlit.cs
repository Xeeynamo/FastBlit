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

using System.Drawing;

namespace Xe.Drawing
{
    /// <summary>
    /// Drawing between images.
    /// </summary>
    public interface IBlit
    {
        /// <summary>
        /// Get the size of blit surface.
        /// </summary>
        /// <returns>Blit surface's size</returns>
        Size GetSize();

        /// <summary>
        /// Check if the surface is indexed or not.
        /// An indexed image contains a palette and every "pixel" is an index
        /// to the color table.
        /// On a bitmap, every pixel is a color.
        /// </summary>
        /// <returns>true if it is indexed, false if it is a bitmap.</returns>
        bool IsIndexed();

        /// <summary>
        /// Get the image.
        /// </summary>
        /// <returns>The processed image</returns>
        Bitmap GetBitmap();

        /// <summary>
        /// Get the pixel from the specified position.
        /// This does work only if IsIndexed() is false.
        /// </summary>
        /// <param name="x">X position. 0 &lt;= x &lt; GetSize().Width</param>
        /// <param name="y">Y position. 0 &lt;= y &lt; GetSize().Height</param>
        /// <returns>Pixel data</returns>
        int GetPixel(int x, int y);

        /// <summary>
        /// Set the pixel to the specified position.
        /// This does work only if IsIndexed() is false.
        /// </summary>
        /// <param name="x">X position. 0 &lt;= x &lt; GetSize().Width</param>
        /// <param name="y">Y position. 0 &lt;= y &lt; GetSize().Height</param>
        /// <param name="data"></param>
        void SetPixel(int x, int y, int data);

        /// <summary>
        /// Set as transparent every pixel equals to the specified color.
        /// The specified alpha channel will be ignored.
        /// </summary>
        /// <param name="data">Color data to make transparent</param>
        void MakeTransparent(int data);

        /// <summary>
        /// Get the pixel from the specified position.
        /// This does work only if IsIndexed() is false.
        /// </summary>
        /// <param name="x">X position. 0 &lt;= x &lt; GetSize().Width</param>
        /// <param name="y">Y position. 0 &lt;= y &lt; GetSize().Height</param>
        /// <returns>The color at that pixel.</returns>
        Color GetPixelColor(int x, int y);

        /// <summary>
        /// Set the pixel to the specified position.
        /// This does work only if IsIndexed() is false.
        /// </summary>
        /// <param name="x">X position. 0 &lt;= x &lt; GetSize().Width</param>
        /// <param name="y">Y position. 0 &lt;= y &lt; GetSize().Height</param>
        /// <param name="color">Color to write.</param>
        void SetPixelColor(int x, int y, Color color);

        /// <summary>
        /// Set as transparent every pixel equals to the specified color.
        /// The specified alpha channel will be ignored.
        /// </summary>
        /// <param name="data">Color to make transparent</param>
        void MakeTransparent(Color color);

        void DrawImage(IBlit blit, int x, int y);
        void DrawImage(IBlit blit, int x, int y, int srcx, int srcy, int srcwidth, int srcheight);
        void DrawImage(IBlit blit, int x, int y, Rectangle srcrect);
        void DrawImage(IBlit blit, Point pos);
        void DrawImage(IBlit blit, Point pos, int srcx, int srcy, int srcwidth, int srcheight);
        void DrawImage(IBlit blit, Point pos, Rectangle srcrect);
    }
}