using System;
using System.Collections.Generic;
using System.Linq;
using GLGraphics;
using System.Threading.Tasks;

namespace TGELayerDraw
{
    public class LayerShader : IDisposable
    {
        const string DefaultVertexSource =
        @"#version 450
            layout(location = 0) out vec2 texCoord;

            void main()
            {
                float x = -1.0 + float((gl_VertexID & 1) << 2);
                float y = -1.0 + float((gl_VertexID & 2) << 1);
                texCoord.x = (x + 1.0) * 0.5;
                texCoord.y = 1-(y + 1.0) * 0.5;
                gl_Position = vec4(x, y, 0, 1);
            }";
        public GLProgram GLProgram;
        public LayerShader(string fragmentShaderCode)
        {
            GLProgram = new GLProgram();
            GLShader vertexShader = new GLShader(OpenTK.Graphics.OpenGL4.ShaderType.VertexShader);
            GLShader fragmentShader = new GLShader(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader);
            vertexShader.SetSource(DefaultVertexSource);
            fragmentShader.SetSource(fragmentShaderCode);
            GLProgram.AddShader(vertexShader);
            GLProgram.AddShader(fragmentShader);
            GLProgram.LinkProgram();
            vertexShader.Dispose();
            fragmentShader.Dispose();
        }
        public LayerShader(string fragmentShaderCode, string vertexShaderCode)
        {
            GLProgram = new GLProgram();
            GLShader vertexShader = new GLShader(OpenTK.Graphics.OpenGL4.ShaderType.VertexShader);
            GLShader fragmentShader = new GLShader(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader);
            vertexShader.SetSource(vertexShaderCode);
            fragmentShader.SetSource(fragmentShaderCode);
            GLProgram.AddShader(vertexShader);
            GLProgram.AddShader(fragmentShader);
            GLProgram.LinkProgram();
            vertexShader.Dispose();
            fragmentShader.Dispose();
        }

        public void Dispose()
        {
            GLProgram.Dispose();
        }

        public void Use()
        {
            GLProgram.Bind();
        }
    }
}
