using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SPF
{
    public partial class SPFWindow : Form
    {
        struct Settings
        {
            public Bitmap bit;
            public SPFFile spf;
            public Rectangle prevBound;
            public FormWindowState prevWindowState;
        }

        Settings settings;

        Color toolStripColor = Color.DimGray;
        Color imageViewerColor = Color.FromArgb(85, 85, 85);
        Bitmap transparencyBitmap = Properties.Resources.transparency;
        TextureBrush transparencyTexture;

        RectangleF rect;

        string pathToFile;

        //

        bool fullScreen = false;
        float scale = 1;
        Font scaleFont = new Font("Arial", 16);

        //

        Point draggingStartPoint, draggingPoint;
        bool startDragging = false, selfDragging = false;

        //

        string filter = Properties.Resources.filter;
        string filter1 = Properties.Resources.filter1;
        string filter2 = Properties.Resources.filter2;

        OpenFileDialog openDialog;
        SaveFileDialog saveDialog;

        public SPFWindow()
        {
            InitializeComponent();
        }

        //Functionality

        private void RestrictUI(bool spf = true)
        {
            if (spf)
            {
                renderButton.Visible = true;
                toolStripCButton.Visible = true;
                toolStripNButton.Visible = true;
                convertToSpfButton.Visible = false;
                saveImageButton.Visible = true;
                stripCountLabel.Visible = true;
            } else
            {
                renderButton.Visible = false;
                toolStripCButton.Visible = false;
                toolStripNButton.Visible = false;
                convertToSpfButton.Visible = true;
                saveImageButton.Visible = false;
                stripCountLabel.Visible = false;
            }
        }

        private void RestrictUIStartup()
        {
            renderButton.Visible = false;
            toolStripCButton.Visible = false;
            toolStripNButton.Visible = false;
            convertToSpfButton.Visible = false;
            saveImageButton.Visible = false;
            stripCountLabel.Visible = false;
        }

        private bool PointInRect(Point point, RectangleF rect)
        {
            return ( ((point.X > rect.X) && (point.X < rect.X + rect.Width)) && ((point.Y > rect.Y) && (point.Y < rect.Y + rect.Height)) );
        }

        private double PointDistance(Point p1, Point p2)
        {
            System.Windows.Point pp = new System.Windows.Point(p1.X, p1.Y);
            System.Windows.Point pp2 = new System.Windows.Point(p2.Y, p2.Y);

            return (pp - pp2).LengthSquared;
        }

        private void LoadSPF()
        {
            settings.spf = SPFFile.FromFile(pathToFile);
            if (settings.spf == null)
            {
                MessageBox.Show(@"This file doesn't exists ¯\_(ツ)_/¯");
                return;
            } else
            {
                settings.bit = settings.spf.GetImage();

                RestrictUI();

                imageViewer.Invalidate();
                stripCountLabel.Text = "Strip count: " + settings.spf.stripCount.ToString();
            }            
        }

        private void RenderSPF()
        {
            if (settings.spf == null)
            {
                LoadSPF();
            }
            else
            {
                settings.bit = settings.spf.GetImage(toolStripCButton.Checked, toolStripNButton.Checked);
                imageViewer.Invalidate();
            }
        }
        private void Browse()
        {
            string fileFormat = Path.GetExtension(pathToFile);

            if (fileFormat == ".spf")
            {
                filePath.Text = pathToFile;
                LoadSPF();

                return;
            }

            try
            {
                settings.bit = new Bitmap(pathToFile);
                settings.bit.SetResolution(96, 96);

                RestrictUI(false);

                filePath.Text = pathToFile;
                imageViewer.Invalidate();

                
            }
            catch (Exception)
            {
                MessageBox.Show(fileFormat + @" file format is not supported ¯\_(ツ)_/¯");
                pathToFile = String.Empty;
            }
        }

        private void ScaleUpdate(float scaleFactor)
        {
            scale = scaleFactor;
            imageViewer.Invalidate();
        }

        //Events

        private void SPFWindow_Load(object sender, EventArgs e)
        {
            imageViewer.BackColor = imageViewerColor;
            toolStrip1.BackColor = toolStripColor;
            transparencyTexture = new TextureBrush(transparencyBitmap);
            imageViewer.MouseWheel += imageViewer_MouseWheel;

            RestrictUIStartup();

            openDialog = new OpenFileDialog
            {
                Filter = filter,
                CheckFileExists = true,
                CheckPathExists = true
            };

            saveDialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = filter1,
                CheckPathExists = true
            };
        }

        private void SPFWindow_Resize(object sender, EventArgs e)
        {
            imageViewer.Width = ClientSize.Width;
            imageViewer.Height = ClientSize.Height - imageViewer.Top;
            imageViewer.Invalidate();
            toolStrip1.Width = ClientSize.Width;
        }

        private void SPFWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Home)
            {
                ScaleUpdate(1.0f);
            }

            if (e.KeyCode == Keys.F11)
            {
                if (fullScreen == false)
                {
                    settings.prevBound = Bounds;
                    settings.prevWindowState = WindowState;

                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = FormBorderStyle.None;
                    Bounds = Screen.PrimaryScreen.Bounds;

                    fullScreen = !fullScreen;
                }
                else
                {
                    WindowState = settings.prevWindowState;
                    FormBorderStyle = FormBorderStyle.Sizable;

                    Bounds = settings.prevBound;

                    fullScreen = !fullScreen;
                }
            }

            if (e.KeyCode == Keys.F12)
            {
                RenderSPF();
            }
        }

        private void SPFWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (!selfDragging) e.Effect = DragDropEffects.Move;
        }

        private void SPFWindow_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                pathToFile = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                Browse();
            }
        }

        //

        private void imageViewer_Paint(object sender, PaintEventArgs e)
        {
            if (settings.bit != null)
            {
                float left = (imageViewer.ClientSize.Width / 2) - ((settings.bit.Width * scale) / 2);
                float top = toolStrip1.Height + ((imageViewer.ClientSize.Height - toolStrip1.Height) / 2) - ((settings.bit.Height * scale) / 2);

                rect = new RectangleF(left, top, (settings.bit.Width - 1) * scale, (settings.bit.Height - 1) * scale);

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.Clear(imageViewer.BackColor);

                e.Graphics.FillRectangle(transparencyTexture, rect);

                e.Graphics.TranslateTransform(left, top);
                e.Graphics.ScaleTransform(scale, scale);
                e.Graphics.DrawImage(settings.bit, 0, 0);

                e.Graphics.ResetTransform();

                string scaleString = (Math.Floor(scale * 100)).ToString() + "%";

                e.Graphics.DrawString(scaleString, scaleFont, Brushes.White, 6, toolStrip1.Height + 6);
            }
            
        }

        private void imageViewer_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
               ScaleUpdate(Math.Min(5.0f, Math.Max(0.1f, scale + (e.Delta / 120) * 0.05f)));
        }

        private void imageViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if ((settings.bit != null) && (PointInRect(e.Location, rect)))
            {
                draggingStartPoint = e.Location;
                startDragging = true;
            }
        }

        private void imageViewer_MouseUp(object sender, MouseEventArgs e)
        {
            startDragging = false;
        }

        private void imageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            draggingPoint = e.Location;

            if (startDragging && PointDistance(draggingPoint, draggingStartPoint) > 100 * 100)
            {
                selfDragging = true;

                string name = Path.GetFileNameWithoutExtension(filePath.Text);
                string newName = name + ".spf";
                string newDir = @"C:\temp\" + newName;

                string[] path = new string[1] { newDir };

                DragDropEffects effect = DoDragDrop(new DataObject(DataFormats.FileDrop, path), DragDropEffects.Move);

                if (effect == DragDropEffects.Move)
                {
                    settings.spf.Save(newDir);
                    startDragging = false;
                    selfDragging = false;
                }

                if (effect == DragDropEffects.None)
                {
                    startDragging = false;
                    selfDragging = false;
                }
            }
        }

 
        //

        private void renderButton_Click(object sender, EventArgs e)
        {
            RenderSPF();
        }

        private void filePath_KeyUp(object sender, KeyEventArgs e)
        {
            pathToFile = ((ToolStripTextBox)sender).Text;
            if (e.KeyCode == Keys.Enter) Browse();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                pathToFile = openDialog.FileName;
                Browse();
            }
        }

        private void convertToSpfButton_Click(object sender, EventArgs e)
        {
            saveDialog.Filter = filter2;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                SPFFile spfFile = new SPFFile(settings.bit);
                spfFile.Save(saveDialog.FileName);
                imageViewer.Invalidate();
            }
        }



        private void saveImageButton_Click(object sender, EventArgs e)
        {
            string name = Path.GetFileNameWithoutExtension(filePath.Text);

            saveDialog.Filter = filter1;
            saveDialog.FileName = name;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                settings.bit.Save(saveDialog.FileName);
            }
        }
    }
}