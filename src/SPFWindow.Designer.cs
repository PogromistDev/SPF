namespace SPF
{
    partial class SPFWindow
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SPFWindow));
            this.imageViewer = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.renderButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripCButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripNButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.filePath = new System.Windows.Forms.ToolStripTextBox();
            this.browseButton = new System.Windows.Forms.ToolStripButton();
            this.convertToSpfButton = new System.Windows.Forms.ToolStripButton();
            this.saveImageButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stripCountLabel = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.imageViewer)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageViewer
            // 
            this.imageViewer.BackColor = System.Drawing.Color.DimGray;
            this.imageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewer.Location = new System.Drawing.Point(0, 0);
            this.imageViewer.Name = "imageViewer";
            this.imageViewer.Size = new System.Drawing.Size(1136, 681);
            this.imageViewer.TabIndex = 0;
            this.imageViewer.TabStop = false;
            this.imageViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.imageViewer_Paint);
            this.imageViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageViewer_MouseDown);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.DimGray;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderButton,
            this.toolStripCButton,
            this.toolStripNButton,
            this.toolStripSeparator2,
            this.filePath,
            this.browseButton,
            this.convertToSpfButton,
            this.saveImageButton,
            this.toolStripSeparator1,
            this.stripCountLabel});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip1.Size = new System.Drawing.Size(1136, 32);
            this.toolStrip1.TabIndex = 7;
            // 
            // renderButton
            // 
            this.renderButton.BackColor = System.Drawing.Color.Transparent;
            this.renderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.renderButton.ForeColor = System.Drawing.Color.White;
            this.renderButton.Image = ((System.Drawing.Image)(resources.GetObject("renderButton.Image")));
            this.renderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(70, 29);
            this.renderButton.Text = "Render SPF";
            this.renderButton.Click += new System.EventHandler(this.renderButton_Click);
            // 
            // toolStripCButton
            // 
            this.toolStripCButton.BackColor = System.Drawing.Color.Transparent;
            this.toolStripCButton.CheckOnClick = true;
            this.toolStripCButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripCButton.ForeColor = System.Drawing.Color.White;
            this.toolStripCButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripCButton.Image")));
            this.toolStripCButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripCButton.Name = "toolStripCButton";
            this.toolStripCButton.Size = new System.Drawing.Size(23, 29);
            this.toolStripCButton.Text = "C";
            this.toolStripCButton.ToolTipText = "Colorize strips";
            // 
            // toolStripNButton
            // 
            this.toolStripNButton.CheckOnClick = true;
            this.toolStripNButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripNButton.ForeColor = System.Drawing.Color.White;
            this.toolStripNButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripNButton.Image")));
            this.toolStripNButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNButton.Name = "toolStripNButton";
            this.toolStripNButton.Size = new System.Drawing.Size(23, 29);
            this.toolStripNButton.Text = "N";
            this.toolStripNButton.ToolTipText = "Normalize strip length";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 32);
            // 
            // filePath
            // 
            this.filePath.CausesValidation = false;
            this.filePath.Name = "filePath";
            this.filePath.Size = new System.Drawing.Size(250, 32);
            this.filePath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.filePath_KeyUp);
            // 
            // browseButton
            // 
            this.browseButton.BackColor = System.Drawing.Color.Transparent;
            this.browseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.browseButton.ForeColor = System.Drawing.Color.White;
            this.browseButton.Image = ((System.Drawing.Image)(resources.GetObject("browseButton.Image")));
            this.browseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(49, 29);
            this.browseButton.Text = "Browse";
            this.browseButton.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.browseButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.browseButton.ToolTipText = "Browse file";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // convertToSpfButton
            // 
            this.convertToSpfButton.BackColor = System.Drawing.Color.Transparent;
            this.convertToSpfButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.convertToSpfButton.ForeColor = System.Drawing.Color.White;
            this.convertToSpfButton.Image = ((System.Drawing.Image)(resources.GetObject("convertToSpfButton.Image")));
            this.convertToSpfButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.convertToSpfButton.Name = "convertToSpfButton";
            this.convertToSpfButton.Size = new System.Drawing.Size(89, 29);
            this.convertToSpfButton.Text = "Convert to SPF";
            this.convertToSpfButton.ToolTipText = "Convert browsed image to SPF";
            this.convertToSpfButton.Click += new System.EventHandler(this.convertToSpfButton_Click);
            // 
            // saveImageButton
            // 
            this.saveImageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveImageButton.ForeColor = System.Drawing.Color.White;
            this.saveImageButton.Image = ((System.Drawing.Image)(resources.GetObject("saveImageButton.Image")));
            this.saveImageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveImageButton.Name = "saveImageButton";
            this.saveImageButton.Size = new System.Drawing.Size(35, 29);
            this.saveImageButton.Text = "Save";
            this.saveImageButton.Click += new System.EventHandler(this.saveImageButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
            // 
            // stripCountLabel
            // 
            this.stripCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.stripCountLabel.ForeColor = System.Drawing.Color.White;
            this.stripCountLabel.Name = "stripCountLabel";
            this.stripCountLabel.Size = new System.Drawing.Size(68, 29);
            this.stripCountLabel.Text = "Strip count:";
            this.stripCountLabel.Visible = false;
            // 
            // SPFWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1136, 681);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.imageViewer);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SPFWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPF";
            this.Load += new System.EventHandler(this.SPFWindow_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SPFWindow_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SPFWindow_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SPFWindow_KeyDown);
            this.Resize += new System.EventHandler(this.SPFWindow_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.imageViewer)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imageViewer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton renderButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton browseButton;
        private System.Windows.Forms.ToolStripTextBox filePath;
        private System.Windows.Forms.ToolStripLabel stripCountLabel;
        private System.Windows.Forms.ToolStripButton convertToSpfButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripCButton;
        private System.Windows.Forms.ToolStripButton toolStripNButton;
        private System.Windows.Forms.ToolStripButton saveImageButton;
    }
}

