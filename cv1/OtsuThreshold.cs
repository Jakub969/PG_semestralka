using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cv1
{
    public class OtsuThreshold
    {
        private int[] histogram;
        private int totalPixels;
        private double[] probabilities;
        private double[] mean;
        private double globalMean;
        private double[] variance;
        private double[] betweenClassVariance;
        private double[] withinClassVariance;
        private double[] threshold;


        public OtsuThreshold(int[] histogram)
        {
            this.histogram = histogram;
            totalPixels = histogram.Sum();
            probabilities = new double[256];
            mean = new double[256];
            variance = new double[256];
            betweenClassVariance = new double[256];
            withinClassVariance = new double[256];
            threshold = new double[256];
        }

        int CalculateOtsuThreshold()
        {
            double sum = 0;
            for (int i = 0; i < 256; i++) sum += i * histogram[i];

            double sumB = 0, wB = 0, wF = 0, maxVariance = 0;
            int threshold = 0;

            for (int i = 0; i < 256; i++)
            {
                wB += histogram[i];
                if (wB == 0) continue;

                wF = totalPixels - wB;
                if (wF == 0) break;

                sumB += i * histogram[i];

                double mB = sumB / wB;
                double mF = (sum - sumB) / wF;

                double variance = wB * wF * Math.Pow(mB - mF, 2);
                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    threshold = i;
                }
            }

            return threshold;
        }

        Bitmap ApplyThreshold(Bitmap image, int threshold)
        {
            Bitmap binary = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    byte value = pixel.R > threshold ? (byte)255 : (byte)0;
                    binary.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }

            return binary;
        }
    }
}
