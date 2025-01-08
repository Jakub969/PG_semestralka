using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cv1
{
    public class GaussFilter
    {
        private double[,] kernel;
        private int kernelSize;
        public GaussFilter(int size, double sigma)
        {
            kernelSize = size;
            kernel = new double[size, size];
            double sum = 0;
            int halfSize = size / 2;
            for (int i = -halfSize; i <= halfSize; i++)
            {
                for (int j = -halfSize; j <= halfSize; j++)
                {
                    kernel[i + halfSize, j + halfSize] = Math.Exp(-(i * i + j * j) / (2 * sigma * sigma));
                    sum += kernel[i + halfSize, j + halfSize];
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= sum;
                }
            }
        }
        public double[,] Kernel
        {
            get { return kernel; }
        }
        public int KernelSize
        {
            get { return kernelSize; }
        }

        Bitmap ApplyGaussianFilter(Bitmap image, double[,] kernel)
        {
            int width = image.Width;
            int height = image.Height;
            int kernelSize = kernel.GetLength(0);
            int offset = kernelSize / 2;

            Bitmap filtered = new Bitmap(width, height);

            for (int x = offset; x < width - offset; x++)
            {
                for (int y = offset; y < height - offset; y++)
                {
                    double sum = 0;

                    for (int i = 0; i < kernelSize; i++)
                    {
                        for (int j = 0; j < kernelSize; j++)
                        {
                            int pixelX = x + i - offset;
                            int pixelY = y + j - offset;

                            Color pixel = image.GetPixel(pixelX, pixelY);
                            sum += kernel[i, j] * pixel.R; // R = G = B pre grayscale
                        }
                    }

                    byte gray = (byte)Math.Min(Math.Max(sum, 0), 255);
                    filtered.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return filtered;
        }

    }
}
