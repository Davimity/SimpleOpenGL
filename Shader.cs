using System.Numerics;
using Silk.NET.OpenGL;

namespace SimpleOpenGL {
    public unsafe class Shader : IDisposable {
        #region Variables

            private readonly uint _handle;
            private readonly GL _gl;

        #endregion

        #region Constructors

            /// <summary> Creates a new shader program from the given vertex and fragment shaders. </summary>
            /// <param name="window"> The window that will use the shader. </param>
            /// <param name="vertex"> The vertex shader path or source code. </param>
            /// <param name="fragment"> The fragment shader path or source code. </param>
            /// <param name="isPath"> Whether the given strings are paths or source code. </param>
            public Shader(Window window, string vertex, string fragment, bool isPath = true) {
                _gl = window.GlContext;

                string vertexSource;
                string fragmentSource;

                if (isPath) {
                    vertexSource = File.ReadAllText(vertex);
                    fragmentSource = File.ReadAllText(fragment);
                }
                else {
                    vertexSource = vertex;
                    fragmentSource = fragment;
                }

                var vertexShader = CompileShader(vertexSource, GLEnum.VertexShader);
                var fragmentShader = CompileShader(fragmentSource, GLEnum.FragmentShader);

                _handle = _gl.CreateProgram();

                _gl.AttachShader(_handle, vertexShader);
                _gl.AttachShader(_handle, fragmentShader);
                _gl.LinkProgram(_handle);

                _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);

                if (status == 0) {
                    var infoLog = _gl.GetProgramInfoLog(_handle);
                    throw new Exception($"Error linking shader program: {infoLog}");
                }

                _gl.DeleteShader(vertexShader);
                _gl.DeleteShader(fragmentShader);
            }

        #endregion

        #region Public Methods

            public void Use() {
                _gl.UseProgram(_handle);
            }

            public void Unuse() {
                _gl.UseProgram(0);
            }

            public void SetUniform(string name, int value) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.Uniform1(location, value);
            }

            public void SetUniform(string name, float value) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.Uniform1(location, value);
            }

            public void SetUniform(string name, bool value) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.Uniform1(location, value ? 1 : 0);
            }

            public void SetUniform(string name, Vector2 value) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.Uniform2(location, value);
            }

            public void SetUniform(string name, Vector3 value) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.Uniform3(location, value);
            }

            public void SetUniform(string name, Vector4 value) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.Uniform4(location, value);
            }

            public void SetUniform(string name, Matrix4x4 value, bool traspose = false) {
                var location = _gl.GetUniformLocation(_handle, name);
                _gl.UniformMatrix4(location, 1, traspose, (float*)&value);
            }

            public bool HasUniform(string name) {
                return _gl.GetUniformLocation(_handle, name) != -1;
            }

            public void Dispose() {
                _gl.DeleteProgram(_handle);
            }

        #endregion

        #region Private Methods

            private uint CompileShader(string source, GLEnum type) {
                var shader = _gl.CreateShader(type);
                _gl.ShaderSource(shader, source);
                _gl.CompileShader(shader);

                var infoLog = _gl.GetShaderInfoLog(shader);
                if (!string.IsNullOrWhiteSpace(infoLog)) {
                    throw new Exception($"Error compiling shader of type {type}: {infoLog}");
                }

                return shader;
            }

        #endregion
    }
}
