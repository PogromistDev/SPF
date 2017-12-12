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
    public partial class Form1 : Form
    {
        Bitmap bit;
        Bitmap toolStripTransparencyBitmap;

        Rectangle toolStripRectangle;
        Brush toolStripBrush = new SolidBrush(Color.FromArgb(127, 127, 127, 127));

        string pathToFile;

        float xscale = 1, yscale = 1;
        int x = 0, y = 0;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            //toolStrip1.BackColor = Color.FromArgb(167, 127, 127, 127);

            //pictureBox1.Controls.Add(toolStrip1);
        }

        private void CreateSPF()
        {
            FileStream fs = new FileStream("image.spf", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII);

            bw.Write(new char[] { 'S', 'P'});

            bw.Write(500); // width
            bw.Write(501); // height

            bw.Write(3); // strip count

            bw.Write(83500); // strip length
            bw.Write((byte)0); // r
            bw.Write((byte)0); // g
            bw.Write((byte)255); // b
            bw.Write((byte)0); // filler

            bw.Write(83500); // strip length
            bw.Write((byte)255); // r
            bw.Write((byte)0); // g
            bw.Write((byte)0); // b
            bw.Write((byte)0); // filler

            bw.Write(83500); // strip length
            bw.Write((byte)0); // r
            bw.Write((byte)255); // g
            bw.Write((byte)0); // b
            bw.Write((byte)0); // filler

            bw.Close();
        }

        private void ReadSPF()
        {
            FileStream fs = new FileStream("image.spf", FileMode.Open);
            BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

            br.BaseStream.Seek(2, SeekOrigin.Begin);

            Color color;
            int width, height, stripCount, length, off = 0, x = 0, xx, yy;
            byte r, g, b;

            width = br.ReadInt32();
            height = br.ReadInt32();

            stripCount = br.ReadInt32();

            bit = new Bitmap(width, height);

            Random rand = new Random();

            for (int y = 0; y < stripCount; y++)
            {
                length = (toolStripNButton.Checked == true ? 1: br.ReadInt32());

                r = br.ReadByte();
                g = br.ReadByte();
                b = br.ReadByte();
                br.ReadByte();

                if (toolStripCButton.Checked)
                {
                    r = (byte)rand.Next(255);
                    g = (byte)rand.Next(255);
                    b = (byte)rand.Next(255);
                }

                color = Color.FromArgb(r, g, b);

                for (; x < off + length; x++)
                {
                    xx = x % width;
                    yy = x / width;

                    bit.SetPixel(xx, yy, color);
                }

                off = x;
            }

            br.Close();

            toolStripLabelStripCount.Text = String.Format("Strips number: {0}", stripCount);
        }

        private void ConvertToSPF()
        {
            FileStream fs = new FileStream("image.spf", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.ASCII);

            Color color;
            int x = 0;
            int xx = 0, yy = 0;

            int stripCount = 0;
            int length = 1;

            int imageByteNumber = bit.Width * bit.Height;

            bw.Write(new char[] { 'S', 'P' });

            bw.Write(bit.Width); // width
            bw.Write(bit.Height); // height

            bw.Write(0); // strip count

            while (x < imageByteNumber)
            {
                color = bit.GetPixel(xx, yy);

                x++;

                xx = x % bit.Width;
                yy = x / bit.Width;

                while ((x < imageByteNumber) && (color == bit.GetPixel(xx, yy)))
                {
                    x++;

                    length++;

                    xx = x % bit.Width;
                    yy = x / bit.Width;
                }

                bw.Write(length);
                bw.Write(color.R);
                bw.Write(color.G);
                bw.Write(color.B);
                bw.Write((byte)0);

                stripCount++;

                length = 1;
            }

            bw.BaseStream.Seek(10, SeekOrigin.Begin);
            bw.Write(stripCount);

            bw.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //CreateSPF();
            ReadSPF();

            toolStripRectangle = new Rectangle(0, 0, toolStrip1.Width, toolStrip1.Height);

            toolStripTransparencyBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, toolStrip1.Height);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float left = pictureBox1.ClientSize.Width / 2 - (bit.Width * xscale / 2);
            float top = pictureBox1.ClientSize.Height / 2 - (bit.Height * yscale / 2);

            RectangleF rect = new RectangleF(left, top, bit.Width * xscale, bit.Height * yscale);
            SolidBrush br = new SolidBrush(Color.FromArgb(127, 0, 0, 0));

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.Clear(Color.DimGray);
            e.Graphics.FillRectangle(br, rect.X, rect.Y, rect.Width, rect.Height);
            e.Graphics.DrawImage(bit, rect);
        }

        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            xscale += (e.Delta / 120) * (float)0.05;
            yscale = xscale;
            pictureBox1.Invalidate();
            toolStrip1.Invalidate();
            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = ClientSize.Width;
            pictureBox1.Height = ClientSize.Height - pictureBox1.Top;
            pictureBox1.Invalidate();
            toolStrip1.Width = ClientSize.Width;
            toolStrip1.Invalidate();
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            ConvertToSPF();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pathToFile = filePath.Text;
            ConvertToSPF();
            pictureBox1.Invalidate();
        }

        private void toolStrip1_Paint(object sender, PaintEventArgs e)
        {
            toolStripRectangle.Width = Width;

            pictureBox1.DrawToBitmap(toolStripTransparencyBitmap, toolStripRectangle);

            //

            Bitmap img = new Bitmap(toolStripTransparencyBitmap);
            Bitmap blurPic = new Bitmap(img.Width, img.Height);

            Int32 avgR = 0, avgG = 0, avgB = 0;
            Int32 blurPixelCount = 0;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    avgR += pixel.R;
                    avgG += pixel.G;
                    avgB += pixel.B;

                    blurPixelCount++;
                }
            }

            avgR = avgR / blurPixelCount;
            avgG = avgG / blurPixelCount;
            avgB = avgB / blurPixelCount;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    blurPic.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            e.Graphics.Clear(toolStrip1.BackColor);
            e.Graphics.DrawImage(blurPic, 0, 0);
            e.Graphics.FillRectangle(toolStripBrush, toolStripRectangle);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            bit.Save("transformed.png");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();

            pathToFile = f.FileName;
            filePath.Text = f.FileName;

            bit = new Bitmap(f.FileName);
            pictureBox1.Invalidate();
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            ReadSPF();
            pictureBox1.Invalidate();
        }
    }
}