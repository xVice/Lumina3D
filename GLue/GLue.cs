using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Lumina3D.GLumina
{
    public static class GLue
    {
        public static int CreateTexture(string imagePath)
        {
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            using (var image = new Bitmap(imagePath))
            {
                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                 ImageLockMode.ReadOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                              data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                image.UnlockBits(data);
            }

            // Set texture parameters if needed
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return textureId;
        }

        public static void UseTexture(int shaderProgram, int textureId, string uniformName, int textureUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            int uniformLocation = GL.GetUniformLocation(shaderProgram, uniformName);
            GL.Uniform1(uniformLocation, textureUnit); // Set the uniform to the texture unit
        }

        public static int CreateUniform1(int shaderProg,string name , double data)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.Uniform1(loc, data);
            return loc;
        }

        public static int CreateUniform2(int shaderProg, string name, double x, double y)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.Uniform2(loc, x, y);
            return loc;
        }

        public static int CreateUniform3(int shaderProg, string name, double x, double y, double z)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.Uniform3(loc, x,y,z);
            return loc;
        }

        public static int CreateUniform4(int shaderProg, string name, double x, double y, double z, double w)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.Uniform4(loc, x,y,z,w);
            return loc;
        }

        public static int CreateMatrix2(int shaderProg, string name, Matrix2 mat2)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.UniformMatrix2(loc, false, ref mat2);
            return loc;
        }

        public static int CreateMatrix3(int shaderProg, string name, Matrix3 mat3)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.UniformMatrix3(loc, false, ref mat3);
            return loc;
        }

        public static int SetupVertexAttrib(int shaderProg, string attributeName, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
        {
            int attribLocation = GL.GetAttribLocation(shaderProg, attributeName);
            GL.VertexAttribPointer(attribLocation, size, type, normalize, stride, offset);
            GL.EnableVertexAttribArray(attribLocation);
            return attribLocation;
        }


        public static int CreateMatrix4(int shaderProg, string name, Matrix4 mat4)
        {
            int loc = GL.GetUniformLocation(shaderProg, name);
            GL.UniformMatrix4(loc, false, ref mat4);
            return loc;
        }



    }
}
