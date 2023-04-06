using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCube
{
    internal static class Shader
    {
        public static void VertexWireShader(ref string shader)
        {
            shader = "#version 330 core\n" +
                     "layout(location = 0) in vec3 pos;\n" +
                     "uniform mat4 model;\n" +
                     "uniform mat4 view;\n" +
                     "uniform mat4 proj;\n" +
                     "void main(){\n" +
                     "gl_Position = proj * view * model * vec4(pos, 1.0);}";
        }

        public static void FragmentWireShader(ref string shader)
        {
            shader = "#version 330 core\n" +
                     "out vec4 fragColor;\n" +
                     "uniform vec3 color;\n" +
                     "uniform float alpha;\n"+
                     "void main(){\n" +
                     "fragColor = vec4(color,alpha);}";
        }

        public static void VertexShadedShader(ref string shader)
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

        public static void FragmentShadedShader(ref string shader)
        {
            shader = "#version 330 core\n" +
                      "in vec3 oNorm;\n" +
                      "in vec3 fragPos;\n" +
                      "out vec4 fragColor;\n" +
                      "uniform vec3 color;\n" +
                      "uniform uint lightOn;\n" +
                      "uniform float alpha;\n" +
                      "const vec3 lightPos = vec3(0.0,0.0,10.0);\n" +
                      "const vec3 lightColor = vec3(1.0,1.0,1.0);\n" +
                      "const float ambK = 0.4;\n" +
                      "void main(){\n" +
                      "vec3 outColor = color;\n"+
                      "if(lightOn == 1){\n"+
                      "vec3 norm = normalize(oNorm);\n" +
                      "vec3 dirLight = normalize(lightPos - fragPos);\n" +
                      "float diff = max(dot(norm, dirLight), 0.0);\n" +
                      "vec3 diffuse = diff * lightColor;\n" +
                      "vec3 ambient =  ambK * lightColor;\n" +
                      "outColor = (diffuse + ambient) * color;}\n" +
                      "fragColor = vec4(outColor,alpha);}";
        }
    }
}
