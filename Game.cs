using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using GLGraphics;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace TGELayerDraw
{
    public class Game : GameWindow, IDisposable
    {
        enum CommandType
        {
            Sprite,
            Partial
        }

        struct DrawCommand
        {
            public CommandType CommandType;
            public Layer Layer;
            public int X, Y, W, H;
            public float Rotation;
            public Sprite Sprite;
            public LayerShader? SpriteShader;
            public BlendMode BlendMode;

            public DrawCommand(Layer layer, int x, int y, float rotation, Sprite sprite, LayerShader? spriteShader, BlendMode blendMode)
            {
                Layer = layer;
                X = x;
                Y = y;
                W = 0;
                H = 0;
                Rotation = rotation;
                Sprite = sprite;
                SpriteShader = spriteShader;
                BlendMode = blendMode;
                CommandType = CommandType.Sprite;
            }

            public DrawCommand(Layer layer, int x, int y, int w, int h, float rotation, Sprite sprite, LayerShader? spriteShader, BlendMode blendMode)
            {
                CommandType = CommandType.Partial;
                Layer = layer;
                X = x;
                Y = y;
                W = w;
                H = h;
                Rotation = rotation;
                Sprite = sprite;
                SpriteShader = spriteShader;
                BlendMode = blendMode;
            }
        }

        public readonly Layer DefaultLayer;
        public Layer ActiveLayer { get; private set; }
        public BlendMode BlendMode = BlendMode.None;
        public Vector2i PixelSize = new Vector2i(4);

        public Vector2i CursorPos { get { return (Vector2i)Vector2.Divide(MousePosition, PixelSize); } }

        const string defaultFrag =
            @"#version 460
            layout(location = 0) in vec2 v_UV;

            layout(binding = 0) uniform sampler2D _MainTex;
            
            out vec4 color;

            void main()
            {
                vec3 actualColor = texture(_MainTex, v_UV).rgb;
                color = vec4(actualColor, 1.0);
            }";

        public readonly LayerShader DefaultShader;
        public LayerShader ActiveShader;
        //List<DrawCommand> drawCommands;
        public Vector2i Dimensions;
        VertexArray dummyVAO;
        public Vector2i GameArea => new Vector2i(ActiveLayer.Width * PixelSize.X, ActiveLayer.Height * PixelSize.Y);
        public Game(Vector2i size) : this(size.X, size.Y)
        {

        }

        public Game(int sizeX, int sizeY) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }
            Dimensions = new Vector2i(sizeX, sizeY);
            ActiveLayer = DefaultLayer = new Layer(sizeX, sizeY);
            FitWindow();
            dummyVAO = new VertexArray();
            //drawCommands = new List<DrawCommand>();
            ActiveShader = DefaultShader = new LayerShader(defaultFrag);
            WindowBorder = WindowBorder.Fixed;
        }

        protected sealed override void OnUpdateFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Update((float)args.Time);
            SwapBuffers();
        }

        public virtual void Update(float dt) { }

        public Layer CreateLayer(Vector2i dimensions)
        {
            return CreateLayer(dimensions.X, dimensions.Y);
        }

        public Layer CreateLayer(int x, int y)
        {
            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }
            return new Layer(x, y);
        }

        public void SetLayer(Layer layer)
        {
            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }
            layer.SetActive();
            ActiveLayer = layer;
        }

        public void FitWindow()
        {
            Vector2i pos = Bounds.Min;
            ClientRectangle = new Box2i(0, 0, ActiveLayer.Width * PixelSize.X, ActiveLayer.Height * PixelSize.Y);
            Bounds = new Box2i(pos, pos + Bounds.Max);
        }

        public void SetViewport(int x, int y, int w, int h)
        {
            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }
            GL.Viewport(x, y, w, h);
        }

        public void DisplaySprite(HardwareSprite layer, float xPercentage = 0, float yPercentage = 0)
        {
            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }
            //GL.Viewport((int)(xPercentage * ClientSize.X), (int)(yPercentage * ClientSize.Y), layer.Width * PixelSize.X, layer.Height * PixelSize.Y);
            layer.OpenGLTexture.Bind(0);
            dummyVAO.Bind();
            ActiveShader.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public void RunShader()
        {
            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }
            dummyVAO.Bind();
            ActiveShader.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public new void Dispose()
        {
            DefaultShader.Dispose();
            Layer.SetNoneActive();
            DefaultLayer.Dispose();
            base.Dispose();
        }
    }
}