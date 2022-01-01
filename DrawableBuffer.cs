using OpenTK.Mathematics;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace TGELayerDraw
{
    public abstract class DrawableBuffer<T> where T : unmanaged
    {
        public T[] Data;
        protected int width;
        protected int height;

        public int Width => width;
        public int Height => height;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DrawableBuffer()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public DrawableBuffer(int w, int h)
        {
            width = w;
            height = h;
            Data = new T[w * h];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetPixel(int x, int y, in T borderColor)
        {
            int pos = x + y * width;
            if (x >= 0 && x < width && pos >= 0 && pos < Data.Length)
            {
                return Data[pos];
            }
            return borderColor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(in T color)
        {
            Data.Fill(color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float Sign(in int x3, in int aX, in int aY, in int bX, in int bY, in int cX, in int cY)
        {
            return (aX - x3) * (bY - cY) - (bX - cX) * (aY - cY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void DrawLine(int sx, int ex, int ny, in T color, in BlendMode blendMode, in bool checkBounds)
        {
            for (int i = sx; i <= ex; i++)
                DrawPixel(i, ny, color, blendMode, checkBounds);
        }

        #region DrawPixel
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelChecked(in Vector2i pos, in T color)
        {
            DrawPixelChecked(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelChecked(in int x, in int y, in T color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelCheckedAdd(in Vector2i pos, in T color)
        {
            DrawPixelCheckedAdd(pos.X, pos.Y, color);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelCheckedAdd(in int x, in int y, in T color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelCheckedClip(in Vector2i pos, in T color)
        {
            DrawPixelCheckedClip(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelCheckedClip(in int x, in int y, in T color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelCheckedAlpha(in Vector2i pos, in T color)
        {
            DrawPixelCheckedAlpha(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelCheckedAlpha(in int x, in int y, in T srcColor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelUnchecked(in Vector2i pos, in T color)
        {
            DrawPixelUnchecked(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelUnchecked(in int x, in int y, in T color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelUncheckedAdd(in Vector2i pos, in T color)
        {
            DrawPixelUncheckedAdd(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelUncheckedAdd(in int x, in int y, in T color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelUncheckedClip(in Vector2i pos, in T color)
        {
            DrawPixelUncheckedClip(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelUncheckedClip(in int x, in int y, in T color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixelUncheckedAlpha(in Vector2i pos, in T color)
        {
            DrawPixelUncheckedAlpha(pos.X, pos.Y, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawPixelUncheckedAlpha(in int x, in int y, in T srcColor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixel(in Vector2i pos, in T srcColor, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawPixel(pos.X, pos.Y, srcColor, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPixel(in int x, in int y, in T srcColor, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            if (checkBounds)
            {
                switch (blendMode)
                {
                    case BlendMode.None:
                        DrawPixelChecked(x, y, srcColor);
                        return;
                    case BlendMode.Add:
                        DrawPixelCheckedAdd(x, y, srcColor);
                        return;
                    case BlendMode.Alpha:
                        DrawPixelCheckedAlpha(x, y, srcColor);
                        return;
                    case BlendMode.Clip:
                        DrawPixelCheckedClip(x, y, srcColor);
                        return;
                    default:
                        return;
                }
            }
            else
            {
                switch (blendMode)
                {
                    case BlendMode.None:
                        DrawPixelUnchecked(x, y, srcColor);
                        return;
                    case BlendMode.Add:
                        DrawPixelUncheckedAdd(x, y, srcColor);
                        return;
                    case BlendMode.Alpha:
                        DrawPixelUncheckedAlpha(x, y, srcColor);
                        return;
                    case BlendMode.Clip:
                        DrawPixelUncheckedClip(x, y, srcColor);
                        return;
                    default:
                        return;
                }
            }
        }
        #endregion

        #region Box

        #region Outline
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawBox(in Box2i box, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawBox(box.Min.X, box.Min.Y, box.Max.X, box.Max.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawBox(in Vector4i pos, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawBox(pos.X, pos.Y, pos.Z - pos.X, pos.W - pos.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawBox(in Vector2i pos, in Vector2i dim, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawBox(pos.X, pos.Y, dim.X, dim.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawBox(in int x, in int y, int w, int h, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            w -= 1;
            h -= 1;
            for (int i = 1; i < w; i++)
            {
                DrawPixel(x + i, y, color, blendMode, checkBounds);
                DrawPixel(x + i, y + h, color, blendMode, checkBounds);
            }
            for (int i = 0; i <= h; i++)
            {
                DrawPixel(x, y + i, color, blendMode, checkBounds);
                DrawPixel(x + w, y + i, color, blendMode, checkBounds);
            }
        }
        #endregion

        #region Filled
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillBox(in Box2i box, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            FillBox(box.Min.X, box.Min.Y, box.Max.X, box.Max.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillBox(in Vector4i pos, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            FillBox(pos.X, pos.Y, pos.Z, pos.W, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillBox(in Vector2i pos, in Vector2i dim, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            FillBox(pos.X, pos.Y, dim.X, dim.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillBox(in int x, in int y, in int w, in int h, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            if (blendMode != BlendMode.Alpha && !checkBounds)
            {
                for (int i = 0; i < w; i++)
                {
                    DrawPixel(x + i, y, color, blendMode, checkBounds);
                }
                for (int i = 0; i < h; i++)
                {
                    Array.Copy(Data, x + y * width, Data, x + (y + i) * width, w);
                }
            }
            else
            {
                for (int j = 0; j < h; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        DrawPixel(x + i, y + j, color, blendMode, checkBounds);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Circle

        #region Outline
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(in Vector2i pos, in int r, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawCircle(pos.X, pos.Y, r, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(in int x0, in int y0, in int r, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            int x = 0;
            int y = r;
            int p = 3 - (r * 2);

            while (y >= x)
            {
                DrawPixel(x0 - x, y0 - y, color, blendMode, checkBounds);//upper left left
                DrawPixel(x0 - y, y0 - x, color, blendMode, checkBounds);//upper upper left
                DrawPixel(x0 + y, y0 - x, color, blendMode, checkBounds);//upper upper right
                DrawPixel(x0 + x, y0 - y, color, blendMode, checkBounds);//upper right right
                DrawPixel(x0 - x, y0 + y, color, blendMode, checkBounds);//lower left left
                DrawPixel(x0 - y, y0 + x, color, blendMode, checkBounds);//lower lower left
                DrawPixel(x0 + y, y0 + x, color, blendMode, checkBounds);//lower lower right
                DrawPixel(x0 + x, y0 + y, color, blendMode, checkBounds);//lower right right

                if (p < 0) p += 4 * x++ + 6;
                else p += 4 * (x++ - y--) + 10;
            }
        }
        #endregion

        #region Filled

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillCircle(in Vector2i pos, in int r, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            FillCircle(pos.X, pos.Y, r, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillCircle(in int x0, in int y0, in int r, T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            // Taken from wikipedia
            // Taken from OLC console game engine
            int x = 0;
            int y = r;
            int p = 3 - 2 * r;

            while (y >= x)
            {
                // Modified to draw scan-lines instead of edges
                DrawLine(x0 - x, x0 + x, y0 - y, color, blendMode, checkBounds);
                DrawLine(x0 - y, x0 + y, y0 - x, color, blendMode, checkBounds);
                DrawLine(x0 - x, x0 + x, y0 + y, color, blendMode, checkBounds);
                DrawLine(x0 - y, x0 + y, y0 + x, color, blendMode, checkBounds);
                if (p < 0) p += 4 * x++ + 6;
                else p += 4 * (x++ - y--) + 10;
            }
        }
        #endregion

        #endregion

        #region Line
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Vector2i pos1, in Vector2i pos2, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true, int dashes = 0)
        {
            DrawLine(pos1.X, pos1.Y, pos2.X, pos2.Y, color, blendMode, checkBounds, dashes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in int x, in int y, in int x2, in int y2, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true, int dashes = 0)
        {
            var shortLen = y2 - y;
            var longLen = x2 - x;
            bool yLonger = false;
            if ((shortLen ^ (shortLen >> 31)) - (shortLen >> 31) > (longLen ^ (longLen >> 31)) - (longLen >> 31))
            {
                shortLen ^= longLen;
                longLen ^= shortLen;
                shortLen ^= longLen;

                yLonger = true;
            }

            var inc = longLen < 0 ? -1 : 1;

            float multDiff = longLen == 0 ? shortLen : shortLen / (float)longLen;

            if (yLonger)
            {
                for (var i = 0; i != longLen; i += inc)
                {
                    DrawPixel((int)(x + i * multDiff + 0.5f), y + i, color, blendMode, checkBounds);
                }
            }
            else
            {
                for (var i = 0; i != longLen; i += inc)
                {
                    DrawPixel(x + i, (int)(y + i * multDiff + 0.5f), color, blendMode, checkBounds);
                }
            }
        }

        #endregion

        #region Triangle

        #region Outline
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawTriangle(in Vector2i pos1, in Vector2i pos2, in Vector2i pos3, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawTriangle(pos1.X, pos1.Y, pos2.X, pos2.Y, pos3.X, pos3.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawTriangle(in int x1, in int y1, in int x2, in int y2, in int x3, in int y3, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawLine(x1, y1, x2, y2, color, blendMode, checkBounds);
            DrawLine(x2, y2, x3, y3, color, blendMode, checkBounds);
            DrawLine(x3, y3, x1, y1, color, blendMode, checkBounds);
        }

        #endregion

        #region Filled
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillTriangle(in Vector2i pos1, in Vector2i pos2, in Vector2i pos3, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            FillTriangle(pos1.X, pos1.Y, pos2.X, pos2.Y, pos3.X, pos3.Y, color, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillTriangle(in int x1, in int y1, in int x2, in int y2, in int x3, in int y3, in T color, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            var minX = Math.Min(Math.Min(x1, x2), x3);
            var maxX = Math.Max(Math.Max(x1, x2), x3);

            var minY = Math.Min(Math.Min(y1, y2), y3);
            var maxY = Math.Max(Math.Max(y1, y2), y3);

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    float d1, d2, d3;
                    bool hasNeg, hasPos;

                    d1 = Sign(x3, x, y, x1, y1, x2, y2);
                    d2 = Sign(x3, x, y, x2, y2, x3, y3);
                    d3 = Sign(x3, x, y, x3, y3, x1, y1);

                    hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
                    hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

                    if (!(hasNeg && hasPos))
                        DrawPixel(x, y, color, blendMode, checkBounds);
                }
            }
        }
        #endregion

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawSprite(in Vector2i pos, DrawableBuffer<T> sprite, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawSprite(pos.X, pos.Y, sprite, center, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawSprite(int xA, int yA, DrawableBuffer<T> sprite, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            if (center)
            {
                xA -= sprite.width / 2;
                yA -= sprite.height / 2;
            }
            if (blendMode == BlendMode.None)
            {
                int rows = sprite.height + (yA + sprite.height > height ? height - (yA + sprite.height) : 0);
                for (int i = 0; i < rows; i++)
                {
                    int length = sprite.width + (xA < 0 ? xA : 0) + (xA + sprite.width > width ? width - (xA + sprite.width) : 0);
                    int startIdx = i * sprite.width + (xA < 0 ? -xA : 0);
                    int destIdx = Math.Max(Math.Min(xA, width), 0) + Math.Min((i + yA) * width, height * width);
                    if (length > 0 && destIdx >= 0)
                    {
                        Array.Copy(sprite.Data, startIdx, Data, destIdx, length);
                    }
                }
            }
            else
            {
                for (int y = 0; y < sprite.height; y++)
                {
                    for (int x = 0; x < sprite.width; x++)
                    {
                        var srcColor = sprite.Data[x + y * sprite.width];
                        DrawPixel(x + xA, y + yA, srcColor, blendMode, checkBounds);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPartialSprite(in Vector2i posDest, DrawableBuffer<T> sprite, in Vector2i posSrc, in Vector2i dimSrc, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawPartialSprite(posDest.X, posDest.Y, sprite, posSrc.X, posSrc.Y, dimSrc.X, dimSrc.Y, center, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPartialSprite(in Vector2i posDest, DrawableBuffer<T> sprite, in Vector4i pos, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawPartialSprite(posDest.X, posDest.Y, sprite, pos.X, pos.Y, pos.Z, pos.W, center, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPartialSprite(in Vector2i posDest, DrawableBuffer<T> sprite, in Box2i box, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawPartialSprite(posDest.X, posDest.Y, sprite, box.Min.X, box.Min.Y, box.Max.X - box.Min.X, box.Max.Y - box.Min.Y, center, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPartialSprite(in Vector2i posDest, DrawableBuffer<T> spriteB, in int xB, in int yB, in int wB, in int hB, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            DrawPartialSprite(posDest.X, posDest.Y, spriteB, xB, yB, wB, hB, center, blendMode, checkBounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawPartialSprite(int xA, int yA, DrawableBuffer<T> spriteB, in int xB, in int yB, in int wB, in int hB, in bool center = false, in BlendMode blendMode = BlendMode.Clip, in bool checkBounds = true)
        {
            if (center)
            {
                xA -= (wB) / 2;
                yA -= (hB) / 2;
            }

            if (blendMode == BlendMode.None)
            {
                int rows = hB + (yA + (hB - yB) > height ? height - (yA + (hB - yB)) : 0);
                for (int i = yB; i < rows; i++)
                {
                    int length = wB - xB + (xA < 0 ? xA : 0) + (xA + (wB - xB) > width ? width - (xA + (wB - xB)) : 0);
                    int startIdx = xB + i * spriteB.width + (xA < 0 ? -xA : 0);
                    int destIdx = Math.Max(Math.Min(xA, width), 0) + Math.Min((i - yB + yA) * width, height * width);
                    if (length > 0 && destIdx >= 0)
                    {
                        Array.Copy(spriteB.Data, startIdx, Data, destIdx, length);
                    }
                }
            }
            else
            {
                for (int y = 0; y < hB; y++)
                {
                    for (int x = 0; x < wB; x++)
                    {
                        if (x + xB < spriteB.width && x + xB >= 0 && y + yB < spriteB.height && y + yB >= 0)
                        {
                            var srcColor = spriteB.Data[(x + xB) + (y + yB) * spriteB.width];
                            DrawPixel(x + xA, y + yA, srcColor, blendMode, checkBounds);
                        }
                    }
                }
            }
        }
    }
}