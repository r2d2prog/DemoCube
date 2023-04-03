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
        Programm program;
        Matrix4 projection;
        Matrix4 view;
        public Form1()
        {
            InitializeComponent();
        }

        private void SetMatrixUniform(ref Matrix4 matrix,string name)
        {
            var location = GL.GetUniformLocation(program.Id, name);
            GL.UniformMatrix4(location, false,ref matrix);
        }

        private void SetUniformColor(ref Color color)
        {
            int location = GL.GetUniformLocation(program.Id, "color");
            GL.Uniform3(location, (float)color.R / 255, (float)color.G / 255, (float)color.B / 255);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 20.0f),Vector3.Zero,Vector3.UnitY);
            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, (float)glCanvas.Size.Width / glCanvas.Size.Height, 0.1f, 32.0f);
            GL.Viewport(0, 0, glCanvas.Width, glCanvas.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            program.UseProgram();
            SetMatrixUniform(ref cube.Model, "model");
            SetMatrixUniform(ref view, "view");
            SetMatrixUniform(ref projection, "proj");
            SetUniformColor(ref cube.Color);
            cube.Render();
            glCanvas.SwapBuffers();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.Cyan);
            cube = new Cube();
            program = new Programm();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            cube.FreeBuffers();
            program.FreeProgramm();
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
            model = Matrix4.Identity;
            color = Color.Orange;
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

        public ref Matrix4 Model { get { return ref model; } }
        public ref Color Color { get { return ref color; } }

        public void Render()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, 36, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
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

    public class Programm
    {
        int programm;
        int vs;
        int fs;
        public Programm()
        {
            programm = GL.CreateProgram();
            CreateShader();
            CreateShader(false);
            GL.AttachShader(programm, vs);
            GL.AttachShader(programm, fs);
            GL.LinkProgram(programm);    
        }

        public int Id { get { return programm; } }

        public void UseProgram() => GL.UseProgram(programm);

        public void FreeProgramm()
        {
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
            GL.DetachShader(programm, vs);
            GL.DetachShader(programm, fs);
            GL.DeleteProgram(programm);
        }

        private void SetVertexShader(ref string shader)
        {
            shader = "#version 330 core\n" +
                     "layout(location = 0) in vec3 pos;\n" +
                     "layout(location = 1) in vec3 norm;\n" +
                     "out vec3 oNorm;\n" +
                     "out vec3 fragPos;\n" +
                     "uniform mat4 model;\n" +
                     "uniform mat4 view;\n" +
                     "uniform mat4 proj;\n" +
                     "void main(){\n" +
                     "oNorm = norm;\n" +
                     "fragPos = vec3(model * vec4(pos, 1.0));\n" +
                     "gl_Position = proj * view * vec4(fragPos, 1.0);}";
        }

        private void SetFragmentShader(ref string shader)
        {
            shader = "#version 330 core\n"+
                     "in vec3 oNorm;\n"+
                     "in vec3 fragPos;\n"+
                     "out vec4 fragColor;\n" +
                     "uniform vec3 color;\n" +
                     "const vec3 lightPos = vec3(0.0,0.0,10.0);\n" +
                     "const vec3 lightColor = vec3(1.0,1.0,1.0);\n" +
                     "const float ambK = 0.1;\n" +
                     "void main(){\n" +
                     "vec3 norm = normalize(oNorm);\n" +
                     "vec3 dirLight = normalize(lightPos - fragPos);\n" +
                     "float diff = max(dot(norm, dirLight), 0.0);\n" +
                     "vec3 diffuse = diff * lightColor;\n" +
                     "vec3 ambient =  ambK * lightColor;\n"+
                     "vec3 ou = (diffuse + ambient) * color;\n"+
                     "fragColor = vec4(ou,1.0);}";
        }

        private void CreateShader(bool isVertexShader = true)
        {
            int shader = GL.CreateShader(isVertexShader ? ShaderType.VertexShader : ShaderType.FragmentShader);
            string data = "";
            if (isVertexShader)
            {
                vs = shader;
                SetVertexShader(ref data);
            }
            else
            {
                fs = shader;
                SetFragmentShader(ref data);
            }
            GL.ShaderSource(shader, data);
            GL.CompileShader(shader);
        }
    }
}
