using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KeypadDriverV2.Graphics
{
    public class Shader
    {
        private readonly int handle;

        private static readonly Regex includeRegex = new Regex("^\\s*#\\s*include\\s+[\"<](.*)[\">]");
        private static readonly Regex shaderInputRegex = new Regex("^\\s * (?> attribute |in)\\s+(?:(?:lowp|mediump|highp)\\s+)?\\w+\\s+(\\w+)");

        private readonly Dictionary<string, int> uniformLocations = new();
        private readonly List<ShaderInput> shaderInputs = new();
        private int lastInputIndex = -1;

        public Shader(string vertName, string fragName)
        {

            int vertexShader = CompilePart(vertName, ShaderType.VertexShader);
            int fragmentShader = CompilePart(fragName, ShaderType.FragmentShader);

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int res);
            if (res != 1)
            {
                string infoLog = GL.GetProgramInfoLog(handle);
                throw new Exception($"Error occurred while linking shader program: ({vertName} + {fragName}),\n" +
                                    $"ProgramInfoLog: {infoLog}");
            }

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            for (int i = 0; i < uniformCount; i++)
            {
                string key = GL.GetActiveUniform(handle, i, out _, out _);
                int location = GL.GetUniformLocation(handle, key);
                uniformLocations.Add(key, location);
            }
        }

        public void Bind() => GL.UseProgram(handle);

        private int CompilePart(string name, ShaderType type)
        {
            string ext = type switch
            {
                ShaderType.FragmentShader => ".frag",
                ShaderType.VertexShader => ".vert",
                _ => throw new NotImplementedException($"ShaderType{type}")
            };

            string[] lines = ReadFile(name + ext, type).ToArray();
            
            int[] lengths = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                lengths[i] = lines[i].Length;

            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, lines.Length, lines, lengths);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int res);
            if (res != 1)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred while compiling shader part: {name + ext},\n" +
                                    $"ShaderInfoLog: {infoLog}");
            }

            return shader;
        }

        private List<string> ReadFile(string fileName, ShaderType type)
        {
            byte[] data = GlobalResource.Shaders.Get(fileName);
            List<string> lines = new List<string>();

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (reader.Peek() != -1)
                    {
                        string line = reader.ReadLine();

                        Match includeMatch = includeRegex.Match(line);
                        if (includeMatch.Success)
                        {
                            string includeName = includeMatch.Groups[1].Value.Trim();
                            ReadFile(includeName, type).ForEach(str => lines.Add(str + '\n'));
                            continue;
                        }

                        if (type == ShaderType.VertexShader)
                        {
                            Match inputMatch = shaderInputRegex.Match(line);
                            if (inputMatch.Success)
                            {
                                shaderInputs.Add(new ShaderInput
                                {
                                    Location = ++lastInputIndex,
                                    Name = inputMatch.Groups[1].Value.Trim()
                                });
                                continue;
                            }
                        }

                        lines.Add(line + '\n');
                    }
                }
            }

            return lines;
        }

        private int GetAttribLocation(string attribName) => GL.GetAttribLocation(handle, attribName);

        public static implicit operator int(Shader shader) => shader.handle;

        // Setting a uniform:
        //     1. Bind the program you want to set the uniform on
        //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //     3. Use the appropriate GL.Uniform* function to set the uniform.

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(uniformLocations[name], true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(handle);
            GL.Uniform3(uniformLocations[name], data);
        }
    }
}
