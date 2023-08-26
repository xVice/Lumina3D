namespace ECS3D.ECSEngine.Control
{
    partial class ECSControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.glContext = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // glContext
            // 
            this.glContext.BackColor = System.Drawing.Color.Black;
            this.glContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glContext.Location = new System.Drawing.Point(0, 0);
            this.glContext.Name = "glContext";
            this.glContext.Size = new System.Drawing.Size(481, 263);
            this.glContext.TabIndex = 0;
            this.glContext.VSync = false;
            this.glContext.Load += new System.EventHandler(this.glContext_Load);
            this.glContext.Paint += new System.Windows.Forms.PaintEventHandler(this.glContext_Paint);
            this.glContext.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glContext_KeyDown);
            this.glContext.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glContext_MouseMove);
            // 
            // ECSControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.glContext);
            this.Name = "ECSControl";
            this.Size = new System.Drawing.Size(481, 263);
            this.Load += new System.EventHandler(this.ECSControl_Load);
            this.Resize += new System.EventHandler(this.ECSControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl glContext;
    }
}
