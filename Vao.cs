using Silk.NET.OpenGL;
using SimpleOpenGL.Structs;

namespace SimpleOpenGL {
    public class Vao : IDisposable {
        #region Variables

            private readonly uint _vao;
            private readonly uint _vbo;
            private readonly uint _ebo;

            private readonly Window _window;

            private nuint _vertexDataSize;
            private nuint _indexDataSize;
            private uint _vertexCount;
            private uint _indexCount;

            private bool _disposed;

        #endregion

        #region Properties

            public Window Window => _window;
            public uint VertexCount => _vertexCount;
            public uint IndexCount => _indexCount;

            public bool Disposed => _disposed;

        #endregion

        #region Constructors

            public Vao(Window window, float[] vertices, uint[]? indices = null, VertexAttribute[] attributes = null!) {
                _window = window;
                var gl = window.GlContext;

                _vao = gl.GenVertexArray();
                gl.BindVertexArray(_vao);

                _vbo = gl.GenBuffer();
                gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

                _vertexDataSize = (nuint)(vertices.Length * sizeof(float));
                _vertexCount = (uint)(vertices.Length / 3f);

                gl.BufferData<float>(BufferTargetARB.ArrayBuffer, _vertexDataSize, vertices, BufferUsageARB.StaticDraw);

                ConfigureVertexAttributes(attributes);

                if (indices != null) {
                    _ebo = gl.GenBuffer();
                    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

                    _indexDataSize = (nuint)(indices.Length * sizeof(uint));
                    _indexCount = (uint)indices.Length;

                    gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, _indexDataSize, indices, BufferUsageARB.StaticDraw);
                }
                else {
                    _ebo = 0;
                }

                gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                gl.BindVertexArray(0);
            }

        #endregion

        #region Public Methods

            public void UpdateVertices(float[] vertices) {

                if(_disposed) throw new ObjectDisposedException(nameof(Vao));

                var gl = _window.GlContext;
                
                gl.BindVertexArray(_vao);
                gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

                var newSize = (nuint)(vertices.Length * sizeof(float));
                _vertexCount = (uint)(vertices.Length / 3f);

                if(newSize > _vertexDataSize)
                    gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);
                else
                    gl.BufferSubData<float>(BufferTargetARB.ArrayBuffer, 0, newSize, vertices);
                
                _vertexDataSize = newSize;

                gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                gl.BindVertexArray(0);
            }

            public void UpdateIndices(uint[] indices) {
                if(_disposed) throw new ObjectDisposedException(nameof(Vao));

                var gl = _window.GlContext;

                gl.BindVertexArray(_vao);
                gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

                var newSize = (nuint)(indices.Length * sizeof(uint));
                _indexCount = (uint)indices.Length;

                if (newSize > _indexDataSize)
                    gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, newSize, indices, BufferUsageARB.StaticDraw);
                else
                    gl.BufferSubData<uint>(BufferTargetARB.ElementArrayBuffer, 0, newSize, indices);

                _indexDataSize = newSize;

                gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
                gl.BindVertexArray(0);
            }

            public void Bind() {
                if(_disposed) throw new ObjectDisposedException(nameof(Vao));
                _window.GlContext.BindVertexArray(_vao);
            }

            public void Unbind() {
                if(_disposed) throw new ObjectDisposedException(nameof(Vao));
                _window.GlContext.BindVertexArray(0);
            }

            public void Dispose() {
                if (_disposed) return;

                _window.GlContext.DeleteVertexArray(_vao);
                _window.GlContext.DeleteBuffer(_vbo);

                if(_ebo != 0) _window.GlContext.DeleteBuffer(_ebo);

                _disposed = true;
            }

        #endregion

        #region Private Methods

            private unsafe void ConfigureVertexAttributes(VertexAttribute[]? attributes) {
                if (attributes == null || attributes.Length == 0) {
                    _window.GlContext.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
                    _window.GlContext.EnableVertexAttribArray(0);
                    return;
                }

                long offset = 0;
                foreach (var attribute in attributes) {
                    _window.GlContext.VertexAttribPointer(attribute.Index, attribute.Size, attribute.Type, attribute.Normalized, attribute.Stride, (void*)offset);
                    _window.GlContext.EnableVertexAttribArray(attribute.Index);

                    offset += attribute.Size * sizeof(float);
                }
            }

        #endregion
    }
}
