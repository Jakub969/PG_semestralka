using System.Data;
using System.Drawing.Drawing2D;

namespace cv1
{
    public partial class Form1 : Form
    {
        private List<Point> mousePositions = [];
        private GrayscaleImage? originalImage;
        private int imageWidth;
        private int imageHeight;

        public Form1()
        {
            InitializeComponent();
        }

        private void doubleBufferPanelDrawing_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (originalImage != null)
            {
                using Bitmap bitmap = originalImage.ToBitmap();
                g.DrawImage(bitmap, new Point(0, 0));
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
            string path = @"C:\\Users\\jakub\\Desktop\\data";

            DataTable table = new();
            table.Columns.Add("File Name");
            table.Columns.Add("File Path");

            string[] files = Directory.GetFiles(path);

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

        private void ReloadImage()
        {
            originalImage = null;

            string? selectedString = comboBox1.SelectedValue as string;

            if (string.IsNullOrEmpty(selectedString))
                return;

            try
            {
                byte[] imageBytes = File.ReadAllBytes(selectedString);
                originalImage = new GrayscaleImage(imageBytes, imageWidth, imageHeight);
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

    }
}
