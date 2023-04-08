namespace DemoCube
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.glCanvas = new OpenTK.GLControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.alphaValue = new System.Windows.Forms.Label();
            this.alphaLabel = new System.Windows.Forms.Label();
            this.trackAlpha = new System.Windows.Forms.TrackBar();
            this.colorLabel = new System.Windows.Forms.Label();
            this.sideColor = new System.Windows.Forms.Panel();
            this.lightBtn = new System.Windows.Forms.Button();
            this.shadeBtn = new System.Windows.Forms.Button();
            this.colorChoice = new System.Windows.Forms.ColorDialog();
            this.info = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // glCanvas
            // 
            this.glCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glCanvas.BackColor = System.Drawing.Color.Black;
            this.glCanvas.Location = new System.Drawing.Point(159, 3);
            this.glCanvas.Name = "glCanvas";
            this.glCanvas.Size = new System.Drawing.Size(647, 461);
            this.glCanvas.TabIndex = 0;
            this.glCanvas.VSync = false;
            this.glCanvas.Load += new System.EventHandler(this.OnLoad);
            this.glCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.glCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMove);
            this.glCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnUp);
            this.glCanvas.Resize += new System.EventHandler(this.OnResize);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.28307F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.71693F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.glCanvas, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(809, 467);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.info);
            this.groupBox1.Controls.Add(this.alphaValue);
            this.groupBox1.Controls.Add(this.alphaLabel);
            this.groupBox1.Controls.Add(this.trackAlpha);
            this.groupBox1.Controls.Add(this.colorLabel);
            this.groupBox1.Controls.Add(this.sideColor);
            this.groupBox1.Controls.Add(this.lightBtn);
            this.groupBox1.Controls.Add(this.shadeBtn);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 461);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Управление:";
            // 
            // alphaValue
            // 
            this.alphaValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaValue.AutoSize = true;
            this.alphaValue.Location = new System.Drawing.Point(107, 270);
            this.alphaValue.Name = "alphaValue";
            this.alphaValue.Size = new System.Drawing.Size(13, 13);
            this.alphaValue.TabIndex = 6;
            this.alphaValue.Text = "1";
            // 
            // alphaLabel
            // 
            this.alphaLabel.AutoSize = true;
            this.alphaLabel.Location = new System.Drawing.Point(17, 197);
            this.alphaLabel.Name = "alphaLabel";
            this.alphaLabel.Size = new System.Drawing.Size(76, 13);
            this.alphaLabel.TabIndex = 5;
            this.alphaLabel.Text = "Альфа-канал:";
            // 
            // trackAlpha
            // 
            this.trackAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackAlpha.Location = new System.Drawing.Point(99, 163);
            this.trackAlpha.Maximum = 100;
            this.trackAlpha.Name = "trackAlpha";
            this.trackAlpha.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackAlpha.Size = new System.Drawing.Size(45, 104);
            this.trackAlpha.TabIndex = 4;
            this.trackAlpha.Value = 100;
            this.trackAlpha.ValueChanged += new System.EventHandler(this.OnChange);
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = true;
            this.colorLabel.Location = new System.Drawing.Point(9, 31);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(75, 13);
            this.colorLabel.TabIndex = 3;
            this.colorLabel.Text = "Выбор цвета:";
            // 
            // sideColor
            // 
            this.sideColor.BackColor = System.Drawing.Color.DarkGray;
            this.sideColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sideColor.Location = new System.Drawing.Point(110, 28);
            this.sideColor.Name = "sideColor";
            this.sideColor.Size = new System.Drawing.Size(20, 20);
            this.sideColor.TabIndex = 2;
            this.sideColor.Click += new System.EventHandler(this.ColorChoice);
            // 
            // lightBtn
            // 
            this.lightBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lightBtn.Location = new System.Drawing.Point(6, 115);
            this.lightBtn.Name = "lightBtn";
            this.lightBtn.Size = new System.Drawing.Size(138, 42);
            this.lightBtn.TabIndex = 1;
            this.lightBtn.Text = "Вкл.Освещение";
            this.lightBtn.UseVisualStyleBackColor = true;
            this.lightBtn.Click += new System.EventHandler(this.OnLightClick);
            // 
            // shadeBtn
            // 
            this.shadeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shadeBtn.Location = new System.Drawing.Point(6, 67);
            this.shadeBtn.Name = "shadeBtn";
            this.shadeBtn.Size = new System.Drawing.Size(138, 42);
            this.shadeBtn.TabIndex = 0;
            this.shadeBtn.Text = "Закрашенный";
            this.shadeBtn.UseVisualStyleBackColor = true;
            this.shadeBtn.Click += new System.EventHandler(this.OnShadeClick);
            // 
            // colorChoice
            // 
            this.colorChoice.Color = System.Drawing.Color.DarkGray;
            // 
            // info
            // 
            this.info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.info.Location = new System.Drawing.Point(6, 327);
            this.info.Multiline = true;
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(138, 128);
            this.info.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 467);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(825, 506);
            this.Name = "Form1";
            this.Text = "Демо Куб";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackAlpha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl glCanvas;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button lightBtn;
        private System.Windows.Forms.Button shadeBtn;
        private System.Windows.Forms.ColorDialog colorChoice;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Panel sideColor;
        private System.Windows.Forms.TrackBar trackAlpha;
        private System.Windows.Forms.Label alphaValue;
        private System.Windows.Forms.Label alphaLabel;
        private System.Windows.Forms.TextBox info;
    }
}

