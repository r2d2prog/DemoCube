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
        private Cube cube;
        private Programs program;
        private System.Windows.Forms.Timer timer;
        private Matrix4 projection;
        private Matrix4 view;
        private Point start;
        private Point end;
        private Vector3 position = new Vector3(0.0f, 0.0f, 20.0f);
        private Vector3 target = Vector3.Zero;
        private Vector2 yawPitch = new Vector2();
        private readonly string[] btnShadeMode = { "Закрашенный", "Контур", "Оба" };
        private readonly string[] btnLightMode = { "Вкл.Освещение", "Выкл.Освещение" };
        private float length;
        private int btnShadeCurrent;
        private int btnLightCurrent;
        private bool isStartRotate;
        private bool isNeedRedraw;

        public Form1()
        {
            InitializeComponent();
        }

        private void SetMatrixUniform(ref Matrix4 matrix, string programName, string name)
        {
            var location = GL.GetUniformLocation(program[programName], name);
            GL.UniformMatrix4(location, false, ref matrix);
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

        private float CalculateDelta(int coordStart, int coordEnd, int dimension) => ((float)coordEnd - coordStart) / (dimension / 2) * 2 * (float)Math.PI;

        private void UpdateAngle(ref float coords)
        {
            if (coords > 2 * Math.PI)
                coords = coords - 2 * (float)Math.PI;
            else if (coords < 0)
                coords = 2 * (float)Math.PI + coords;
        }

        private void UpdateCamera()
        {
            yawPitch.X += CalculateDelta(end.Y, start.Y, glCanvas.Height);
            yawPitch.Y += CalculateDelta(end.X, start.X, glCanvas.Width);
            UpdateAngle(ref yawPitch.X);
            UpdateAngle(ref yawPitch.Y);
            Vector3 front = new Vector3();
            front.X = (float)(Math.Cos(yawPitch.Y) * Math.Cos(yawPitch.X));
            front.Y = (float)(Math.Sin(yawPitch.X));
            front.Z = (float)(Math.Cos(yawPitch.X) * -Math.Sin(yawPitch.Y));
            front.Normalize();
            position = -front * length;
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
            if (isNeedRedraw)
            {
                var shadeMode = btnShadeMode[btnShadeCurrent];
                if (isStartRotate)
                    UpdateCamera();
                view = Matrix4.LookAt(position, target, Vector3.UnitY);
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
                isNeedRedraw = false;
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            var vector = position + target;
            length = vector.Length;
            vector.Normalize();
            yawPitch.X = (float)Math.Acos(Vector3.Dot(vector, Vector3.UnitZ));
            yawPitch.Y = (float)Math.Acos(Vector3.Dot(vector, Vector3.UnitX));
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000 / 60;
            timer.Tick += OnTimer;
            isNeedRedraw = true;
            timer.Start();
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(Color.Cyan);
            cube = new Cube();
            cube.Color = colorChoice.Color;
            program = new Programs();
            var status = program.CreateProgram("Shaded", Shader.VertexShadedShader, Shader.FragmentShadedShader);
            if (!status || !program.CreateProgram("Wireframe", Shader.VertexWireShader, Shader.FragmentWireShader))
                Close();
        }

        private void OnTimer(object sender, EventArgs e) => glCanvas.Invalidate();

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            isNeedRedraw = false;
            timer.Stop();
            cube.FreeBuffers();
            program.FreePrograms();
        }

        private void OnShadeClick(object sender, EventArgs e)
        {
            btnShadeCurrent = (btnShadeCurrent + 1) % 3;
            var button = sender as Button;
            button.Text = btnShadeMode[btnShadeCurrent];
            isNeedRedraw = true;
        }

        private void OnLightClick(object sender, EventArgs e)
        {
            btnLightCurrent = (btnLightCurrent + 1) % 2;
            var button = sender as Button;
            button.Text = btnLightMode[btnLightCurrent];
            isNeedRedraw = true;
        }

        private void OnChange(object sender, EventArgs e)
        {
            alphaValue.Text = ((sender as TrackBar).Value * 0.01f).ToString();
            isNeedRedraw = true;
        }

        private void ColorChoice(object sender, EventArgs e)
        {
            colorChoice.ShowDialog();
            sideColor.BackColor = cube.Color = colorChoice.Color;
            isNeedRedraw = true;
        }

        private void OnUp(object sender, MouseEventArgs e) => isStartRotate = false;

        private void OnMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!isStartRotate)
                {
                    start = e.Location;
                    end = start;
                    isStartRotate = true;
                }
                else
                {
                    start = end;
                    end = e.Location;
                    isNeedRedraw = true;
                }
            }
        }

        private void OnResize(object sender, EventArgs e) => isNeedRedraw = true;
    }

    public class Programs
    {
        Hashtable programs;
        public delegate void ShaderFun(ref string data);
        public Programs()
        {
            programs = new Hashtable();
        }

        public int this[string key]
        {
            get => ((Tuple<int, int, int>)programs[key]).Item1;
        }

        public void UseProgram(string key) => GL.UseProgram(((Tuple<int, int, int>)programs[key]).Item1);

        public void FreePrograms()
        {
            foreach (DictionaryEntry entry in programs)
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

        private bool CreateShader(int shader, ShaderFun shaderFun, ref string data)
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
