using Assimp;
using Lumina3D.Components;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
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
        private Engine engine;
        private IWindow SilkView = null;
        private GL gl = null;
        private IInputContext inputCtx = null;


        public Renderer(Engine engine)
        {
            this.engine = engine;
        }

        public IWindow GetWindow()
        {
            return SilkView;
        }

        public GL GetGL()
        {
            return gl;
        }

        public IInputContext GetInputContext()
        {
            return inputCtx;
        }

        public void InitializeGL()
        {
            SilkView = Window.Create(WindowOptions.Default);
            gl = null;
            inputCtx = null;

            //Setup gl and input context
            SilkView.Load += () =>
            {
                gl = SilkView.CreateOpenGL();
                inputCtx = SilkView.CreateInput();
            };

            //handle resizes
            SilkView.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                gl.Viewport(s);
            };

            //The render function
            SilkView.Render += delta =>
            {
                //Add skybox, imgui or other draws here
                engine.DeltaTime = delta;
                engine.Update();
                DrawFrame();

            };
        }

        public GL Gl()
        {
            return gl;
        }

        public void DrawFrame()
        {

            if (engine.activeCamera != null)
            {
                var activeCam = engine.activeCamera;

                SilkView.MakeCurrent();

                gl.ClearColor(Color.Gray);

                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                gl.DepthFunc(DepthFunction.Less);
                gl.Enable(EnableCap.PolygonOffsetFill);
                gl.Enable(EnableCap.DepthTest);
                gl.Enable(EnableCap.CullFace);
                gl.CullFace(GLEnum.CullFaceMode);

                var viewMatrix = activeCam.GetViewMatrix();
                var projectionMatrix = activeCam.GetProjectionMatrix();

                foreach (var renderer in engine.GetComponents<MeshRenderer>())
                {
                    uint queryId;
                    gl.GenQueries(1, out queryId);
                    gl.BeginQuery(QueryTarget.SamplesPassed, queryId);
                    renderer.Render(activeCam, viewMatrix, projectionMatrix, new List<System.Numerics.Vector3> { new System.Numerics.Vector3(0, 0, 0) });
                    int result;
                    gl.GetQueryObject(queryId, QueryObjectParameterName.QueryResult, out result);

                    // Delete the query object
                    gl.DeleteQueries(1, queryId);
                    if (result == 0)
                    {
                        renderer.Disable();
                    }
                    else
                    {
                        renderer.Enable();
                    }
                }

                SilkView.SwapBuffers();
            }
        }

    }
}
