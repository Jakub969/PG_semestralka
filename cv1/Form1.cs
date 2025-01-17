using System.Data;
using System.Drawing.Drawing2D;
using System.IO;

namespace cv1
{
    public partial class Form1 : Form
    {
        private List<Point> mousePositions = [];
        private GrayscaleImage? originalImage;
        private int imageWidth;
        private int imageHeight;
        private int thresholdValue = 65;

        public Form1()
        {
            InitializeComponent();
        }

        private void doubleBufferPanelDrawing_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (originalImage != null)
            {
                // Apply the Gaussian high-pass filter
                //byte[,] highPassData = originalImage.ApplyGaussianHighPassFilter();
                byte[,] combinedData = originalImage.ApplyCombinedFilter();

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
            }
        }



        private void doubleBufferPanelDrawing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePositions.Add(e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
                mousePositions.Clear();
            }

            doubleBufferPanelDrawing.Invalidate();
        }

        private void doubleBufferPanelDrawing_MouseMove(object sender, MouseEventArgs e)
        {

            doubleBufferPanelDrawing.Invalidate();
        }

        private void doubleBufferPanelDrawing_MouseUp(object sender, MouseEventArgs e)
        {

            doubleBufferPanelDrawing.Invalidate();
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
