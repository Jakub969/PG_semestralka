using System.Data;
using System.Drawing.Drawing2D;
using System.Geometry;
using System.IO;

namespace cv1
{
    public partial class Form1 : Form
    {
        private GrayscaleImage? originalImage;
        private int imageWidth;
        private int imageHeight;
        private int thresholdValue;
        private bool changeThrashold = false;
        // Add a list to store the simplified curve points
        private List<Point> simplifiedCurvePoints = new();

        // Add a list to store the Bézier curves
        private List<BezierCurve> bezierCurves = new();

        // Threshold for RDP simplification
        private const double RDP_EPSILON = 5.0;

        public Form1()
        {
            InitializeComponent();
        }

        private void doubleBufferPanelDrawing_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (originalImage != null)
            {
                // Apply the combined filter
                byte[,] combinedData = originalImage.ApplyCombinedFilter();

                if (!changeThrashold)
                {
                    // Calculate the Otsu threshold
                    int otsuThreshold = originalImage.CalculateOtsuThreshold(combinedData);
                    thresholdValue = otsuThreshold;
                    changeThrashold = true;
                }

                numericUpDownThreshold.Value = thresholdValue;

                // Apply thresholding to the high-pass filtered data
                Bitmap bitmap = originalImage.ApplyThreshold(combinedData, thresholdValue);
                g.DrawImage(bitmap, new Point(0, 0));

                // Get the center of the line from the high-pass data
                PointF? center = originalImage.GetLineCenter(combinedData, thresholdValue);

                if (center.HasValue)
                {
                    // Draw a red circle at the center point
                    float radius = 5f;
                    using (Brush brush = new SolidBrush(Color.Red))
                    {
                        g.FillEllipse(brush, center.Value.X - radius, center.Value.Y - radius, radius * 2, radius * 2);
                    }
                }

                // Extract curve points
                List<Point> curvePoints = originalImage.ExtractCurvePoints(thresholdValue);

                // Simplify points
                simplifiedCurvePoints = GeometryUtils.RamerDouglasPeucker(curvePoints, RDP_EPSILON);

                // Fit Bézier curves
                bezierCurves = FitBezierCurves(simplifiedCurvePoints);

                // Draw Bézier curves
                foreach (var bezier in bezierCurves)
                {
                    using (Pen pen = new Pen(Color.Blue, 2))
                    {
                        g.DrawBezier(pen, bezier.StartPoint, bezier.ControlPoint1, bezier.ControlPoint2, bezier.EndPoint);
                    }
                }
            }
        }
        /// <summary>
        /// Fits cubic Bézier curves to the list of points.
        /// </summary>
        /// <param name="points">Simplified list of points.</param>
        /// <returns>List of Bézier curves.</returns>
        private List<BezierCurve> FitBezierCurves(List<Point> points)
        {
            List<BezierCurve> curves = new();

            if (points.Count < 2)
                return curves;

            // Simple approach: create a Bézier curve between every two consecutive points
            // with control points being the midpoints for smoothness
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point p0 = points[i];
                Point p3 = points[i + 1];

                // Calculate control points
                // You can improve this by calculating more accurate control points
                Point p1 = new Point((2 * p0.X + p3.X) / 3, (2 * p0.Y + p3.Y) / 3);
                Point p2 = new Point((p0.X + 2 * p3.X) / 3, (p0.Y + 2 * p3.Y) / 3);

                curves.Add(new BezierCurve(p0, p1, p2, p3));
            }

            return curves;
        }

        // Define a simple Bézier curve class
        private class BezierCurve
        {
            public Point StartPoint { get; }
            public Point ControlPoint1 { get; }
            public Point ControlPoint2 { get; }
            public Point EndPoint { get; }

            public BezierCurve(Point start, Point control1, Point control2, Point end)
            {
                StartPoint = start;
                ControlPoint1 = control1;
                ControlPoint2 = control2;
                EndPoint = end;
            }
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
            changeThrashold = false;
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
            changeThrashold = false;

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
