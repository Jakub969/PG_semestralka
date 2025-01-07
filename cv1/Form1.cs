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

            //Rectangle r = new(100, 100, 200, 100);

            //using LinearGradientBrush lBrush = new(r, Color.Red, Color.Yellow, LinearGradientMode.BackwardDiagonal);

            //g.FillRectangle(lBrush, r);

            //using Pen pen1 = new(Color.Blue);
            //g.DrawRectangle(pen1, r);

            //g.DrawArc(pen1, r, 12, 84);

            //Point p1 = new(150, 50);
            //Point p2 = new(250, 150);

            //g.DrawLine(pen1, p1, p2);

            //using Pen pen2 = new Pen(Color.Red, 4);
            //Rectangle r2 = new(300, 100, 200, 100);
            //g.FillEllipse(Brushes.Purple, r2);
            //g.DrawEllipse(pen2, r2);

            //using Font fnt = new("Verdana", 16);
            //g.DrawString("GDI+ World", fnt, new SolidBrush(Color.Red), 14, 10);



            //foreach (Point pt in mousePositions)
            //{
            //    Rectangle mouseRectangle = new(pt.X - 25, pt.Y - 25, 50, 50);
            //    g.DrawEllipse(pen1, mouseRectangle);
            //}

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
                originalImage = new(imageBytes, imageWidth, imageHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading file!", MessageBoxButtons.OK);
                return;
            }

            doubleBufferPanelDrawing.Invalidate();
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
    }
}
