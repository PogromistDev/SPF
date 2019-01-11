using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SPF
{
    public class SPFFile
    {
        // header

        public char[] signature;
        public int width, height;
        public int stripCount;
        public SPFStrip[] strips;

        // cache

        private Bitmap original;
        private Bitmap normalized;
        private Bitmap colorized;
        private Bitmap colorizedNormalized;

        // strip structure

        public class SPFStrip
        {
            public int length;
            public Color[] color;

            public SPFStrip(int length, Color[] color)
            {
                this.length = length;
                this.color = color;
            }
        }

        // constructors

        public SPFFile() {}

        // read spf from stream
        public static SPFFile FromStream(Stream stream)
        {
            SPFFile spfFile = new SPFFile();

            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, (int)stream.Length);
            stream.Close();

            BinaryReader br = new BinaryReader(new MemoryStream(buffer));

            spfFile.signature = br.ReadChars(2);

            spfFile.width = br.ReadInt32();
            spfFile.height = br.ReadInt32();

            spfFile.stripCount = br.ReadInt32();

            spfFile.strips = new SPFStrip[spfFile.stripCount];

            for (int i = 0; i < spfFile.stripCount; i++)
            {
                int length;
                byte r, g, b, a;

                length = br.ReadInt32();

                if (length < 0)
                {
                    List<Color> rawColors = new List<Color>();

                    for (int j = 0; j < Math.Abs(length); j++)
                    {
                        r = br.ReadByte();
                        g = br.ReadByte();
                        b = br.ReadByte();
                        a = br.ReadByte();

                        rawColors.Add(Color.FromArgb(a, r, g, b));
                    }

                    spfFile.strips[i] = new SPFStrip(length, rawColors.ToArray());
                }
                else
                {
                    r = br.ReadByte();
                    g = br.ReadByte();
                    b = br.ReadByte();
                    a = br.ReadByte();

                    spfFile.strips[i] = new SPFStrip(length, new Color[] { Color.FromArgb(a, r, g, b) });
                }
                
            }

            br.Close();

            return spfFile;
        }

        // read spf from file
        public static SPFFile FromFile(string pathToFile)
        {
            FileStream fs = new FileStream(pathToFile, FileMode.Open);

            SPFFile spfFile = FromStream(fs);
            return spfFile;
        }

        // convert bitmap to spf
        public static SPFFile FromBitmap(Bitmap bit)
        {
            SPFFile spfFile = new SPFFile();

            Color color;
            Color nextColor = Color.White;

            List<Color> rawColors = new List<Color>();

            List<SPFStrip> strips = new List<SPFStrip>();

            int offset = 0; // pixel offset
            int xx = 0, yy = 0; // pixel 2D coordinate

            int length = 1; // strip length

            int imagePixelNumber = bit.Width * bit.Height; // number of pixels

            //

            spfFile.signature = new char[] { 'S', 'P' };

            spfFile.width = bit.Width; // width
            spfFile.height = bit.Height; // height

            int stripCount = 0;

            // algorithm for finding pixels with same color for forming strips

            while (offset < imagePixelNumber)
            {
                color = bit.GetPixel(xx, yy);

                // next color

                offset++;

                OffsetToCoordinates(ref xx, ref yy, offset, bit.Width);

                // while offset less that imagePixelNumber and previous pixel color equals to current pixel color
                while ((offset < imagePixelNumber) && (color == (nextColor = bit.GetPixel(xx, yy))) )
                {
                    offset++;

                    length++;

                    OffsetToCoordinates(ref xx, ref yy, offset, bit.Width);
                }

                if (length == 1)
                {
                    rawColors.Add(color);
                    
                    length = -1;

                    // next color

                    Color col = Color.White;

                    while ((offset < imagePixelNumber) && (color != (col = bit.GetPixel(xx, yy))) )
                    {
                        rawColors.Add(col);
                        color = col;

                        length--;

                        offset++;

                        OffsetToCoordinates(ref xx, ref yy, offset, bit.Width);
                    }

                    strips.Add(new SPFStrip(length, rawColors.ToArray()));
                    rawColors.Clear();

                }
                else
                {
                    strips.Add(new SPFStrip(length, new Color[] { color }));
                }

                stripCount++;

                length = 1; // length to default
            }

            spfFile.stripCount = stripCount;
            spfFile.strips = strips.ToArray();
            spfFile.original = bit;

            return spfFile;
        }

        // methods

        // convert spf to bitmap
        public Bitmap ToBitmap(bool colorize = false, bool normalize = false, bool colorizeRawStripAsOneColor = false)
        {
            Bitmap bit = new Bitmap(width, height);
            Color color = Color.Black;
            Random rand = new Random();
            int x = 0, xx = 0, yy = 0, length;

            for (int i = 0; i < stripCount; i++)
            {
                length = strips[i].length;

                if (length < 0)
                {
                    // if colorize is true then randomize color else use color from file

                    if (colorizeRawStripAsOneColor)
                    {
                        if (colorize)
                        {
                            color = Color.FromArgb(strips[i].color[0].A, rand.Next(255), rand.Next(255), rand.Next(255));
                        }
                    }

                    for (int j = 0; j < Math.Abs(length); j++)
                    {
                        if (!colorizeRawStripAsOneColor)
                        {
                            if (colorize)
                            {
                                color = Color.FromArgb(strips[i].color[0].A, rand.Next(255), rand.Next(255), rand.Next(255));
                            }
                            else
                            {
                                color = strips[i].color[j];
                            }
                        }

                        OffsetToCoordinates(ref xx, ref yy, x, width);

                        bit.SetPixel(xx, yy, color);

                        x++;
                    }
                }
                else
                {
                    // if normalize is true then make length of strip to one else use length from file

                    length = (normalize ? 1 : length);

                    // if colorize is true then randomize color else use color from file

                    color = (colorize ? Color.FromArgb(strips[i].color[0].A, rand.Next(255), rand.Next(255), rand.Next(255)) : strips[i].color[0]);

                    for (int j = 0; j < length; j++)
                    {
                        OffsetToCoordinates(ref xx, ref yy, x, width);

                        bit.SetPixel(xx, yy, color);

                        x++;
                    }
                }
                
            }

            return bit;
        }

        // fetch image from cache
        public Bitmap GetImage(bool colorize = false, bool normalize = false, bool colorizeRawStripAsOneColor = false)
        {
            Bitmap bit = null;

            CacheImage(colorize, normalize, colorizeRawStripAsOneColor);

            // returning cached image

            if (colorize && normalize)
            {
                bit = colorizedNormalized;
                return bit;
            }
            
            if (!colorize && !normalize)
            {
                bit = original;
                return bit;
            }
 
            if (colorize)
            {
                bit = colorized;
                return bit;
            }          

            if (normalize)
            {
                bit = normalized;
            }

            return bit;
        }

        // cache image
        public void CacheImage(bool colorize = false, bool normalize = false, bool colorizeRawStripAsOneColor = false)
        {
            if (colorize && colorized == null && !normalize)
            {
                colorized = ToBitmap(true, false, colorizeRawStripAsOneColor);
                return;
            }

            if (normalize && normalized == null && !colorize)
            {
                normalized = ToBitmap(false, true, colorizeRawStripAsOneColor);
                return;
            }

            if (colorize && normalize && colorizedNormalized == null)
            {
                colorizedNormalized = ToBitmap(true, true, colorizeRawStripAsOneColor);
                return;
            }

            if (!colorize && !normalize && original == null)
            {
                original = ToBitmap();
                return;
            }
        }

        // clear cache
        public void ClearCache()
        {
            colorized = null;
            normalized = null;
            colorizedNormalized = null;
        }

        // save spf to file
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
                if (strips[i].length < 0)
                {
                    for (int j = 0; j < Math.Abs(strips[i].length); j++)
                    {
                        bw.Write(strips[i].color[j].R);
                        bw.Write(strips[i].color[j].G);
                        bw.Write(strips[i].color[j].B);
                        bw.Write(strips[i].color[j].A);
                    }
                }
                else
                {
                    bw.Write(strips[i].color[0].R);
                    bw.Write(strips[i].color[0].G);
                    bw.Write(strips[i].color[0].B);
                    bw.Write(strips[i].color[0].A);
                }
            }

            bw.Close();
        }

        private static void OffsetToCoordinates(ref int xx, ref int yy, int offset, int width)
        {
            xx = offset % width;
            yy = offset / width;
        }

    }
}
