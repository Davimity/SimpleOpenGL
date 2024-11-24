
using System.Numerics;

namespace SimpleOpenGL.Interfaces {
    public interface ICamera {

        #region Properties

            public Vector3 Position { get; set; }

            public Vector3 Front { get; }
            public Vector3 Up { get; }
            public Vector3 Right { get; }

            public Vector3 WorldUp { get; }

            public Quaternion Rotation { get; set; }

            public float Near { get; set; }
            public float Far { get; set; }
            
            public float Fov { get; set; }
            public float Aspect { get; set; }

        #endregion

        #region Public Methods

            public Matrix4x4 GetViewMatrix();
            public Matrix4x4 GetProjectionMatrix();
            
        #endregion
    }
}
