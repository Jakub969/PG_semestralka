namespace cv1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelTools = new Panel();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            numericUpDownHeight = new NumericUpDown();
            numericUpDownWidth = new NumericUpDown();
            comboBox1 = new ComboBox();
            panelHistogram = new DoubleBufferPanel();
            doubleBufferPanelDrawing = new DoubleBufferPanel();
            contextMenuStrip1 = new ContextMenuStrip(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            setDirectoryToolStripMenuItem = new ToolStripMenuItem();
            panelTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWidth).BeginInit();
            doubleBufferPanelDrawing.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panelTools
            // 
            panelTools.BackColor = SystemColors.AppWorkspace;
            panelTools.Controls.Add(label3);
            panelTools.Controls.Add(label2);
            panelTools.Controls.Add(label1);
            panelTools.Controls.Add(numericUpDownHeight);
            panelTools.Controls.Add(numericUpDownWidth);
            panelTools.Controls.Add(comboBox1);
            panelTools.Controls.Add(menuStrip1);
            panelTools.Dock = DockStyle.Left;
            panelTools.Location = new Point(0, 0);
            panelTools.Margin = new Padding(3, 4, 3, 4);
            panelTools.Name = "panelTools";
            panelTools.Size = new Size(205, 933);
            panelTools.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(40, 144);
            label3.Name = "label3";
            label3.Size = new Size(97, 20);
            label3.TabIndex = 4;
            label3.Text = "Image height";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(45, 106);
            label2.Name = "label2";
            label2.Size = new Size(92, 20);
            label2.TabIndex = 4;
            label2.Text = "Image width";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 39);
            label1.Name = "label1";
            label1.Size = new Size(76, 20);
            label1.TabIndex = 3;
            label1.Text = "Image file";
            // 
            // numericUpDownHeight
            // 
            numericUpDownHeight.Location = new Point(135, 137);
            numericUpDownHeight.Margin = new Padding(3, 4, 3, 4);
            numericUpDownHeight.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDownHeight.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            numericUpDownHeight.Name = "numericUpDownHeight";
            numericUpDownHeight.Size = new Size(51, 27);
            numericUpDownHeight.TabIndex = 2;
            numericUpDownHeight.Value = new decimal(new int[] { 512, 0, 0, 0 });
            numericUpDownHeight.ValueChanged += numericUpDownHeight_ValueChanged;
            // 
            // numericUpDownWidth
            // 
            numericUpDownWidth.Location = new Point(135, 99);
            numericUpDownWidth.Margin = new Padding(3, 4, 3, 4);
            numericUpDownWidth.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDownWidth.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            numericUpDownWidth.Name = "numericUpDownWidth";
            numericUpDownWidth.Size = new Size(51, 27);
            numericUpDownWidth.TabIndex = 1;
            numericUpDownWidth.Value = new decimal(new int[] { 512, 0, 0, 0 });
            numericUpDownWidth.ValueChanged += numericUpDownWidth_ValueChanged;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(14, 63);
            comboBox1.Margin = new Padding(3, 4, 3, 4);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(172, 28);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // panelHistogram
            // 
            panelHistogram.Location = new Point(0, 533);
            panelHistogram.Name = "panelHistogram";
            panelHistogram.Size = new Size(1415, 400);
            panelHistogram.TabIndex = 0;
            panelHistogram.Paint += panelHistogram_Paint;
            // 
            // doubleBufferPanelDrawing
            // 
            doubleBufferPanelDrawing.Controls.Add(panelHistogram);
            doubleBufferPanelDrawing.Dock = DockStyle.Fill;
            doubleBufferPanelDrawing.Location = new Point(205, 0);
            doubleBufferPanelDrawing.Margin = new Padding(3, 4, 3, 4);
            doubleBufferPanelDrawing.Name = "doubleBufferPanelDrawing";
            doubleBufferPanelDrawing.Size = new Size(1417, 933);
            doubleBufferPanelDrawing.TabIndex = 1;
            doubleBufferPanelDrawing.Paint += doubleBufferPanelDrawing_Paint;
            doubleBufferPanelDrawing.MouseDown += doubleBufferPanelDrawing_MouseDown;
            doubleBufferPanelDrawing.MouseMove += doubleBufferPanelDrawing_MouseMove;
            doubleBufferPanelDrawing.MouseUp += doubleBufferPanelDrawing_MouseUp;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(205, 28);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { setDirectoryToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // setDirectoryToolStripMenuItem
            // 
            setDirectoryToolStripMenuItem.Name = "setDirectoryToolStripMenuItem";
            setDirectoryToolStripMenuItem.Size = new Size(224, 26);
            setDirectoryToolStripMenuItem.Text = "Set directory";
            setDirectoryToolStripMenuItem.Click += setDirectory;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1622, 933);
            Controls.Add(doubleBufferPanelDrawing);
            Controls.Add(panelTools);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            panelTools.ResumeLayout(false);
            panelTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWidth).EndInit();
            doubleBufferPanelDrawing.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTools;
        private DoubleBufferPanel doubleBufferPanelDrawing;
        private ComboBox comboBox1;
        private Label label3;
        private Label label2;
        private Label label1;
        private NumericUpDown numericUpDownHeight;
        private NumericUpDown numericUpDownWidth;
        private DoubleBufferPanel panelHistogram;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem setDirectoryToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
    }
}
