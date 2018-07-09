using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace SPF
{
    public class SPFFile
    {
        //header

        public char[] signature;
        public int width, height;
        public int stripCount;
        public SPFStrip[] strips;

        //cache

        private Bitmap original;
        private Bitmap normalized;
        private Bitmap colorized;
        private Bitmap colorizedNormalized;

        //constructors

        public SPFFile() {}

        public SPFFile(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            signature = br.ReadChars(2);

            width = br.ReadInt32();
            height = br.ReadInt32();

            stripCount = br.ReadInt32();

            strips = new SPFStrip[stripCount];

            for (int i = 0; i < stripCount; i++)
            {
                int length;
                byte r, g, b, a;

                length = br.ReadInt32();

                r = br.ReadByte();
                g = br.ReadByte();
                b = br.ReadByte();
                a = br.ReadByte();

                strips[i] = new SPFStrip(length, Color.FromArgb(a, r, g, b));
            }

            br.Close();
        }

        public static SPFFile FromFile(string pathToFile)
        {
            FileStream fs = new FileStream(pathToFile, FileMode.Open);

            SPFFile spfFile = new SPFFile(fs);
            return spfFile;
        }

        public SPFFile(Bitmap bit)
        {
            Color color;
            List<SPFFile.SPFStrip> strips = new List<SPFFile.SPFStrip>();

            int x = 0;
            int xx = 0, yy = 0;

            int stripCount = 0;
            int length = 1;

            int imageByteNumber = bit.Width * bit.Height;

            signature = new char[] { 'S', 'P' };

            width = bit.Width; // width
            height = bit.Height; // height


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

                /*
                if (length == 1)
                {
                    length = -1;
                    long curOff = bw.BaseStream.Position, lastOff;
                    bw.Write(length);

                    x++;

                    xx = x % bit.Width;
                    yy = x / bit.Width;

                    Color col = bit.GetPixel(xx, yy);

                    while ((x < imageByteNumber) && (color != col))
                    {
                        bw.Write(color.R);
                        bw.Write(color.G);
                        bw.Write(color.B);
                        bw.Write(color.A);

                        color = col;

                        length--;

                        x++;

                        xx = Math.Min(x % bit.Width, bit.Width-1);
                        yy = Math.Min(x / bit.Width, bit.Height-1);

                        col = bit.GetPixel(xx, yy);
                    }

                    lastOff = bw.BaseStream.Position;

                    bw.BaseStream.Seek(curOff, SeekOrigin.Begin);
                    bw.Write(length);
                    bw.BaseStream.Seek(lastOff, SeekOrigin.Begin);

                }
                */

                strips.Add(new SPFFile.SPFStrip(length, color));

                stripCount++;

                length = 1;
            }

            this.stripCount = stripCount;
            this.strips = strips.ToArray();
        }

        //methods

        public Bitmap ToBitmap(bool colorize = false, bool normalize = false)
        {
            Bitmap bit = new Bitmap(width, height);
            Color color;
            Random rand = new Random();
            int x = 0, xx, yy, length;

            for (int i = 0; i < stripCount; i++)
            {
                length = (normalize ? 1 : strips[i].length);
                color = (colorize ? Color.FromArgb(strips[i].color.A, rand.Next(255), rand.Next(255), rand.Next(255)) : strips[i].color);
                for (int j = 0; j < length; j++)
                {
                    xx = x % width;
                    yy = x / width;

                    bit.SetPixel(xx, yy, color);

                    x++;
                }
            }

            return bit;
        }

        public Bitmap GetImage(bool colorize = false, bool normalize = false)
        {
            Bitmap bit = null;
            if (colorize && colorized == null && !normalize) colorized = ToBitmap(true, false);
            if (normalize && normalized == null && !colorize) normalized = ToBitmap(false, true);
            if (colorize && normalize && colorizedNormalized == null) colorizedNormalized = ToBitmap(true, true);
            if (!colorize && !normalize && original == null) original = ToBitmap();

            if (colorize) bit = colorized;
            if (normalize) bit = normalized;
            if (colorize && normalize) bit = colorizedNormalized;
            if (!colorize && !normalize) bit = original;

            return bit;
        }

        public void Save(string pathToFile)
        {
            FileStream fs = new FileStream(pathToFile, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(signature);
            bw.Write(width);
            bw.Write(height);
            bw.Write(stripCount);

            for (int i = 0; i < stripCount; i++)
            {
                bw.Write(strips[i].length);
                bw.Write(strips[i].color.R);
                bw.Write(strips[i].color.G);
                bw.Write(strips[i].color.B);
                bw.Write(strips[i].color.A);
            }

            bw.Close();
        }

        // strip structure

        public class SPFStrip
        {
            public int length;
            public Color color;

            public SPFStrip(int length, Color color)
            {
                this.length = length;
                this.color = color;
            }
        }
    }
}
