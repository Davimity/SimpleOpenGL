using SimpleOpenGL.Interfaces;

using System.Numerics;

using SUQuaternion = SimpleUtilities.Geometry.Rotations.Quaternion;

namespace SimpleOpenGL {
    /// <summary>Represents a camera in a tridimensional space</summary>
    /// <remarks>NOT THREAD-SAFE</remarks>
    public class Camera : ICamera {

        #region Variables

            private Vector3 _worldUp;
            private Vector3 _position;
            private float _near;
            private float _far;
            private float _fov;
            private float _aspect;

            private Quaternion _rotation;

            private Vector3? _frontCache;
            private Vector3? _rightCache;
            private Vector3? _upCache;

            private Matrix4x4? _viewMatrixCache;
            private Matrix4x4? _projectionMatrixCache;
            private Matrix4x4? _inverseViewMatrixCache;

        #endregion

        #region Properties

            public Vector3 WorldUp {
                get => _worldUp;
                set {
                    _worldUp = Vector3.Normalize(value);
                    _viewMatrixCache = null;
                    _inverseViewMatrixCache = null;
                }
            }

            public Vector3 Position {
                get => _position;
                set {
                    _position = value;
                    _viewMatrixCache = null;
                    _inverseViewMatrixCache = null;
                }
            }

            public Quaternion Rotation {
                get => _rotation;
                set {
                    _rotation = value;
                    _frontCache = null;
                    _rightCache = null;
                    _upCache = null;

                    _viewMatrixCache = null;
                    _inverseViewMatrixCache = null;
                }
            }

            public Vector3 Front {
                get {
                    if (_frontCache.HasValue) return _frontCache.Value;

                    _frontCache = Vector3.Normalize(SUQuaternion.Rotate(_rotation, -Vector3.UnitZ));

                    return _frontCache.Value; 
                }
            }

            public Vector3 Back => -Front;

            public Vector3 Right {
                get {
                    if(_rightCache.HasValue) return _rightCache.Value;

                    _rightCache = Vector3.Normalize(Vector3.Cross(Front, _worldUp));
                    return _rightCache.Value;
                }
            }

            public Vector3 Left => -Right;

            public Vector3 Up {
                get {
                    if (_upCache.HasValue) return _upCache.Value;

                    _upCache = Vector3.Normalize(SUQuaternion.Rotate(_rotation, Vector3.UnitY));
                    return _upCache.Value;
                }
            }

            public Vector3 Down => -Up;

            public float Fov {
                get => _fov;
                set {
                    _fov = value;
                    _projectionMatrixCache = null;
                }
            }

            public float Aspect {
                get => _aspect;
                set {
                    _aspect = value;
                    _projectionMatrixCache = null;
                }
            }

            public float Near {
                get => _near;
                set {
                    _near = value;
                    _projectionMatrixCache = null;
                }
            }

            public float Far {
                get => _far;
                set {
                    _far = value;
                    _projectionMatrixCache = null;
                }
            }

        #endregion

        #region Constructors

            public Camera() {
                _position = new Vector3(0, 1, 1);
                _rotation = new Quaternion();

                Fov = 0.7853981634f;
                _aspect = 16f / 9f;

                _near = 0.01f;
                _far = 1000f;
            }

            public Camera(Vector3 position) {
                _position = position;
                _rotation = new Quaternion();

                Fov = 0.7853981634f;
                _aspect = 16f / 9f;

                _near = 0.01f;
                _far = 1000f;
            }

            public Camera(Vector3 position, Quaternion rotation) {
                _position = position;
                _rotation = rotation;

                Fov = 0.7853981634f;
                _aspect = 16f / 9f;

                _near = 0.01f;
                _far = 1000f;
            }

            public Camera(Vector3 position, Quaternion rotation, float fov, float aspect, float near, float far, Vector3 worldUp) {
                _position = position;
                _rotation = rotation;

                Fov = fov;
                _aspect = aspect;

                _near = near;
                _far = far;

                _worldUp = worldUp;
            }

            public Camera(Camera camera) {
                _position = camera._position;
                _rotation = camera._rotation;
                Fov = camera.Fov;
                _aspect = camera._aspect;
                _near = camera.Near;
                _far = camera._far;
                _worldUp = camera._worldUp;
            }

        #endregion

        #region Public Methods

            public void Move(Vector3 direction, float amount) {
                Position += Vector3.Normalize(direction) * amount;
            }

            public void Rotate(Vector3 axis, float angle) {
                Rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * _rotation);
            }

            public void Orbit(Vector3 target, float horizontalAngle, float verticalAngle) {
                var direction = Position - target;

                var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, horizontalAngle) * Quaternion.CreateFromAxisAngle(Right, verticalAngle);

                direction = Vector3.Transform(direction, rotation);
                Position = target + direction;
                Rotation = rotation * _rotation;
            }

            public Matrix4x4 GetProjectionMatrix() {
                if(_projectionMatrixCache.HasValue) return _projectionMatrixCache.Value;

                _projectionMatrixCache = Matrix4x4.CreatePerspectiveFieldOfView(_fov, _aspect, _near, _far);
                return _projectionMatrixCache.Value;
            }

            public Matrix4x4 GetViewMatrix() {
                if (_viewMatrixCache.HasValue) return _viewMatrixCache.Value;

                _viewMatrixCache = Matrix4x4.CreateLookAt(_position, _position + Front, _worldUp);
                return _viewMatrixCache.Value;
            }

            public Matrix4x4 GetInveseViewMatrix() {
                if(_inverseViewMatrixCache.HasValue) return _inverseViewMatrixCache.Value;

                Matrix4x4.Invert(GetViewMatrix(), out var inverseView);
                _inverseViewMatrixCache = inverseView;

                return _inverseViewMatrixCache.Value;
            }

        #endregion
    }
}
