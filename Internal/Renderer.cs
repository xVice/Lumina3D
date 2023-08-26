using Assimp;
using ECS3D.ECSEngine.Control;
using Lumina3D.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lumina3D.Internal
{
    public class Renderer
    {
        private ECSControl control;

        private Engine Engine { get => control.Engine; }

        public Renderer(ECSControl control)
        {
            this.control = control;
        }

        public void DrawFrame()
        {

            if (Engine.activeCamera != null)
            {
                var activeCam = Engine.activeCamera;

                control.GL().MakeCurrent();

                GL.ClearColor(Color.Gray);

                int queryId;

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.DepthFunc(DepthFunction.Less);
                GL.Enable(EnableCap.PolygonOffsetFill);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
                GL.GenQueries(1, out queryId);
                GL.BeginQuery(QueryTarget.SamplesPassed, queryId);


                var viewMatrix = activeCam.GetViewMatrix();
                var projectionMatrix = activeCam.GetProjectionMatrix();

                // Render the skybox first
                //RenderSkybox();

                // Render the mesh
                foreach (var renderer in Engine.GetComponents<MeshRenderer>())
                {

                    renderer.Render(activeCam, viewMatrix, projectionMatrix, new List<Vector3> { new Vector3(0, 0, 0) });

                }


                GL.Disable(EnableCap.PolygonOffsetFill);
                GL.EndQuery(QueryTarget.SamplesPassed);

                // Get query result
                int result;
                GL.GetQueryObject(queryId, GetQueryObjectParam.QueryResult, out result);

                // Delete the query object
                GL.DeleteQueries(1, ref queryId);

                // Swap buffers
                control.GL().SwapBuffers();
                Thread.Sleep(1000);
            }
        }

    }
}
