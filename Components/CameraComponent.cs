using Lumina3D.Internal;
using Lumina3D.Utils;
using Silk.NET.Input;
using System;
using System.Numerics;

namespace Lumina3D.Components
{
    public class CameraComponent : EntityComponent
    {
        // Camera properties
        public Vector3 Position { get; set; }
        public Vector3 Front { get; set; }
        public Vector3 Up { get; private set; }
        public float Yaw { get; set; }
        public enum CameraMovement { Forward, Backward, Left, Right }
        public float Pitch { get; set; }
        public float MovementSpeed { get; set; } = 0.1f;
        public float MouseSensitivity { get; set; } = 0.1f;
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }

        private readonly IInputContext _inputContext;

        public CameraComponent(IInputContext inputContext)
        {
            _inputContext = inputContext;
            Front = new Vector3(0.0f, 0.0f, -1.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
        }

        // Calculate the view matrix
        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        // Calculate the projection matrix
        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public void Move(CameraMovement direction)
        {
            float velocity = MovementSpeed;

            if (direction == CameraMovement.Backward)
                velocity *= -1.0f;

            if (direction == CameraMovement.Forward || direction == CameraMovement.Backward)
                Position += Front * velocity;
            if (direction == CameraMovement.Left || direction == CameraMovement.Right)
                Position += Vector3.Normalize(Vector3.Cross(Front, Up)) * velocity;
        }

        // Function to rotate the camera using mouse input
        public void Rotate(Vector2 delta)
        {
            var mouse = _inputContext.Mice[0];
            var mouseDelta = delta;

            Yaw += mouseDelta.X * MouseSensitivity;
            Pitch -= mouseDelta.Y * MouseSensitivity;

            if (Pitch > 89.0f)
                Pitch = 89.0f;
            if (Pitch < -89.0f)
                Pitch = -89.0f;

            UpdateFrontVector();
        }


        private void UpdateFrontVector()
        {
            float yawRadian = MathHelper.DegreesToRadians(Yaw);
            float pitchRadian = MathHelper.DegreesToRadians(Pitch);

            Vector3 newFront = new Vector3
            {
                X = (float)(Math.Cos(yawRadian) * Math.Cos(pitchRadian)),
                Y = (float)Math.Sin(pitchRadian),
                Z = (float)(Math.Sin(yawRadian) * Math.Cos(pitchRadian))
            };

            Front = Vector3.Normalize(newFront);
        }
    }
}
