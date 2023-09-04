using glTFLoader.Schema;
using Lumina3D.Internal;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lumina3D.Components.Shader;

namespace Lumina3D.Components
{
    public class Shader
    {
        public string vertPath;
        public string fragPath;

        public uint ShaderID = 0;
        public ShaderInfo shaderInfo;
        public Material material;
        public enum LuminaShaderType { GLTF, Assimp }
        public Engine engine;
        public GL gl { get => engine.renderer.Gl(); }

        public Shader(Engine engine,string vertPath,string fragPath, ShaderInfo shaderInfo)
        {
            this.engine = engine;
            this.vertPath = vertPath;
            this.fragPath = fragPath;
            this.shaderInfo = shaderInfo;
        }

        public struct ShaderInfo
        {
            public LuminaShaderType ShaderType { get; set; }
            public float AlphaCutoff { get; set; }
            public float[] EmissiveFactor { get; set; }
            public bool DoubleSided { get; set; }

            public Texture[] Textures { get; set; }
        }

        public struct Texture
        {

        }


        public static Shader BuildFromGLTF(Engine engine, string vert, string frag ,Material mat)
        {
            ShaderInfo shader = new ShaderInfo();
            shader.AlphaCutoff = mat.AlphaCutoff;
            shader.EmissiveFactor = mat.EmissiveFactor;
            shader.DoubleSided = mat.DoubleSided;
            
     


            var buildshader = new Shader(engine, vert, frag, shader);
            buildshader.material = mat;

            buildshader.ShaderID = buildshader.Build();

            return buildshader;
        }

        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = string.Empty;

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading shader source: " + e.Message);
            }

            return shaderSource;
        }

        public uint Build()
        {

         
            string vertexShaderSource = LoadShaderSource(vertPath);
            string fragmentShaderSource = LoadShaderSource(fragPath);
            uint vertShader = gl.CreateShader(ShaderType.VertexShader);
            uint fragShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(vertShader, vertexShaderSource);
            gl.ShaderSource(fragShader, fragmentShaderSource);

            gl.CompileShader(vertShader);

            gl.GetShader(vertShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + gl.GetShaderInfoLog(vertShader));

            gl.CompileShader(vertShader);

            gl.GetShader(fragShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + gl.GetShaderInfoLog(fragShader));

            gl.CompileShader(fragShader);

            uint shaderProg = gl.CreateProgram();
            gl.AttachShader(shaderProg, vertShader);
            gl.AttachShader(shaderProg, fragShader);

            gl.LinkProgram(shaderProg);

            gl.GetProgram(shaderProg, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + gl.GetProgramInfoLog(shaderProg));

            gl.DetachShader(shaderProg, vertShader);
            gl.DetachShader(shaderProg, fragShader);
            gl.DeleteShader(vertShader);
            gl.DeleteShader(fragShader);
          
            return shaderProg;

        }


    }

    public class ShaderCache : EntityComponent
    {

        public Engine engine;
        public GL gl { get => engine.renderer.Gl(); }
        public List<Shader> ShaderCacheList = new List<Shader>();

    
        public Shader GetCachedShader(int shaderId)
        {
            return ShaderCacheList.Where(x => x.ShaderID == shaderId).First();
        }

        public Shader GetCachedShader(Material mat)
        {
            return ShaderCacheList.Where(x => x.material == mat).First();
        }


        public Shader CacheShader(Shader Shader)
        {
            Shader.Build();
            ShaderCacheList.Add(Shader);
            return Shader;
        }

        public void ClearCache()
        {
            foreach(var shader in ShaderCacheList)
            {
                gl.DeleteProgram(shader.ShaderID);
            }
            ShaderCacheList.Clear();
        }

 
        
    }
}
