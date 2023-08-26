using System;
using System.Windows.Forms;
using Lumina3D.Components;
using Lumina3D.Internal;
using Lumina3D;
using System.Drawing;
using OpenTK;
using System.ComponentModel;

namespace ECS3D.ECSEngine.Control
{
    public partial class ECSControl : UserControl
    {
        public Engine Engine { get; set; }

        public ECSControl()
        {
            InitializeComponent();
        }

        private void ECSControl_Load(object sender, EventArgs e)
        {
            Engine = new Engine(this);
       
            //Engine.StartEngine();
        }

        private void glContext_Paint(object sender, PaintEventArgs e)
        {
            glContext.MakeCurrent();
            
        }

        Point lastMousePos;

        public void CenterMouseCursor()
        {
            Invoke((MethodInvoker)delegate {
                Point center = new Point(glContext.Width / 2, glContext.Height / 2);
                Cursor.Position = glContext.PointToScreen(center);
                lastMousePos = center;
            });


        }



        private void glContext_KeyDown(object sender, KeyEventArgs e)
        {
          
        }

        private void glContext_MouseMove(object sender, MouseEventArgs e)
        {
          
        }

        public GLControl GL()
        {

            return glContext;
        }

        private void ECSControl_Resize(object sender, EventArgs e)
        {
            if(Engine != null)
            {
                Engine.FixAspect();
            }
        }

        private void glContext_Load(object sender, EventArgs e)
        {

        }
    }
}
