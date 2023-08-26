using Lumina3D.Components;
using Lumina3D.Internal;
using Lumina3D;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace Lumina3D.Components
{
    public class MeshRenderer : EntityComponent
    {
        public Color MeshColor { get; set; } = Color.White;

        private MeshComponent meshComponent;
        private TransformComponent transformComponent;

        public ShaderCache ShaderCache { get; set; } = new ShaderCache();

        public override void Awake()
        {
            if(ShaderCache != null)
            {
            ShaderCache = new ShaderCache();
            }
            
            meshComponent = Entity.GetComponent<MeshComponent>();
            transformComponent = Entity.GetComponent<TransformComponent>();

        }

        public void Render(CameraComponent cam, Matrix4 viewMatrix, Matrix4 projectionMatrix,List<Vector3> lightDirections)
        {


            Matrix4 worldMatrix = transformComponent.GetModelMatrix();
            Matrix4 mvpMatrix = worldMatrix * viewMatrix * projectionMatrix;
            if (meshComponent.Enabled)
            {
                meshComponent.Draw(cam, mvpMatrix, worldMatrix, cam.position);
            }

        }
    }
}
