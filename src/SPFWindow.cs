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
        Bitmap bit;

        Color toolStripColor = Color.DimGray;
        Color imageViewerColor = Color.FromArgb(85, 85, 85);
        Bitmap transparencyBitmap = new Bitmap(@"img\transparency.png");
        TextureBrush transparencyTexture;

        RectangleF rect;

        string pathToFile;

        //

        float scale = 1;
        Font scaleFont = new Font("Arial", 16);

        //

        string filter = "Strip format (*.spf)|*.spf|Portable network graphics (*.png)|*.png|Joint Photographic Experts Group (*.jpg, *.jpeg)|*.jpg;*.jpeg|Bitmap (*.bmp)|*.bmp|All files (*.*)|*.*";
        string filter1 = "Portable network graphics (*.png)|*.png|Joint Photographic Experts Group (*.jpg, *.jpeg)|*.jpg;*.jpeg|Bitmap (*.bmp)|*.bmp|All files (*.*)|*.*";
        string filter2 = "Strip format (*.spf)|*.spf";

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

        private void LoadSPF()
        {
            SPFInfo spfInfo = SPF.ReadSPF(pathToFile, toolStripCButton.Checked, toolStripNButton.Checked);
            if (spfInfo == null)
            {
                MessageBox.Show("This file doesn't exists ");
                return;
            }

            bit = spfInfo.bit;

            RestrictUI();

            imageViewer.Invalidate();
            stripCountLabel.Text = "Strip count: " + spfInfo.stripCount.ToString();
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
                bit = new Bitmap(pathToFile);
                bit.SetResolution(96, 96);

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
        }

        private void SPFWindow_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
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
            if (bit != null)
            {
                float left = (imageViewer.ClientSize.Width / 2) - ((bit.Width * scale) / 2);
                float top = toolStrip1.Height + ((imageViewer.ClientSize.Height - toolStrip1.Height) / 2) - ((bit.Height * scale) / 2);

                rect = new RectangleF(left, top, (bit.Width - 1) * scale, (bit.Height - 1) * scale);

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.Clear(imageViewer.BackColor);

                e.Graphics.FillRectangle(transparencyTexture, rect);

                e.Graphics.TranslateTransform(left, top);
                e.Graphics.ScaleTransform(scale, scale);
                e.Graphics.DrawImage(bit, 0, 0);

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
            if (bit != null) if (PointInRect(e.Location, rect)) DoDragDrop(new DataObject(DataFormats.Bitmap, bit), DragDropEffects.Copy);
        }

        //

        private void renderButton_Click(object sender, EventArgs e)
        {
            LoadSPF();
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
                SPF.ConvertToSPF(saveDialog.FileName, bit);
                imageViewer.Invalidate();
            }
        }

        private void saveImageButton_Click(object sender, EventArgs e)
        {
            saveDialog.Filter = filter1;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                bit.Save(saveDialog.FileName);
            }
        }
    }
}