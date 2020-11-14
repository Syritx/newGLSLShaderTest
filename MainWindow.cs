using System;
using System.IO;
using System.Text;

using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

using OpenTK.Graphics.OpenGL;

namespace shaderTest {
    class MainWindow : GameWindow
    {
        int vertexBufferObject,
            vertexArrayObject;

        float[] vertices = {
            -0.5f, -0.5f, 0.0f, 
             0.5f, -0.5f, 0.0f, 
             0.0f,  0.5f, 0.0f  
        };
        Shader shader;

        string vertexShaderSource;
        string fragmentShaderSource; 

        NativeWindowSettings nativeWindowSettings;
        public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) :
            base(gameWindowSettings, nativeWindowSettings) {

            this.nativeWindowSettings = nativeWindowSettings;
            vertexShaderSource = GetShaderSource("vertexShader.glsl");
            fragmentShaderSource = GetShaderSource("fragmentShader.glsl");

            Run();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            vertexArrayObject = GL.GenVertexArray();

            GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,3 * sizeof(float),0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            try { 
                shader.UseShader();
            } catch(Exception e1) {}
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length/3);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            int width = nativeWindowSettings.Size.X;
            int height = nativeWindowSettings.Size.Y;

            Console.WriteLine(width + " " + height);

            GL.Viewport(0, 0, width, height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            Matrix4 perspectiveMatrix =
                Matrix4.CreatePerspectiveFieldOfView(1, width / height, 1.0f, 2000.0f);

            GL.LoadMatrix(ref perspectiveMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.End();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            shader = new Shader(vertexShaderSource, fragmentShaderSource);
            vertexBufferObject = GL.GenBuffer();

            GL.ClearColor(0, 0, 0, 0);
            GL.Enable(EnableCap.DepthTest);
        }


        string GetShaderSource(string file) {
            string source = "";

            using (StreamReader reader = new StreamReader(file, Encoding.UTF8)) {
                source = reader.ReadToEnd();
            }
            return source;
        }
    }
}