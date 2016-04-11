using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace NinjaScan_GUI
{
    class Cube3D : GameWindow
    {

        AHRS.MadgwickAHRS ahrs;

        private void DrawCube()
        {
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color4.Gray);
            GL.Vertex3(0.5, -0.5, -0.5);      // P1 is red
            GL.Vertex3(0.5, 0.5, -0.5);      // P2 is green
            GL.Vertex3(-0.5, 0.5, -0.5);      // P3 is blue
            GL.Vertex3(-0.5, -0.5, -0.5);      // P4 is purple
            GL.End();

            // White side - BACK
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color4.Yellow);
            GL.Vertex3(0.5, -0.5, 0.5);
            GL.Vertex3(0.5, 0.5, 0.5);
            GL.Vertex3(-0.5, 0.5, 0.5);
            GL.Vertex3(-0.5, -0.5, 0.5);
            GL.End();

            // Purple side - RIGHT
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color4.White);
            GL.Vertex3(0.5, -0.5, -0.5);
            GL.Vertex3(0.5, 0.5, -0.5);
            GL.Vertex3(0.5, 0.5, 0.5);
            GL.Vertex3(0.5, -0.5, 0.5);
            GL.End();

            // Green side - LEFT
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color4.Red);
            GL.Vertex3(-0.5, -0.5, 0.5);
            GL.Vertex3(-0.5, 0.5, 0.5);
            GL.Vertex3(-0.5, 0.5, -0.5);
            GL.Vertex3(-0.5, -0.5, -0.5);
            GL.End();

            // Blue side - TOP
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color4.Blue);
            GL.Vertex3(0.5, 0.5, 0.5);
            GL.Vertex3(0.5, 0.5, -0.5);
            GL.Vertex3(-0.5, 0.5, -0.5);
            GL.Vertex3(-0.5, 0.5, 0.5);
            GL.End();

            // Red side - BOTTOM
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(Color4.Green);
            GL.Vertex3(0.5, -0.5, -0.5);
            GL.Vertex3(0.5, -0.5, 0.5);
            GL.Vertex3(-0.5, -0.5, 0.5);
            GL.Vertex3(-0.5, -0.5, -0.5);
            GL.End();
        }

        public Cube3D(Form1 owner)
            : base(600, 400, GraphicsMode.Default, "1-2:Camera")
        {
            ahrs = owner.AHRS;

            VSync = VSyncMode.On;

            Keyboard.KeyUp += (sender, e) =>
            {
                //Escapeキーで終了
                if (e.Key == Key.Escape)
                {
                    this.Exit();
                }
            };

            RenderFrame += (sender, e) =>
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);
                Vector3 eyepoint = new Vector3(0, 0, -3);
                Matrix4 modelview = Matrix4.LookAt(eyepoint, Vector3.UnitZ, Vector3.UnitY);
                GL.LoadMatrix(ref modelview);

                Quaternion q = new Quaternion(ahrs.Quaternion[1],
                    ahrs.Quaternion[2],
                    ahrs.Quaternion[3],
                    ahrs.Quaternion[0]);
                float angle;
                OpenTK.Vector3 vec3 = new Vector3();
                q.ToAxisAngle(out vec3, out angle);
                GL.Rotate((float)(angle * 180 / Math.PI), vec3);

                //float deg = 0;
                //GL.Rotate(deg, Vector3d.UnitY);
                //deg += 1;

                DrawCube();

                SwapBuffers();
            };
        }

        // Windowの起動時に実行
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)Width / (float)Height, 1.0f, 64.0f);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }
    }
}
