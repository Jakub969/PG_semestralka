using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cv1
{
    public class GrayscaleImage
    {
        private byte[] data;
        private int width; 
        private int height;
        private int[] histogram = new int[256];

        public int Width { get { return width; } }
        public int Height { get { return height; } }  
        public byte[] Data { get { return data; } }

        public int[] Histogram { get { return histogram; } }

        public GrayscaleImage(byte[] parData, int parWidth, int parHeight)
        {
            ArgumentNullException.ThrowIfNull(parData);

            if (parWidth < 16 || parHeight < 16 || parWidth > 1024 || parHeight > 1024)
                throw new ArgumentException("Grayscale image is out of size");

            data = parData;
            width = parWidth;   
            height = parHeight; 

            RecalculateHistogram();
        }

        public GrayscaleImage(int parWidth, int parHeight)
        {
            if (parWidth < 16 || parHeight < 16 || parWidth > 1024 || parHeight > 1024)
                throw new ArgumentException("Grayscale image is out of size");

            width = parWidth;
            height = parHeight;
            data = new byte[width * height];
        }

        public void RecalculateHistogram()
        {
            histogram = new int[256];

            // naplnenie histogramu
            foreach (byte b in data)
                histogram[b] += 1;
        }

        public Bitmap ToBitmap()
        {
            Bitmap resultBitmap = new (width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte color = data[x + width * y];
                    resultBitmap.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }

            return resultBitmap;
        }
    }
}
