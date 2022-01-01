using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Threading.Tasks;
using GLGraphics;
using OpenTK.Mathematics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace TGELayerDraw
{
    public unsafe partial class Sprite : DrawableBuffer<Color4>
    {
        public float OpacityModifier = 1f;
        public Sprite(Sprite sprite)
        {
            width = sprite.width;
            height = sprite.height;
            Data = new Color4[width * height];
            Array.Copy(sprite.Data, Data, width * height);
        }

        public Sprite(Vector2i dimensions) : this(dimensions.X, dimensions.Y) { }

        public Sprite(int x, int y)
        {
            width = x;
            height = y;
            Data = new Color4[x * y];
            Clear(Color4.Transparent);
        }

        public Sprite(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path) + " is null");
            }

            var bmp = (Bitmap)Bitmap.FromFile(path);
            LoadBitmap(bmp, ref width, ref height, out Data);
            bmp.Dispose();
        }

        public Sprite(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream) + " is null");
            }

            var bmp = (Bitmap)Bitmap.FromStream(stream);
            LoadBitmap(bmp, ref width, ref height, out Data);
            bmp.Dispose();
        }

        public Sprite(Bitmap bmp)
        {
            LoadBitmap(bmp, ref width, ref height, out Data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LoadBitmap(Bitmap bmp, ref int width, ref int height, out Color4[] data)
        {
            if (bmp == null)
            {
                throw new ArgumentNullException(nameof(bmp) + " is null");
            }
            width = bmp.Width;
            height = bmp.Height;
            data = new Color4[width * height];
            var imgData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte* dataPtr = (byte*)imgData.Scan0;
            for (int i = 0; i < width * height; i++)
            {
                data[i] = new Color4(dataPtr[i * 4 + 2] / 255f, dataPtr[i * 4 + 1] / 255f, dataPtr[i * 4 + 0] / 255f, dataPtr[i * 4 + 3] / 255f);
            }
            bmp.UnlockBits(imgData);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawPixelCheckedClip(in int x, in int y, in Color4 color)
        {
            int pos = x + y * width;
            if (x >= 0 && x < width && pos >= 0 && pos < Data.Length && color.A > 0.99f)
            {
                DrawPixelUnchecked(x, y, color);
            }
        }

        public override void DrawPixelCheckedAdd(in int x, in int y, in Color4 color)
        {
            int pos = x + y * width;
            if (x >= 0 && x < width && pos >= 0 && pos < Data.Length)
            {
                DrawPixelUncheckedAdd(x, y, color);
            }
        }

        public override void DrawPixelUncheckedAdd(in int x, in int y, in Color4 color)
        {
            int pos = x + y * width;
            Color4 oldColor = Data[pos];
            Vector128<float> src = Vector128.Create(color.R, color.G, color.B, 0);
            Vector128<float> dest = Vector128.Create(oldColor.R, oldColor.G, oldColor.B, 0);
            Vector128<float> srcAlpha = Vector128.Create(color.A);
            var colorn = Fma.MultiplyAdd(src, srcAlpha, dest).AsVector3();
            Data[pos] = new Color4(colorn.X, colorn.Y, colorn.Z, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawPixelCheckedAlpha(in int x, in int y, in Color4 srcColor0)
        {
            Color4 srcColor = srcColor0;
            srcColor.A *= OpacityModifier;
            int pos = x + y * width;
            if (x >= 0 && x < width && pos >= 0 && pos < Data.Length)
            {
                Color4 destColor = Data[pos];
                Vector128<float> src = Vector128.Create(srcColor.R, srcColor.G, srcColor.B, 0);
                Vector128<float> dest = Vector128.Create(destColor.R, destColor.G, destColor.B, 0);
                Vector128<float> srcAlpha = Vector128.Create(srcColor.A);
                Vector128<float> destAlpha = Vector128.Create(1f - srcColor.A);
                Vector128<float> multipliedColorSrc = Sse.Multiply(src, srcAlpha);
                var color = Fma.MultiplyAdd(dest, destAlpha, multipliedColorSrc).AsVector3();
                DrawPixelUnchecked(x, y, new Color4(color.X, color.Y, color.Z, srcColor.A + destColor.A * (1f - srcColor.A)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawPixelChecked(in int x, in int y, in Color4 color)
        {
            int pos = x + y * width;
            if (x >= 0 && x < width && pos >= 0 && pos < Data.Length)
            {
                Data[pos] = color;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawPixelUnchecked(in int x, in int y, in Color4 color)
        {
            Data[x + y * width] = color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawPixelUncheckedClip(in int x, in int y, in Color4 color)
        {
            if (color.A > 0.99f)
            {
                DrawPixelUnchecked(x, y, color);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawPixelUncheckedAlpha(in int x, in int y, in Color4 srcColor0)
        {
            Color4 srcColor = srcColor0;
            srcColor.A *= OpacityModifier;
            Color4 destColor = Data[x + y * width];
            Vector128<float> src = Vector128.Create(srcColor.R, srcColor.G, srcColor.B, 0);
            Vector128<float> dest = Vector128.Create(destColor.R, destColor.G, destColor.B, 0);
            Vector128<float> srcAlpha = Vector128.Create(srcColor.A);
            Vector128<float> destAlpha = Vector128.Create(1f - srcColor.A);
            Vector128<float> multipliedColorSrc = Sse.Multiply(src, srcAlpha);
            var color = Fma.MultiplyAdd(dest, destAlpha, multipliedColorSrc).AsVector3();
            DrawPixelUnchecked(x, y, new Color4(color.X, color.Y, color.Z, srcColor.A + destColor.A * (1f - srcColor.A)));
        }
    }
}
