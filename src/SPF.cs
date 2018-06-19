using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace SPF
{
    public class SPFInfo
    {
        public Bitmap bit;
        public int stripCount;

        public SPFInfo(Bitmap bit, int stripCount)
        {
            this.bit = bit;
            this.stripCount = stripCount;
        }
    }

    public static class SPF
    {
        public static SPFInfo ReadSPF(string pathToFile, bool colorize = false, bool normalize = false)
        {
            try
            {
                FileStream fs = new FileStream(pathToFile, FileMode.Open);
                BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

                Color color;
                int width, height, stripCount, length, off = 0, x = 0, xx = 0, yy = 0;
                byte r, g, b, a = 255;

                br.BaseStream.Seek(2, SeekOrigin.Begin);

                //Reading image properties

                width = br.ReadInt32();
                height = br.ReadInt32();

                stripCount = br.ReadInt32();

                //

                Bitmap bit = new Bitmap(width, height);
                SPFInfo spfInfo = new SPFInfo(bit, stripCount);

                Random rand = new Random();

                for (int y = 0; y < stripCount; y++)
                {
                    if (normalize)
                    {
                        length = 1;
                        br.BaseStream.Seek(4, SeekOrigin.Current);
                    }
                    else
                    {
                        length = br.ReadInt32();
                    }

                    //Reading pixel's color components

                    r = br.ReadByte();
                    g = br.ReadByte();
                    b = br.ReadByte();
                    a = br.ReadByte();

                    if (colorize)
                    {
                        r = (byte)rand.Next(255);
                        g = (byte)rand.Next(255);
                        b = (byte)rand.Next(255);
                    }

                    color = Color.FromArgb(a, r, g, b);

                    for (; x < off + length; x++)
                    {
                        xx = x % width;
                        yy = x / width;

                        bit.SetPixel(xx, yy, color);
                    }

                    off = x;
                }

                br.Close();
                return spfInfo;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public static void ConvertToSPF(string pathToFile, Bitmap bit)
        {
            FileStream fs = new FileStream(pathToFile, FileMode.Create);
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
                bw.Write(color.A);

                stripCount++;

                length = 1;
            }

            bw.BaseStream.Seek(10, SeekOrigin.Begin);
            bw.Write(stripCount);

            bw.Close();
        }
    }
}
