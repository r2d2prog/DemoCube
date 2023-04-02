using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DemoCube
{
    public partial class Form1 : Form
    {
        Cube cube;
        public Form1()
        {
            InitializeComponent();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            GL.Viewport(0, 0, glCanvas.Width, glCanvas.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            glCanvas.SwapBuffers();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Cyan);
            cube = new Cube();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            cube.FreeBuffers();
        }
    }

    public class Cube
    {
        const float side = 10.0f;
        const float initAngle = 7 * (float)Math.PI / 4;
        const float offsetAngle = (float)Math.PI / 2;
        const int totalPoints = 6 * 4;
        int vao;
        int vbo;
        int ebo;
        public Cube(Vector3 origin = new Vector3())
        {
            var points = new List<Vector3>(totalPoints * 2);
            var indices = new List<uint>(totalPoints + 12);
            var up = new List<Vector3>(4);
            var down = new List<Vector3>(4);
            var faces = new List<Vector3>(16);
            CreateFaces(ref origin, up, down, faces);
            FillFaces(up, points, indices);
            FillFaces(down, points, indices, 0, 4);
            FillFaces(faces, points, indices, 0, 8);
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, points.Count * sizeof(float) * 3, points.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint),indices.ToArray(), BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public void FreeBuffers()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            vao = vbo = 0;
        }

        private void CreateFace(List<Vector3> points, ref Vector3 origin, float offsetAngle, float radius)
        {
            var vector = new Vector3(0.0f, origin.Y, 0.0f);
            float angle = initAngle;
            for (var i = 0; i < 4; ++i)
            {
                vector.X = origin.X + radius * (float)Math.Cos(angle);
                vector.Z = origin.Z + radius * -(float)Math.Sin(angle);
                points.Add(vector);
                angle += offsetAngle;
            }
        }

        private void CreateFaces(ref Vector3 origin, List<Vector3> up, List<Vector3> down, List<Vector3> faces)
        {
            origin.Y = origin.Y + side / 2;
            var radius = (float)Math.Sqrt(2) * side / 2;
            CreateFace(up, ref origin, offsetAngle, radius);
            origin.Y = -origin.Y;
            CreateFace(down, ref origin, -offsetAngle, radius);
            for(int i = 0, j = 3; i < 4; ++i, --j)
            {
                faces.Add(up[i]);
                faces.Add(i == 0 ? down[0] : down[j + 1]);
                faces.Add(down[j]);
                faces.Add(i == 3 ? up[0] : up[i + 1]);
            }
        }

        private void FillFaces(List<Vector3> src, List<Vector3> points, List<uint> indices, int srcRead = 0, int dstWrite = 0)
        {
            var totalSides = (src.Count - srcRead) / 4;
            for (var i = 0; i < totalSides; ++i, srcRead += 4, dstWrite += 4)
            {
                var normal = GetNormal(src[srcRead + 1] - src[srcRead], src[srcRead + 2] - src[srcRead]);
                for (var j = 0; j < 4; ++j)
                {
                    points.Add(src[srcRead + j]);
                    points.Add(normal);
                    indices.Add((uint)(dstWrite + j));
                    if (j == 2)
                        indices.Add((uint)(dstWrite + j));
                }
                indices.Add((uint)dstWrite);
            }
        }

        private Vector3 GetNormal(Vector3 left, Vector3 right)
        {
            var normal = Vector3.Cross(left, right);
            normal.Normalize();
            return normal;
        }
    }

}
