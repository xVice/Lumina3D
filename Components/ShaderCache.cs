using glTFLoader.Schema;
using Lumina3D.GLumina;
using Lumina3D.Internal;
using OpenTK.Graphics.OpenGL;
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

        public int ShaderID = 0;
        public ShaderInfo shaderInfo;
        public Material material;
        public enum LuminaShaderType { GLTF, Assimp }


        public Shader(string vertPath,string fragPath, ShaderInfo shaderInfo)
        {
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


        public static Shader BuildFromGLTF(string vert, string frag ,Material mat)
        {
            ShaderInfo shader = new ShaderInfo();
            shader.AlphaCutoff = mat.AlphaCutoff;
            shader.EmissiveFactor = mat.EmissiveFactor;
            shader.DoubleSided = mat.DoubleSided;
            
     


            var buildshader = new Shader(vert, frag, shader);
            buildshader.material = mat;

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

        public int Build()
        {
            string vertexShaderSource = LoadShaderSource(vertPath);
            string fragmentShaderSource = LoadShaderSource(fragPath);
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            GLue.CreateUniform1(shaderProgram, "AlphaCutoff", (double)shaderInfo.AlphaCutoff);
            GLue.CreateUniform1(shaderProgram, "DoubleSided", shaderInfo.DoubleSided ? 1.0 : 0.0);




            ShaderID = shaderProgram;
            return shaderProgram;
        }

        
    }

    public class ShaderCache : EntityComponent
    {

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
                GL.DeleteProgram(shader.ShaderID);
            }
            ShaderCacheList.Clear();
        }

 
        
    }
}
