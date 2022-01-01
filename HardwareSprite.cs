using GLGraphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TGELayerDraw
{
    public class HardwareSprite : Sprite
    {
        protected Texture2D texture = new Texture2D();

        private TextureFilter textureFilter = TextureFilter.Nearest;

        public HardwareSprite(Sprite sprite) : base(sprite) { InitializeTexture(texture, this); }

        public HardwareSprite(Vector2i dimensions) : base(dimensions) { InitializeTexture(texture, this); }

        public HardwareSprite(string path) : base(path) { InitializeTexture(texture, this); }

        public HardwareSprite(Stream stream) : base(stream) { InitializeTexture(texture, this); }

        public HardwareSprite(Bitmap bmp) : base(bmp) { InitializeTexture(texture, this); }

        public HardwareSprite(int x, int y) : base(x, y) { InitializeTexture(texture, this); }

        public TextureFilter TextureFilter
        {
            get => textureFilter;
            set
            {
                textureFilter = value;
                texture.Filter = (GLGraphics.TextureFilter)value;
                texture.GenMipmaps();
            }
        }

        public Texture2D OpenGLTexture { get => texture; set => texture = value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetLevels(int width, int height)
        {
            int levels = 1;
            while (((width | height) >> levels) != 0)
            {
                ++levels;
            }
            return levels;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void InitializeTexture(Texture2D texture, Sprite s)
        {
            texture.Init(s.Width, s.Height, TextureFormat.Rgba16f, GetLevels(s.Width, s.Height));
            texture.SetImage(s.Data, 0, 0, s.Width, s.Height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, OpenTK.Graphics.OpenGL4.PixelType.Float);
            texture.Filter = GLGraphics.TextureFilter.Nearest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateVisual()
        {
            texture.SetImage(Data, 0, 0, width, height, OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, OpenTK.Graphics.OpenGL4.PixelType.Float);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateMipmaps()
        {
            texture.GenMipmaps();
        }
    }
}
