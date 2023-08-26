using Assimp.Configs;
using Assimp;
using glTFLoader.Schema;
using glTFLoader;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.IO;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using Lumina3D.Components;
using Lumina3D.Internal;
using Lumina3D;
using static OpenTK.Graphics.OpenGL.GL;
using Lumina3D.GLumina;

namespace Lumina3D.Components
{
    public class MeshComponent : EntityComponent
    {
        public Assimp.Scene Scene { get; set; } = null;

        private MeshRenderer renderer;
        public Gltf Gltf { get; set; } = null;
        public Color MeshColor { get; set; } = Color.White;

        bool buildingCache = false;

        public MeshComponent(Assimp.Scene scene)
        {
            Scene = scene;
        }

        public MeshComponent(Gltf gltf)
        {
            Gltf = gltf;
        }

        private static void CheckGLError(string operation)
        {
            ErrorCode errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError)
            {
                throw new Exception($"OpenGL error ({errorCode}) occurred during {operation}");
            }
        }


        public override void Awake()
        {
            renderer = Entity.GetComponent<MeshRenderer>();
            BuildShaderCache();
        }


        public override void OnEnable()
        {
            BuildShaderCache();

        }

        public void BuildShaderCache()
        {
            if (!buildingCache)
            {
                buildingCache = true;
                Console.WriteLine("[SHCH] - Building Shader Cache for: " + Entity.EntityName);
                if (Scene != null)
                {
                    foreach (var mesh in Scene.Meshes)
                    {
                        //renderer.AssimpShaderCache.Add(mesh, BuildAssimpShader(mesh));
                    }
                }
                else if (Gltf != null)
                {
                    foreach (var material in Gltf.Materials)
                    {
                        renderer.ShaderCache.CacheShader(Shader.BuildFromGLTF("./shaders/builtin/pbr.vert", "./shaders/builtin/pbr.frag", material));
                    }
                }

            }

        }

        public override void OnDisable()
        {
            Console.WriteLine("[SHCH] - Clearing cached shaders for: " + Entity.EntityName);
            renderer.ShaderCache.ClearCache();
        }

        //Could make multiple draw method for .stl or .obj, .gltf has everything though so why not use it?
        public void Draw(CameraComponent cam, Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 lightDirection)
        {
            foreach (var mesh in Gltf.Meshes)
            {
                foreach (var primitive in mesh.Primitives)
                {
                    //setup gltf stuff
                    var vertexAccessor = Gltf.Accessors[primitive.Attributes["POSITION"]];
                    var vertexBufferView = Gltf.BufferViews[(int)vertexAccessor.BufferView];
                    var indexAccessor = Gltf.Accessors[(int)primitive.Indices];
                    var indexBufferView = Gltf.BufferViews[(int)indexAccessor.BufferView];
                    var material = (int)primitive.Material;
                    var modelMatrix = viewMatrix + projectionMatrix; //might be wrong i dont know math!
                    var normalMatrix = new Matrix3(Matrix4.Transpose(Matrix4.Invert(modelMatrix)));

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferView.Buffer);

                    // Get the shader from the ShaderCache
                    int shaderProgram = renderer.ShaderCache.GetCachedShader(material).ShaderID;
                    GL.UseProgram(shaderProgram);

                    // Set uniform values
                    GLue.CreateUniform1(shaderProgram, "Time", Engine.Time);
                    GLue.CreateUniform3(shaderProgram, "LightDirection", lightDirection.X, lightDirection.Y, lightDirection.Z);
                    GLue.CreateUniform3(shaderProgram, "CameraPos", cam.position.X, cam.position.Y, cam.position.Z);
                    GLue.CreateMatrix4(shaderProgram, "ViewMatrix", viewMatrix);
                    GLue.CreateMatrix4(shaderProgram, "ProjectionMatrix", projectionMatrix);
                    GLue.CreateMatrix3(shaderProgram, "NormalMatrix", normalMatrix);

                    // Draw with the appropriate shader
                    GL.DrawElements((BeginMode)primitive.Mode, (int)indexAccessor.Count, DrawElementsType.UnsignedInt, 0);
                }
            }
        }



        public static MeshComponent LoadFromFile(string filePath, PostProcessSteps ppSteps, params PropertyConfig[] configs)
        {
            if (!File.Exists(filePath))
                return null;

            AssimpContext importer = new AssimpContext();
            if (configs != null)
            {
                foreach (PropertyConfig config in configs)
                    importer.SetConfig(config);
            }

            Assimp.Scene scene = importer.ImportFile(filePath, ppSteps);
            if (scene == null)
                return null;

            importer.Dispose();

            MeshComponent model = new MeshComponent(scene);
            return model;
        }

        public static MeshComponent LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var gltf = Interface.LoadModel(filePath);
            if (gltf == null)
                return null;



            MeshComponent model = new MeshComponent(gltf);
            return model;
        }

    }


}
