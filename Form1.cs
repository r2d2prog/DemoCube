using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        Programs program;
        Matrix4 projection;
        Matrix4 view;
        readonly string[] btnShadeMode = {"Закрашенный", "Контур", "Оба"};
        readonly string[] btnLightMode = {"Вкл.Освещение", "Выкл.Освещение"};
        int btnShadeCurrent;
        int btnLightCurrent;

        public Form1()
        {
            InitializeComponent();
        }

        private void SetMatrixUniform(ref Matrix4 matrix, string programName, string name)
        {
            var location = GL.GetUniformLocation(program[programName], name);
            GL.UniformMatrix4(location, false,ref matrix);
        }

        private void SetUniformLighting(uint isEnable, string programName)
        {
            int location = GL.GetUniformLocation(program[programName], "lightOn");
            GL.Uniform1(location, isEnable);
        }

        private void SetUniformColor(Color color, string programName)
        {
            int location = GL.GetUniformLocation(program[programName], "color");
            GL.Uniform3(location, (float)color.R / 255, (float)color.G / 255, (float)color.B / 255);
        }

        private void SetUniformAlpha(float value, string programName)
        {
            int location = GL.GetUniformLocation(program[programName], "alpha");
            GL.Uniform1(location, value);
        }

        private void Render(string mode)
        {
            program.UseProgram(mode);
            SetMatrixUniform(ref cube.Model, mode, "model");
            SetMatrixUniform(ref view, mode, "view");
            SetMatrixUniform(ref projection, mode, "proj");
            SetUniformAlpha(float.Parse(alphaValue.Text), mode);
            if (mode == "Shaded")
            {
                SetUniformLighting((uint)btnLightCurrent, mode);
                SetUniformColor(cube.Color, mode);
            }
            else
                SetUniformColor(Color.Black, mode);
            cube.Render(mode);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var shadeMode = btnShadeMode[btnShadeCurrent];
            view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 20.0f),Vector3.Zero,Vector3.UnitY);
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, (float)glCanvas.Size.Width / glCanvas.Size.Height, 0.1f, 32.0f);
            GL.Viewport(0, 0, glCanvas.Width, glCanvas.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (trackAlpha.Value != 100)
                GL.DepthFunc(DepthFunction.Always);
            else
                GL.DepthFunc(DepthFunction.Lequal);
            if (shadeMode == "Закрашенный" || shadeMode == "Оба")
                Render("Shaded");
            if (shadeMode == "Контур" || shadeMode == "Оба")
                Render("Wireframe");
            glCanvas.SwapBuffers();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(Color.Cyan);
            cube = new Cube();
            cube.Color = colorChoice.Color;
            program = new Programs();
            var status = program.CreateProgram("Shaded", Shader.VertexShadedShader, Shader.FragmentShadedShader);
            if(!status || !program.CreateProgram("Wireframe", Shader.VertexWireShader, Shader.FragmentWireShader))
                Close();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            cube.FreeBuffers();
            program.FreePrograms();
        }

        private void OnShadeClick(object sender, EventArgs e)
        {
            btnShadeCurrent = (btnShadeCurrent + 1) % 3;
            var button = sender as Button;
            button.Text = btnShadeMode[btnShadeCurrent];
            glCanvas.Invalidate();
        }

        private void OnLightClick(object sender, EventArgs e)
        {
            btnLightCurrent = (btnLightCurrent + 1) % 2;
            var button = sender as Button;
            button.Text = btnLightMode[btnLightCurrent];
            glCanvas.Invalidate();
        }

        private void OnChange(object sender, EventArgs e)
        {
            alphaValue.Text = ((sender as TrackBar).Value * 0.01f).ToString();
            glCanvas.Invalidate();
        }

        private void ColorChoice(object sender, EventArgs e)
        {
            colorChoice.ShowDialog();
            sideColor.BackColor = cube.Color = colorChoice.Color;
            glCanvas.Invalidate();
        }
    }

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
            for(var i = 0; i < 4; ++i)
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

    public class Programs
    {
        Hashtable programs;
        public delegate void ShaderFun(ref string data);
        public Programs()
        {
            programs = new Hashtable(); 
        }

        public int this [string key]
        {
            get => ((Tuple<int,int,int>)programs[key]).Item1;
        }

        public void UseProgram(string key) => GL.UseProgram(((Tuple<int,int,int>)programs[key]).Item1);

        public void FreePrograms()
        {
            foreach(DictionaryEntry entry in programs)
            {
                var program = entry.Value as Tuple<int, int, int>;
                GL.DeleteShader(program.Item2);
                GL.DeleteShader(program.Item3);
                GL.DetachShader(program.Item1, program.Item2);
                GL.DetachShader(program.Item1, program.Item3);
                GL.DeleteProgram(program.Item1);
            }
        }

        public bool CreateProgram(string key, ShaderFun vertFun, ShaderFun fragFun)
        {
            var program = GL.CreateProgram();
            var vertex = GL.CreateShader(ShaderType.VertexShader);
            var fragment = GL.CreateShader(ShaderType.FragmentShader);
            var shader = "";
            var status = CreateShader(vertex, vertFun, ref shader);
            if (status)
            {
                status = CreateShader(fragment, fragFun, ref shader);
                if (status)
                {
                    GL.AttachShader(program, vertex);
                    GL.AttachShader(program, fragment);
                    GL.LinkProgram(program);
                    programs[key] = Tuple.Create(program, vertex, fragment);
                }
            }
            return status;
        }

        private bool CreateShader(int shader, ShaderFun shaderFun,  ref string data)
        {
            var status = true;
            shaderFun(ref data);
            GL.ShaderSource(shader, data);
            GL.CompileShader(shader);
            if (GL.GetError() != ErrorCode.NoError)
            {
                GL.GetShaderInfoLog(shader, out data);
                status = false;
            }
            return status;
        }
    }

    public class ErrorLog
    {
        public ErrorLog(ref string message)
        {
            using (StreamWriter writer = new StreamWriter("errorLog.txt"))
            {
                writer.WriteLine(message);
            }
        }
    }
}
