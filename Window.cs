using System.Numerics;

using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Input;

using Color = System.Drawing.Color;

namespace SimpleOpenGL {
    public class Window : IDisposable{
        #region Variables

            private readonly IWindow _window;
            private GL _gl = null!;
            private IInputContext _input = null!;

            private GLEnum _blendSrc = GLEnum.SrcAlpha;
            private GLEnum _blendDst = GLEnum.OneMinusSrcAlpha;

            public event Action<double>? OnRender;
            public event Action? OnLoad;
            public event Action? OnClose;
            public event Action<Vector2>? OnResize;
            public event Action<Vector2>? OnMove;
            public event Action<Vector2>? OnFrameBufferResize;
            public event Action<string[]>? OnFileDrop;
            public event Action<bool>? OnFocusChanged;
             
            private Color _clearColor = Color.White;

            private bool _closed;

        #endregion

        #region Properties

            public GL GlContext => _gl;
            public IWindow WindowContext => _window;
            public IInputContext InputContext => _input;

            public GLEnum BlendSrc => _blendSrc;
            public GLEnum BlendDst => _blendDst;

            public Color ClearColor {
                set {
                    _clearColor = value;
                    _gl.ClearColor(value.R, value.G, value.B, value.A);
                }
                get => _clearColor;
            }

            public bool Closed => _closed;

            public bool VSync {
                set => _window.VSync = value;
                get => _window.VSync;
            }

            public IReadOnlyList<IMouse> Mice => _input.Mice;
            public IReadOnlyList<IKeyboard> Keyboards => _input.Keyboards;
            public IReadOnlyList<IJoystick> Joysticks => _input.Joysticks;
            public IReadOnlyList<IGamepad> Gamepads => _input.Gamepads;

        #endregion

        #region Constructors

            public Window(string title) {
                var options = WindowOptions.Default;

                options.Title = title;
                options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);

                _window = Silk.NET.Windowing.Window.Create(options);

                InitializeWindow();
            }

            public Window(Vector2 size, string title) {
                var options = WindowOptions.Default;

                options.Size = new Silk.NET.Maths.Vector2D<int>((int)size.X, (int)size.Y);
                options.Title = title;

                _window = Silk.NET.Windowing.Window.Create(options);

                InitializeWindow();
            }

            public Window(Vector2 size, string title, WindowOptions options) {
                options.Size = new Silk.NET.Maths.Vector2D<int>((int)size.X, (int)size.Y);
                options.Title = title;

                _window = Silk.NET.Windowing.Window.Create(options);
                
                InitializeWindow();
            }

        #endregion

        #region Public Methods

            public void Run() {
                if(_closed) throw new InvalidOperationException("Window is _closed.");

                try {
                    _window.Run();
                }
                catch (Silk.NET.GLFW.GlfwException) {
                    throw new InvalidOperationException("Error opening the window. Are you trying to open multiple windows at the same time? That is not supported");
                }
                
                Dispose();
            }

            public void Close() {
                if(_closed) return;

                _closed = true;

                _window.Close();
            }

            public void SetWindowPosition(Vector2 position) {
                if(_closed) throw new InvalidOperationException("Window is _closed.");

                _window.Position = new Silk.NET.Maths.Vector2D<int>((int)position.X, (int)position.Y);
            }

            public void SetWindowSize(Vector2 size) {
                if(_closed) throw new InvalidOperationException("Window is _closed.");

                _window.Size = new Silk.NET.Maths.Vector2D<int>((int)size.X, (int)size.Y);
            }

            public void SetBlendFunc(GLEnum src, GLEnum dst) {
                if (_closed) throw new InvalidOperationException("Window is _closed.");

                _blendSrc = src;
                _blendDst = dst;

                _gl.BlendFunc(src, dst);
            }

            public void Dispose() {
                if(_closed) return;

                _window.Dispose();
                _gl.Dispose();
                _input.Dispose();
            }

        #endregion

        #region Private Methods

            private void InitializeWindow() {
                _window.Load += OnLoadMethod;
                _window.Render += OnRenderMethod;
                _window.Closing += OnCloseMethod;
                _window.Resize += OnResizeMethod;
                _window.Move += OnMoveMethod;
                _window.FileDrop += OnFileDropMethod;
                _window.FocusChanged += OnFocusChangedMethod;
                _window.FramebufferResize += OnFramebufferResizeMethod;
            }

            private void OnLoadMethod() {
                _gl = GL.GetApi(_window);

                _gl.Enable(GLEnum.DepthTest);
                _gl.Enable(GLEnum.Blend);

                _gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

                _input = _window.CreateInput();

                OnLoad?.Invoke();
            }

            private void OnRenderMethod(double deltaTime) {
                OnRender?.Invoke(deltaTime);
            }

            private void OnCloseMethod() {
                OnClose?.Invoke();
            }

            private void OnResizeMethod(Silk.NET.Maths.Vector2D<int> newSize) {
                _gl.Viewport(newSize);
                OnResize?.Invoke((Vector2)newSize);
            }

            private void OnMoveMethod(Silk.NET.Maths.Vector2D<int> newPosition) {
                OnMove?.Invoke((Vector2)newPosition);
            }

            private void OnFileDropMethod(string[] files) {
                OnFileDrop?.Invoke(files);
            }

            private void OnFocusChangedMethod(bool focused) {
                OnFocusChanged?.Invoke(focused);
            }

            private void OnFramebufferResizeMethod(Silk.NET.Maths.Vector2D<int> newSize) {
                OnFrameBufferResize?.Invoke((Vector2)newSize);
            }

        #endregion
    }
}
