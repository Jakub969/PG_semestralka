﻿using System;
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
        public Bitmap ApplyThreshold(byte[,] inputData, int threshold)
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
                    // Check if the pixel value is above the threshold
                    if (inputData[y, x] >= threshold)
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
        public int CalculateOtsuThreshold(byte[,] data)
        {
            int[] histogram = new int[256];
            int width = data.GetLength(1);
            int height = data.GetLength(0);

            // Calculate histogram
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    histogram[data[y, x]]++;
                }
            }

            int total = width * height;
            float sum = 0;
            for (int i = 0; i < 256; i++)
            {
                sum += i * histogram[i];
            }

            float sumB = 0;
            int wB = 0;
            int wF = 0;
            float varMax = 0;
            int threshold = 0;

            for (int t = 0; t < 256; t++)
            {
                wB += histogram[t];
                if (wB == 0) continue;
                wF = total - wB;
                if (wF == 0) break;

                sumB += t * histogram[t];
                float mB = sumB / wB;
                float mF = (sum - sumB) / wF;

                float varBetween = wB * wF * (mB - mF) * (mB - mF);
                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    threshold = t;
                }
            }

            return threshold;
        }


        public PointF? GetLineCenter(byte[,] inputData, int threshold)
        {
            long sumX = 0;
            long sumY = 0;
            int count = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Check if the pixel is below the threshold
                    if (inputData[y, x] < threshold)
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

        private double[,] GenerateGaussianKernel(int size, double sigma)
        {
            double[,] kernel = new double[size, size];
            int radius = size / 2;
            double sigma2 = sigma * sigma;
            double sum = 0;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double exponent = -(x * x + y * y) / (2 * sigma2);
                    double value = Math.Exp(exponent);
                    kernel[y + radius, x + radius] = value;
                    sum += value;
                }
            }

            // Normalize the kernel
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    kernel[y, x] /= sum;
                }
            }

            return kernel;
        }
        private byte[,] Convolve(byte[,] data, double[,] kernel)
        {
            int width = data.GetLength(1);
            int height = data.GetLength(0);
            int kernelSize = kernel.GetLength(0);
            int kernelRadius = kernelSize / 2;
            byte[,] result = new byte[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0;

                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                        {
                            int posY = y + ky;
                            int posX = x + kx;

                            if (posY >= 0 && posY < height && posX >= 0 && posX < width)
                            {
                                sum += data[posY, posX] * kernel[ky + kernelRadius, kx + kernelRadius];
                            }
                        }
                    }

                    result[y, x] = (byte)Math.Clamp(sum, 0, 255);
                }
            }

            return result;
        }
        public byte[,] ApplyGaussianHighPassFilter()
        {
            // Generate a Gaussian kernel (e.g., size 5, sigma 1.0)
            double[,] gaussianKernel = GenerateGaussianKernel(7, 2.0);

            // Apply Gaussian blur to get the low-pass (blurred) image
            byte[,] lowPassData = Convolve(Data, gaussianKernel);

            // Subtract the low-pass image from the original to get high-pass data
            int width = Data.GetLength(1);
            int height = Data.GetLength(0);
            byte[,] highPassData = new byte[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = Data[y, x] - lowPassData[y, x];
                    // Adjust the value to ensure it's within the byte range
                    value = Math.Clamp(value + 128, 0, 255);
                    highPassData[y, x] = (byte)value;
                }
            }

            return highPassData;
        }

        public byte[,] ApplyCombinedFilter()
        {
            // Generate a Gaussian kernel (e.g., size 7, sigma 2.0)
            double[,] gaussianKernel = GenerateGaussianKernel(7, 2.0);

            // Apply Gaussian blur to get the low-pass (blurred) image
            byte[,] lowPassData = Convolve(Data, gaussianKernel);

            // Subtract the low-pass image from the original to get high-pass data
            int width = Data.GetLength(1);
            int height = Data.GetLength(0);
            byte[,] highPassData = new byte[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = Data[y, x] - lowPassData[y, x];
                    // Adjust the value to ensure it's within the byte range
                    value = Math.Clamp(value + 128, 0, 255);
                    highPassData[y, x] = (byte)value;
                }
            }

            // Apply median filter to reduce noise
            byte[,] filteredData = ApplyMedianFilter(highPassData, 3);

            // Combine the original image with the high-pass filtered image
            byte[,] combinedData = new byte[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    combinedData[y, x] = (byte)Math.Clamp((Data[y, x] + filteredData[y, x]) / 2, 0, 255);
                }
            }

            return combinedData;
        }

        private byte[,] ApplyMedianFilter(byte[,] data, int size)
        {
            int width = data.GetLength(1);
            int height = data.GetLength(0);
            byte[,] result = new byte[height, width];
            int radius = size / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    List<byte> neighborhood = new List<byte>();

                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int posY = y + ky;
                            int posX = x + kx;

                            if (posY >= 0 && posY < height && posX >= 0 && posX < width)
                            {
                                neighborhood.Add(data[posY, posX]);
                            }
                        }
                    }

                    neighborhood.Sort();
                    result[y, x] = neighborhood[neighborhood.Count / 2];
                }
            }

            return result;
        }

        public List<Point> ExtractCurvePoints(int threshold)
        {
            List<Point> curvePoints = new();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Data[y, x] < threshold)
                    {
                        curvePoints.Add(new Point(x, y));
                    }
                }
            }

            return curvePoints;
        }
    }
}
