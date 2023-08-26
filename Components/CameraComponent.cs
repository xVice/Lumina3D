using OpenTK;
using System;
using Lumina3D.Components;
using Lumina3D.Internal;
using Lumina3D;
using OpenTK.Input;
using System.Drawing;

namespace Lumina3D.Components
{
    public class CameraComponent : EntityComponent
    {
        // Camera properties
        public Vector3 position { get; set; }
        public Vector3 front { get; set; }
        public Vector3 up { get; set; }
        public float yaw { get; set; }
        public float pitch { get; set; }
        public float movementSpeed { get; set; } = .1f;
        public float mouseSensitivity { get; set; } = .1f;
        public enum CameraMovement { Forward, Backward, Left, Right }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }



        // Calculate the view matrix
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }


        // Calculate the projection matrix
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
        }

        public void MoveCam(KeyboardState kbState)
        {
            if (kbState.IsKeyDown(Key.W))
            {
                Move(CameraComponent.CameraMovement.Forward);
            }
            else if (kbState.IsKeyDown(Key.S))
            {
                Move(CameraComponent.CameraMovement.Backward);
            }
            else if (kbState.IsKeyDown(Key.A))
            {
                Move(CameraComponent.CameraMovement.Left);
            }
            else if (kbState.IsKeyDown(Key.D))
            {
                Move(CameraComponent.CameraMovement.Right);
            }
        }

        private Point lastMousePos;
        public void RotateCam(MouseState mouseState)
        {
            if (mouseState.IsButtonDown(MouseButton.Left))
            {
                // Calculate the mouse delta
                Vector2 mouseDelta = new Vector2(mouseState.X - lastMousePos.X, mouseState.Y - lastMousePos.Y);

                // Rotate the camera
                Rotate(mouseDelta);

                // Center the mouse cursor again
                Engine.GetControl().CenterMouseCursor();
            }
        }

        public static CameraComponent CreateCamera(Engine engine, string name, Vector3 pos)
        {
            float fieldOfViewDegrees = 60f;
            float fieldOfViewRadians = (float)fieldOfViewDegrees * (float)(Math.PI / 180f);
            var entity = engine.CreateEntity(name);
            var camera = new CameraComponent
            {
                position = pos,
                front = Vector3.UnitZ,
                up = Vector3.UnitY,
                FieldOfView = fieldOfViewRadians,
                AspectRatio = (float)engine.GetControl().ClientSize.Width / engine.GetControl().ClientSize.Height, // Convert to float before division
                NearPlane = 0.1f,
                FarPlane = 1000f
            };

            entity.AddComponent<CameraComponent>(camera);
            return camera;
        }

        public void SetActive()
        {
            Engine.activeCamera = this;
        }

        public void Move(CameraMovement direction)
        {
            if (direction == CameraMovement.Forward)
                position += front * movementSpeed;
            if (direction == CameraMovement.Backward)
                position -= front * movementSpeed;
            if (direction == CameraMovement.Left)
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * movementSpeed;
            if (direction == CameraMovement.Right)
                position += Vector3.Normalize(Vector3.Cross(front, up)) * movementSpeed;
        }

        // Function to rotate the camera using mouse input

        public void Rotate(Vector2 mouseDelta)
        {
            yaw += mouseDelta.X * mouseSensitivity;
            pitch -= mouseDelta.Y * mouseSensitivity;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            UpdateFrontVector();
        }

        private void UpdateFrontVector()
        {
            Vector3 newFront = new Vector3();
            newFront.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
            newFront.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            newFront.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));

            front = Vector3.Normalize(newFront);
        }


    }


}
