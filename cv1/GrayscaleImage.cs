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
        public Bitmap ApplyThreshold(int threshold)
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format1bppIndexed);

            int stride = bmpData.Stride;
            IntPtr ptr = bmpData.Scan0;
            int bytes = stride * Height;
            byte[] pixels = new byte[bytes];

            for (int y = 0; y < Height; y++)
            {
                int rowOffset = y * stride;
                for (int x = 0; x < Width; x++)
                {
                    // Check if the pixel value is below the threshold
                    if (Data[y, x] >= threshold)
                    {
                        int byteIndex = rowOffset + (x / 8);
                        int bitIndex = 7 - (x % 8);
                        pixels[byteIndex] |= (byte)(1 << bitIndex);
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, bytes);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
        public PointF? GetLineCenter(int threshold)
        {
            long sumX = 0;
            long sumY = 0;
            int count = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Check if the pixel is part of the line
                    if (Data[y, x] < threshold)
                    {
                        sumX += x;
                        sumY += y;
                        count++;
                    }
                }
            }

            if (count == 0)
                return null;

            float centerX = (float)sumX / count;
            float centerY = (float)sumY / count;

            return new PointF(centerX, centerY);
        }

    }

}
