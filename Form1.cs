using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DemoCube
{
    public partial class Form1 : Form
    {
        private Cube cube;
        private Programs program;
        private Camera camera;
        private System.Windows.Forms.Timer timer;
        private Point start;
        private Point end;
        private readonly string[] btnShadeMode = { "Закрашенный", "Контур", "Оба" };
        private readonly string[] btnLightMode = { "Вкл.Освещение", "Выкл.Освещение" };
        private int btnShadeCurrent;
        private int btnLightCurrent;
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

        private void Render(string mode)
        {
            program.UseProgram(mode);
            SetMatrixUniform(ref cube.Model, mode, "model");
            SetMatrixUniform(ref camera.View, mode, "view");
            SetMatrixUniform(ref camera.Projection, mode, "proj");
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
                camera.Update(ref start, ref end);
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
            glCanvas.MouseWheel += OnMouseWheel;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000 / 60;
            timer.Tick += OnTimer;
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
            camera = new Camera(glCanvas, new Vector3(0.0f, 0.0f, 20.0f), Vector3.Zero);
            isNeedRedraw = true;
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

        private void OnUp(object sender, MouseEventArgs e) => camera.EnableRotate = false;

        private void OnMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!camera.EnableRotate)
                {
                    start = e.Location;
                    end = start;
                    camera.EnableRotate = true;
                }
                else
                {
                    start = end;
                    end = e.Location;
                    isNeedRedraw = true;
                }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            camera.UpdatePosition(e.Delta);
            isNeedRedraw = true;
        }

        private void OnResize(object sender, EventArgs e) => isNeedRedraw = true;
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
