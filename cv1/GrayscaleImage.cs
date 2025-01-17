using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cv1
{
    public class GrayscaleImage
    {
        public int Width { get; }
        public int Height { get; }
        public byte[,] Data { get; }
        public int[] Histogram { get; }

        public GrayscaleImage(string filePath, int width, int height)
        {
            Width = width;
            Height = height;
            Data = new byte[Height, Width];
            Histogram = new int[256];

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Read the Y component (luminance)
                for (int y = 0; y < Height; y++)
                {
                    int offset = y * Width;
                    byte[] row = new byte[Width];
                    int bytesRead = fs.Read(row, 0, Width);

                    if (bytesRead < Width)
                    {
                        throw new Exception("Unexpected end of file while reading luminance data.");
                    }

                    for (int x = 0; x < Width; x++)
                    {
                        byte pixelValue = row[x];
                        Data[y, x] = pixelValue;
                        Histogram[pixelValue]++;
                    }
                }

                // Skip over the U and V components
                long uvSize = (long)(Width * Height / 2);
                fs.Seek(uvSize, SeekOrigin.Current);
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);

            // Use grayscale palette
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            int stride = bmpData.Stride;
            IntPtr ptr = bmpData.Scan0;
            byte[] pixels = new byte[stride * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    pixels[y * stride + x] = Data[y, x];
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, pixels.Length);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
    }

}
