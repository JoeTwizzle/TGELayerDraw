using GLGraphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGELayerDraw
{
    public class Layer : HardwareSprite, IDisposable
    {
        FrameBuffer FrameBuffer;
        public Layer(Vector2i dimensions) : this(dimensions.X, dimensions.Y) { }

        public Layer(int x, int y) : base(x, y)
        {
            FrameBuffer = new FrameBuffer();
            FrameBuffer.AttachTexture(texture, OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment0);
            Data = new Color4[x * y];
            Clear(Color4.Transparent);
        }

        public void BindTexture(int index)
        {
            texture.Bind(index);
        }

        public void SetActive()
        {
            FrameBuffer.Bind();
        }

        public static void SetNoneActive()
        {
            FrameBuffer.BindDefault();
        }

        public void Dispose()
        {
            FrameBuffer.DetachTexture(OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment0);
            texture.Dispose();
            FrameBuffer.Dispose();
        }
    }
}
