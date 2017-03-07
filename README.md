## Xe.Drawing.Blit

C# library that allows to draw images into images.

Performances was the primary goal: it is way faster than the
System.Drawing.Graphics component in .Net Framework.
Why? Microsoft solution uses Gdi Plus, that allows various features like matrix
transformation, filtering and scaling. If you want to draw images only without
applying complex features, then FastBlit is your choice!

## How does it works?

It is completely CPU based: no hardware acceleration is involved. If you are
searching great performances with games or particle system, please use DirectX.

The BlitFast object can be created from an image or a specified size. If alpha
channel is involved, then the object is flagged as alpha-blending capable.
When a BlitFast object that it is completely opaque is blitten, it is directly
copied into memory. If it can be partially transparent, the slower
alpha-blending algorithm will be applied.

How the direct memory copying works? For huge data I am calling CopyMemory API
form Win32's Kernel32.dll. If the data is 1 << n aligned, then a custom method
called CopyMemory* is called. It is essentially an unrolled loop that speed-up
tremendously the copy. C# is compiled as bytecode, so it cannot be optimized
like the memcpy of C; this was the best idea that I had to speed-up memory
copying and I hope that the JIT compiler will optimize it as MMX or SSE code.

BE WARN that the library is flagged as unsafe due to its low-level memory
reading and writing. It is not fully tested and some OutOfRangeException can
pop-up. Please report bugs or create some pull-requests to finalize this lib!


## What it does support

- Bitmap and indexed images
- Draw a portion of image into another
- Get and set pixels on bitmap images
- Alpha blending


## What it does not support

- Bitmap to / from indexed images conversion
- Stretch, rotation and filtering
- Flipping images horizontally or vertically


## What is missing (in order of priority)

- Alpha blending is not properly supported (yet)
- Flip images horizontally or vertically
- Better indexed images management and palette support
- Multi-threaded blitting
- Custom bitmap loading to remove System.Drawing dependency for .Net Core
- A lower-level API written in C


## What about the license?
It is MIT licensed. You can use it for commercial use, distribuite it and edit
it. Just do not remove the copyright notes :) .