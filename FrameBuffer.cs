using Silk.NET.OpenGL;

namespace SimpleOpenGL {
    public unsafe class FrameBuffer {
        
        #region Variables

            private readonly Window _window;
            private readonly uint _fbo;
            private readonly uint _texture;
            private readonly uint _rbo;

            private bool _disposed;

        #endregion

        #region Properties

            public uint Fbo => _fbo;
            public uint Texture => _texture;
            public uint Rbo => _rbo;

        #endregion

        #region Constructors

            public FrameBuffer(Window window, int width, int height) {
                _window = window;
                var gl = window.GlContext;

                _fbo = gl.GenFramebuffer();
                gl.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

                _texture = gl.GenTexture();
                gl.BindTexture(TextureTarget.Texture2D, _texture);
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture, 0);

                _rbo = gl.GenRenderbuffer();
                gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _rbo);
                gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, (uint)width, (uint)height);
                gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _rbo);

                if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete) {
                    throw new Exception("Framebuffer is not complete!");
                }

                gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

        #endregion

        #region Public Methods

            public void Bind() {
                if(_disposed) throw new ObjectDisposedException(nameof(FrameBuffer));

                _window.GlContext.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
            }

            public void Unbind() {
                if (_disposed) throw new ObjectDisposedException(nameof(FrameBuffer));

                _window.GlContext.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }

            public void BindTexture(TextureUnit textureUnit = TextureUnit.Texture0) {
                if (_disposed) throw new ObjectDisposedException(nameof(FrameBuffer));

                _window.GlContext.ActiveTexture(textureUnit);
                _window.GlContext.BindTexture(TextureTarget.Texture2D, _texture);
            }

            public void Dispose() {
                if (_disposed) return; 

                _window.GlContext.DeleteFramebuffer(_fbo);
                _window.GlContext.DeleteTexture(_texture);
                _window.GlContext.DeleteRenderbuffer(_rbo);

                _disposed = true;
            }

        #endregion
    }
}
