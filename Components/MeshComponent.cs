using Assimp.Configs;
//using Assimp;
using glTFLoader.Schema;
using glTFLoader;
using System;
using System.Drawing;
using System.IO;
using Lumina3D.Internal;
using System.Numerics;
using Assimp;
using Silk.NET;
using Silk.NET.OpenGL;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Silk.NET.Input;

namespace Lumina3D.Components
{
    public class MeshComponent : EntityComponent
    {
        public Assimp.Scene Scene { get; set; } = null;

        private MeshRenderer renderer;
        public Gltf Gltf { get; set; } = null;
        public Color MeshColor { get; set; } = Color.White;

        public Renderer rend { get => Engine.renderer; }
        public GL Gl { get => Engine.renderer.Gl(); }
        bool buildingCache = false;

        public MeshComponent(Assimp.Scene scene)
        {
            Scene = scene;
        }

        public MeshComponent(Gltf gltf)
        {
            Gltf = gltf;
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

        /// <summary>
        /// BuildShaderCache for the mesh
        /// </summary>
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
                        renderer.ShaderCache.CacheShader(Shader.BuildFromGLTF(Engine,"./shaders/builtin/pbr.vert", "./shaders/builtin/pbr.frag", material));
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
        /// <summary>
        /// Draws the mesh to the current gl context given all the args
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="viewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="lightDirection"></param>
        public unsafe void Draw(CameraComponent cam, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, Vector3 lightDirection)
        {
            foreach (var mesh in Gltf.Meshes)
            {
                foreach (var primitive in mesh.Primitives)
                {
                    // Setup glTF stuff
                    var vertexAccessor = Gltf.Accessors[primitive.Attributes["POSITION"]];
                    var vertexBufferView = Gltf.BufferViews[(int)vertexAccessor.BufferView];
                    var indexAccessor = Gltf.Accessors[(int)primitive.Indices];
                    var indexBufferView = Gltf.BufferViews[(int)indexAccessor.BufferView];
                    var material = Gltf.Materials[(int)primitive.Material];

                    // Create and use a shader program
                    var shaderProgram = renderer.ShaderCache.GetCachedShader(material); // Implement this function
                    Gl.UseProgram(shaderProgram.ShaderID);

                    const uint positionLoc = 0; // We're defining a constant variable named positionLoc with the value 0.
                    Gl.EnableVertexAttribArray(positionLoc); // We're telling a graphics library (_gl) to enable an attribute at location 0.
                    Gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);


                    // Bind vertex and index buffers
                    Gl.BindBuffer(GLEnum.ArrayBuffer, (uint)vertexBufferView.Buffer);
                    Gl.BindBuffer(GLEnum.ElementArrayBuffer, (uint)indexBufferView.Buffer);

                    // Set shader uniforms (example: model, view, projection matrices)
                    var modelMatrix = CalculateModelMatrix(viewMatrix, projectionMatrix); // Implement this function
                    var normalMatrix = CalculateNormalMatrix(modelMatrix);

                    // Draw the mesh
                    Gl.DrawElements(GLEnum.Triangles, (uint)indexAccessor.Count, DrawElementsType.UnsignedShort, IntPtr.Zero);

                    // Cleanup
                    Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                    Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
                }
            }
        }


        public Matrix4x4 CalculateModelMatrix(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
        {
            return viewMatrix + projectionMatrix; //might be wrong i dont know
        }


        private Matrix3x3 CalculateNormalMatrix(Matrix4x4 modelMatrix)
        {
            Matrix4x4 normalMatrix;
            Matrix4x4.Invert(modelMatrix, out normalMatrix);
            normalMatrix = Matrix4x4.Transpose(normalMatrix);

            // Extract the 3x3 normal matrix from the 4x4 matrix
            return new Matrix3x3(
                normalMatrix.M11, normalMatrix.M12, normalMatrix.M13,
                normalMatrix.M21, normalMatrix.M22, normalMatrix.M23,
                normalMatrix.M31, normalMatrix.M32, normalMatrix.M33);


        }


        //Loads a file using assimp, isnt used anymore.
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

        //Loads a gltf file 
        /// <summary>
        /// Returns a MeshComponent from a .GLTF file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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
