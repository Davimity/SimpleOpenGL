using Silk.NET.OpenGL;

using System.Numerics;

namespace SimpleOpenGL {
    public class Material : IDisposable {
        
        #region Variables

            private readonly Shader _shader;
            private readonly Dictionary<string, Texture> _properties = new();
            private readonly Dictionary<string, object> _uniforms = new();
            private readonly GL _gl;

            private bool _disposed;

        #endregion

        #region _properties

            public Shader Shader => _shader;

        #endregion

        #region Constructors

            public Material(Window window, Shader shader) {
                _shader = shader;
                _gl = window.GlContext;
            }

        #endregion

        #region Public Methods

            public void Use() {
                if(_disposed) throw new ObjectDisposedException("Material");

                _shader.Use();

                foreach (var texture in _properties) {
                    var unitIndex = GetTextureUnitIndexFromName(texture.Key);
                    texture.Value.Bind(TextureUnit.Texture0 + unitIndex);
                    _shader.SetUniform(texture.Key, unitIndex);
                }

                foreach (var uniform in _uniforms) 
                    SetUniformInternal(uniform.Key, uniform.Value);
            }

            public void SetTexture(string name, Texture texture) {
                if(_disposed) throw new ObjectDisposedException("Material");
                _properties[name] = texture;
            }

            public void RemoveTexture(string name) {
                if (_disposed) throw new ObjectDisposedException("Material");

                if (_properties.ContainsKey(name)) 
                    _properties.Remove(name);
            }

            public void SetUniform(string name, int value) {
                if(_disposed) throw new ObjectDisposedException("Material");
                _uniforms[name] = value;
            }

            public void RemoveUniform(string name) {
                if(_disposed) throw new ObjectDisposedException("Material");
                if(_uniforms.ContainsKey(name))
                    _uniforms.Remove(name);
            }

            public void Dispose() {
                _shader.Dispose();
                foreach (var texture in _properties) texture.Value.Dispose();

                _properties.Clear();
                _uniforms.Clear();

                _disposed = true;
            }

        #endregion

        #region Private methods

            private int GetTextureUnitIndexFromName(string name) {
                var index = int.TryParse(name[^1].ToString(), out var result) ? result : 0;
                return index;
            }

            private void SetUniformInternal(string name, object value) {
                switch (value) {
                    case int i:
                        _shader.SetUniform(name, i);
                        break;
                    case float f:
                        _shader.SetUniform(name, f);
                        break;
                    case bool b:
                        _shader.SetUniform(name, b);
                        break;
                    case Vector2 v2:
                        _shader.SetUniform(name, v2);
                        break;
                    case Vector3 v3:
                        _shader.SetUniform(name, v3);
                        break;
                    case Vector4 v4:
                        _shader.SetUniform(name, v4);
                        break;
                    case Matrix4x4 m4:
                        _shader.SetUniform(name, m4);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported uniform type: {value.GetType()}");
                }
            }

        #endregion

    }
}
