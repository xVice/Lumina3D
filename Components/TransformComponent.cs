using OpenTK;
using Lumina3D.Components;
using Lumina3D.Internal;
using Lumina3D;

namespace Lumina3D.Components
{
    public class TransformComponent : EntityComponent
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;

        // Helper methods to manipulate the transform
        public void Translate(Vector3 translation)
        {
            Position += translation;
        }

        public void Rotate(Quaternion rotation)
        {
            Rotation += rotation;
        }

        public void ScaleBy(Vector3 scale)
        {
            Scale *= scale;
        }

        // Convenience methods to get transformation matrices
        public Matrix4 GetModelMatrix()
        {
            Matrix4 translationMatrix = Matrix4.CreateTranslation(Position);
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
            Matrix4 scaleMatrix = Matrix4.CreateScale(Scale);

            return translationMatrix * rotationMatrix * scaleMatrix;

        }
    }
}
