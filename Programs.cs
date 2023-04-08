using System;
using System.Collections;
using OpenTK.Graphics.OpenGL;

namespace DemoCube
{
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
}
