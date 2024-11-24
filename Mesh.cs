using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleOpenGL {
    public unsafe class Mesh : IDisposable {

        #region Variables

            private readonly Vao _vao;
            private readonly Window _window;
            private readonly Material _material;

            private bool _disposed;

            public uint VertexCount => _vao.VertexCount;
            public uint IndexCount => _vao.IndexCount;

        #endregion

        #region Constructor

            public Mesh(Window window, Vao vao, Material material) {
                _window = window;
                _vao = vao;
                _material = material;
            }

        #endregion

        #region Public Methods

            public void Draw() {
                if(_disposed) throw new ObjectDisposedException(nameof(Mesh));

                _material.Use();
                _vao.Bind();
                _window.GlContext.DrawElements(PrimitiveType.Triangles, _vao.IndexCount, DrawElementsType.UnsignedInt, null);
                _vao.Unbind();
            }
            public void Dispose() {
                if (_disposed) return;

                _vao.Dispose();
                _material.Dispose();

                _disposed = true;
            }

        #endregion
    }
}
