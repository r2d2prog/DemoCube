using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace DemoCube
{
    public class Cube
    {
        const float side = 10.0f;
        const float initAngle = 0;
        const float offsetAngle = (float)Math.PI / 2;
        const int totalPoints = 6 * 4;
        Matrix4 model;
        Color color;
        int shadedVao;
        int shadedVbo;
        int shadedEbo;
        int wireVao;
        int wireVbo;

        public ref Matrix4 Model { get { return ref model; } }
        public ref Color Color { get { return ref color; } }

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
            model = Matrix4.Identity;
            BindShadedBuffer(points, indices);
            BindWireframeBuffer(up, down);
        }

        public void Render(string mode)
        {
            if (mode == "Shaded")
            {
                GL.BindVertexArray(shadedVao);
                GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
            }
            else if (mode == "Wireframe")
            {
                GL.BindVertexArray(wireVao);
                GL.DrawArrays(PrimitiveType.Lines, 0, 24);
            }
            GL.BindVertexArray(0);
        }

        public void FreeBuffers()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(shadedVao);
            GL.DeleteVertexArray(wireVao);
            GL.DeleteBuffer(wireVbo);
            GL.DeleteBuffer(shadedVbo);
            GL.DeleteBuffer(shadedEbo);
        }

        private List<Vector3> CreateLines(List<Vector3> src)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 0; i < 4; ++i)
            {
                points.Add(src[i]);
                if (i != 0)
                    points.Add(src[i]);
            }
            points.Add(src[0]);
            return points;
        }

        private List<Vector3> CreateVerticalLines(List<Vector3> up, List<Vector3> down)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 0; i < 4; ++i)
            {
                points.Add(up[i]);
                if ((i & 1) == 0)
                    points.Add(down[i]);
                else
                {
                    var index = i == 1 ? 3 : 1;
                    points.Add(down[index]);
                }
            }
            return points;
        }

        private void BindWireframeBuffer(List<Vector3> up, List<Vector3> down)
        {
            var points = CreateVerticalLines(up, down);
            points.AddRange(CreateLines(up));
            points.AddRange(CreateLines(down));
            wireVao = GL.GenVertexArray();
            wireVbo = GL.GenBuffer();
            GL.BindVertexArray(wireVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, wireVbo);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, points.Count * sizeof(float) * 3, points.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
        }

        private void BindShadedBuffer(List<Vector3> points, List<uint> indices)
        {
            shadedVao = GL.GenVertexArray();
            shadedVbo = GL.GenBuffer();
            shadedEbo = GL.GenBuffer();
            GL.BindVertexArray(shadedVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, shadedVbo);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, points.Count * sizeof(float) * 3, points.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, shadedEbo);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
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
            for (int i = 0, j = 3; i < 4; ++i, --j)
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
