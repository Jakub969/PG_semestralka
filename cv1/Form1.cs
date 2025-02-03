using System.Data;
using MathNet.Numerics.LinearAlgebra;
using MathNetVector = MathNet.Numerics.LinearAlgebra.Vector<double>;
using System.Diagnostics;


namespace cv1
{
    public partial class Form1 : Form
    {
        private GrayscaleImage? originalImage;
        private int imageWidth;
        private int imageHeight;
        private int thresholdValue;
        private bool changeThreshold = false;
        
        private List<Point> simplifiedCurvePoints = new();

        // Prahova hodnota pre Ramer-Douglas-Peucker algoritmus
        private const double RDP_EPSILON = 5.0;

        private Stopwatch stopwatchTotal = new();
        private Stopwatch stopwatchFiltering = new();
        private Stopwatch stopwatchThresholding = new();
        private Stopwatch stopwatchRDP = new();
        private Stopwatch stopwatchBezierFitting = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void doubleBufferPanelDrawing_Paint(object sender, PaintEventArgs e)
        {
            stopwatchTotal.Restart();

            Graphics g = e.Graphics;

            if (originalImage != null)
            {
                
                byte[,] combinedData = originalImage.Data;

                if (gaussFilter.Checked)
                {
                    stopwatchFiltering.Restart();
                    combinedData = originalImage.ApplyCombinedFilter();
                    stopwatchFiltering.Stop();
                }

                if (!changeThreshold)
                {
                    // Vypocet prahovej hodnoty pomocou Otsu metody
                    stopwatchThresholding.Restart();
                    int otsuThreshold = originalImage.CalculateOtsuThreshold(combinedData);
                    stopwatchThresholding.Stop();
                    thresholdValue = otsuThreshold;
                    changeThreshold = true;
                }

                numericUpDownThreshold.Value = thresholdValue;

                // Aplikácia prahovania na obrazok
                stopwatchThresholding.Start();
                Bitmap bitmap = originalImage.ApplyThreshold(combinedData, thresholdValue);
                stopwatchThresholding.Stop();
                g.DrawImage(bitmap, new Point(0, 0));

                // Zistenie bodov krivky
                List<Point> curvePoints = originalImage.ExtractCurvePoints(thresholdValue);

                // Zjednodusenie krivky pomocou Ramer-Douglas-Peucker algoritmu
                stopwatchRDP.Restart();
                simplifiedCurvePoints = GeometryUtils.RamerDouglasPeucker(curvePoints, RDP_EPSILON);
                stopwatchRDP.Stop();

                foreach (var point in simplifiedCurvePoints)
                {
                    g.FillEllipse(Brushes.Green, point.X - 2, point.Y - 2, 4, 4);
                }


                if (simplifiedCurvePoints.Count >= 4)
                {
                    MathNetVector[] vectors = ConvertPoints(simplifiedCurvePoints);

                    try
                    {
                        stopwatchBezierFitting.Restart();
                        MathNetVector[] bezierPoints = FitCubicBezier(vectors, combinedData, thresholdValue);
                        stopwatchBezierFitting.Stop();

                        using (Pen pen = new Pen(Color.Blue, 2))
                        {
                            PointF start = new((float)bezierPoints[0][0], (float)bezierPoints[0][1]);
                            PointF control1 = new((float)bezierPoints[1][0], (float)bezierPoints[1][1]);
                            PointF control2 = new((float)bezierPoints[2][0], (float)bezierPoints[2][1]);
                            PointF end = new((float)bezierPoints[3][0], (float)bezierPoints[3][1]);

                            g.DrawBezier(pen, start, control1, control2, end);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bezier fitting error: {ex.Message}");
                    }
                }
            }
            stopwatchTotal.Stop();
            DisplayProcessingTimes();
        }

        private void DisplayProcessingTimes()
        {
            labelProcessingTime.Text = $"Total: {stopwatchTotal.ElapsedMilliseconds} ms\n" +
                                       $"Filtering: {stopwatchFiltering.ElapsedMilliseconds} ms\n" +
                                       $"Thresholding: {stopwatchThresholding.ElapsedMilliseconds} ms\n" +
                                       $"RDP: {stopwatchRDP.ElapsedMilliseconds} ms\n" +
                                       $"Bezier Fitting: {stopwatchBezierFitting.ElapsedMilliseconds} ms";
        }

        private MathNetVector[] ConvertPoints(List<Point> points)
        {
            return points.Select(p => MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { p.X, p.Y })).ToArray();
        }

