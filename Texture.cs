using Silk.NET.OpenGL;
using StbImageSharp;

namespace SimpleOpenGL {
    public unsafe class Texture : IDisposable {
        #region Variables

            private readonly uint _handle;
            private readonly GL _gl;

            public uint MipMapLevels { get; private set; }
            public uint Width { get; private set; }
            public uint Height { get; private set; }

            private bool _disposed;

        #endregion

        #region Constructors

            public Texture(Window window,
                       string path,
                       TextureWrapMode wrapModeS = TextureWrapMode.Repeat,
                       TextureWrapMode wrapModeT = TextureWrapMode.Repeat,
                       TextureMinFilter minFilter = TextureMinFilter.LinearMipmapLinear,
                       TextureMagFilter magFilter = TextureMagFilter.Linear,
                       InternalFormat internalFormat = InternalFormat.Rgba,
                       PixelFormat pixelFormat = PixelFormat.Rgba,
                       PixelType pixelType = PixelType.UnsignedByte,
                       bool generateMipmaps = true,
                       uint mipMapLevels = 0,
                       bool compressTexture = false,
                       InternalFormat? compressedFormat = null) {

                _gl = window.GlContext;
                _handle = _gl.GenTexture();
                Bind();

                StbImage.stbi_set_flip_vertically_on_load(1);
                using var imageStream = File.OpenRead(path);
                var image = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);

                Width = (uint)image.Width;
                Height = (uint)image.Height;
                MipMapLevels = mipMapLevels;

                if (compressTexture && compressedFormat.HasValue) {
                    fixed (byte* dataPtr = image.Data) {
                        _gl.CompressedTexImage2D(TextureTarget.Texture2D,
                            0, compressedFormat.Value, Width, Height, 0, (uint)image.Data.Length, dataPtr);
                    }
                }
                else {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, Width, Height, 0, pixelFormat, pixelType, in image.Data[0]);
                }

                ConfigureTexture(wrapModeS, wrapModeT, minFilter, magFilter, generateMipmaps);
            }

            public Texture(Window window,
                   byte[] pixelData,
                   uint width,
                   uint height,
                   TextureWrapMode wrapModeS = TextureWrapMode.Repeat,
                   TextureWrapMode wrapModeT = TextureWrapMode.Repeat,
                   TextureMinFilter minFilter = TextureMinFilter.Linear,
                   TextureMagFilter magFilter = TextureMagFilter.Linear,
                   InternalFormat internalFormat = InternalFormat.Rgba,
                   PixelFormat pixelFormat = PixelFormat.Rgba,
                   PixelType pixelType = PixelType.UnsignedByte,
                   bool generateMipmaps = true,
                   uint mipMapLevels = 0,
                   bool compressTexture = false,
                   InternalFormat? compressedFormat = null) {

                _gl = window.GlContext;
                _handle = _gl.GenTexture();
                Bind();

                Width = width;
                Height = height;
                MipMapLevels = mipMapLevels;

                if (compressTexture && compressedFormat.HasValue) {
                    fixed (byte* dataPtr = pixelData) {
                        _gl.CompressedTexImage2D(TextureTarget.Texture2D, 0, compressedFormat.Value, Width, Height, 0, (uint)pixelData.Length, dataPtr);
                    }
                }
                else {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, Width, Height, 0, pixelFormat, pixelType, in pixelData[0]);
                }

                ConfigureTexture(wrapModeS, wrapModeT, minFilter, magFilter, generateMipmaps);
            }

        #endregion

        #region Public methods

            public void Bind(TextureUnit unit = TextureUnit.Texture0) {
                if(_disposed) throw new ObjectDisposedException(null);
                _gl.ActiveTexture(unit);
                _gl.BindTexture(TextureTarget.Texture2D, _handle);
            }

            public void Unbind() {
                if (_disposed) throw new ObjectDisposedException(null);
                _gl.BindTexture(TextureTarget.Texture2D, 0);
            }

            public void UpdateTextureData(byte[] pixelData, uint width, uint height, PixelFormat pixelFormat = PixelFormat.Rgba, PixelType pixelType = PixelType.UnsignedByte) {
                if(_disposed) throw new ObjectDisposedException(null);

                Width = width;
                Height = height;

                Bind();
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, Width, Height, 0, pixelFormat, pixelType, in pixelData[0]);
            }

            public void SetWrapMode(TextureWrapMode wrapModeS, TextureWrapMode wrapModeT) {
                if (_disposed) throw new ObjectDisposedException(null);

                Bind();
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapModeS);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapModeT);
            }

            public void SetFilterMode(TextureMinFilter minFilter, TextureMagFilter magFilter) {
                if (_disposed) throw new ObjectDisposedException(null);

                Bind();
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            }

            public void Dispose() {
                if (_disposed) return;

                _gl.DeleteTexture(_handle);
                _disposed = true;
            }

        #endregion

        #region Private methods

            private void ConfigureTexture(TextureWrapMode wrapModeS, TextureWrapMode wrapModeT, TextureMinFilter minFilter, TextureMagFilter magFilter, bool generateMipmaps) {
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapModeS);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapModeT);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

                if (!generateMipmaps) return;

                if (MipMapLevels > 0) {
                    for (uint i = 1; i < MipMapLevels; i++) {
                        var levelWidth = Math.Max(1, Width >> (int)i);
                        var levelHeight = Math.Max(1, Height >> (int)i);

                        _gl.TexImage2D(TextureTarget.Texture2D, (int)i, InternalFormat.Rgba, levelWidth, levelHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
                    }
                }
                else {
                    _gl.GenerateMipmap(TextureTarget.Texture2D);
                }
            }

        #endregion
    }
}