        private MathNetVector[] FitCubicBezier(MathNetVector[] points, byte[,] inputData, int threshold)
        {
            int n = points.Length;

            if (n < 4)
            {
                throw new ArgumentException("At least 4 points are required to fit a cubic Bezier curve.");
            }

            var p0 = originalImage.GetStartCenter(inputData, threshold);
            var p3 = originalImage.GetEndCenter(inputData, threshold);

            MathNetVector P0, P3;

            // Ak p0 neexistuje, použijeme points[0]
            if (p0.HasValue)
            {
                P0 = MathNetVector.Build.DenseOfArray(new double[] { p0.Value.X, p0.Value.Y });
            }
            else
            {
                P0 = points[0];
            }

            // Ak p3 neexistuje, použijeme points[n-1]
            if (p3.HasValue)
            {
                P3 = MathNetVector.Build.DenseOfArray(new double[] { p3.Value.X, p3.Value.Y });
            }
            else
            {
                P3 = points[n - 1];
            }

            // Parameter t pre jednotlivé body
            double[] t = new double[n];
            t[0] = 0;

            for (int i = 1; i < n; i++)
            {
                t[i] = t[i - 1] + (points[i] - points[i - 1]).L2Norm();
            }
            for (int i = 1; i < n; i++)
            {
                t[i] /= t[n - 1];
            }

            // Vytvorenie matice A a vektorov Bx, By pre vyriešenie najmenších štvorcov
            var A = Matrix<double>.Build.Dense(n, 2);
            var Bx = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(n);
            var By = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(n);

            for (int i = 0; i < n; i++)
            {
                double u = 1 - t[i];
                double tt = t[i] * t[i];
                double uu = u * u;
                double ttt = tt * t[i];
                double uuu = uu * u;

                //Riešenie P1 a P2
                A[i, 0] = 3 * uu * t[i];
                A[i, 1] = 3 * u * tt;

                Bx[i] = points[i][0] - (uuu * P0[0] + ttt * P3[0]);
                By[i] = points[i][1] - (uuu * P0[1] + ttt * P3[1]);
            }

            // Vyriešenie metódy najmenších štvorcov
            var solutionX = A.QR().Solve(Bx);
            var solutionY = A.QR().Solve(By);

            MathNetVector P1 = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(2);
            MathNetVector P2 = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(2);

            P1[0] = solutionX[0];
            P1[1] = solutionY[0];
            P2[0] = solutionX[1];
            P2[1] = solutionY[1];

            return new MathNetVector[] { P0, P1, P2, P3 };
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string defaultPath = @"C:\\Users\\jakub\\Desktop\\data";
            LoadFilesFromDirectory(defaultPath);
        }

        private void ReloadImage()
        {
            originalImage = null;

            string? selectedString = comboBox1.SelectedValue as string;

            if (string.IsNullOrEmpty(selectedString))
                return;

            try
            {
                originalImage = new GrayscaleImage(selectedString, imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading file!", MessageBoxButtons.OK);
                return;
            }

            doubleBufferPanelDrawing.Invalidate();
            panelHistogram.Invalidate();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadImage();
            changeThreshold = false;
        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            imageWidth = (int)numericUpDownWidth.Value;
            ReloadImage();
        }

        private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            imageHeight = (int)numericUpDownHeight.Value;
            ReloadImage();
        }

        private void panelHistogram_Paint(object sender, PaintEventArgs e)
        {
            if (originalImage == null) return;

            int[] histogram = originalImage.Histogram;

            Graphics g = e.Graphics;
            g.Clear(Color.White);

            int max = histogram.Max();
            float scale = max > 0 ? (float)panelHistogram.Height / max : 1;

            int barWidth = Math.Max(1, panelHistogram.Width / 256);

            for (int i = 0; i < 256; i++)
            {
                int barHeight = (int)(histogram[i] * scale);
                g.FillRectangle(Brushes.Black, i * barWidth, panelHistogram.Height - barHeight, barWidth, barHeight);
            }

            using (Font font = new Font("Arial", 16))
            using (Brush brush = new SolidBrush(Color.Red))
            using (Pen pen = new Pen(Color.Purple))
            {
                for (int i = 0; i <= 255; i += 25)
                {
                    int x = i * barWidth;
                    g.DrawLine(pen, x, panelHistogram.Height, x, panelHistogram.Height - 10);
                    g.DrawString(i.ToString(), font, brush, x, panelHistogram.Height - 30);
                }
            }
        }

        private void setDirectory(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    LoadFilesFromDirectory(selectedPath);
                }
            }
        }
        private void numericUpDownThreshold_ValueChanged(object sender, EventArgs e)
        {
            thresholdValue = (int)numericUpDownThreshold.Value;
            doubleBufferPanelDrawing.Invalidate();
        }


        private void LoadFilesFromDirectory(string selectedPath)
        {
            DataTable table = new();
            table.Columns.Add("File Name");
            table.Columns.Add("File Path");
            if (!Directory.Exists(selectedPath))
                return;

            string[] files = Directory.GetFiles(selectedPath);
            changeThreshold = false;

            foreach (string file in files)
            {
                FileInfo fileInfo = new(file);
                table.Rows.Add(fileInfo.Name, fileInfo.FullName);
            }

            comboBox1.DataSource = table;
            comboBox1.DisplayMember = "File Name";
            comboBox1.ValueMember = "File Path";

            string? selectedString = comboBox1.SelectedValue as string;

            if (string.IsNullOrEmpty(selectedString))
                return;

            imageHeight = (int)numericUpDownHeight.Value;
            imageWidth = (int)numericUpDownWidth.Value;

            ReloadImage();
        }
    }
}
