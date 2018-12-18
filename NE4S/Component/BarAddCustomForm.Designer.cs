namespace NE4S.Component
{
    partial class BarAddCustomForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BarAddCustomForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Direction = new System.Windows.Forms.ComboBox();
            this.BeatNumer = new System.Windows.Forms.ComboBox();
            this.BeatDenom = new System.Windows.Forms.ComboBox();
            this.BarCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.BarCount)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "挿入位置";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "拍数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 151);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "個数";
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(105, 251);
            this.OK.Margin = new System.Windows.Forms.Padding(2);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(56, 25);
            this.OK.TabIndex = 3;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(175, 251);
            this.Cancel.Margin = new System.Windows.Forms.Padding(2);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(56, 25);
            this.Cancel.TabIndex = 4;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(225, 90);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "/";
            // 
            // Direction
            // 
            this.Direction.FormattingEnabled = true;
            this.Direction.Items.AddRange(new object[] {
            "選択小節の1つ先",
            "選択小節の1つ前"});
            this.Direction.Location = new System.Drawing.Point(182, 29);
            this.Direction.Margin = new System.Windows.Forms.Padding(2);
            this.Direction.Name = "Direction";
            this.Direction.Size = new System.Drawing.Size(131, 23);
            this.Direction.TabIndex = 6;
            // 
            // BeatNumer
            // 
            this.BeatNumer.FormattingEnabled = true;
            this.BeatNumer.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32"});
            this.BeatNumer.Location = new System.Drawing.Point(152, 88);
            this.BeatNumer.Margin = new System.Windows.Forms.Padding(2);
            this.BeatNumer.Name = "BeatNumer";
            this.BeatNumer.Size = new System.Drawing.Size(72, 23);
            this.BeatNumer.TabIndex = 7;
            // 
            // BeatDenom
            // 
            this.BeatDenom.FormattingEnabled = true;
            this.BeatDenom.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64"});
            this.BeatDenom.Location = new System.Drawing.Point(241, 88);
            this.BeatDenom.Margin = new System.Windows.Forms.Padding(2);
            this.BeatDenom.Name = "BeatDenom";
            this.BeatDenom.Size = new System.Drawing.Size(72, 23);
            this.BeatDenom.TabIndex = 8;
            // 
            // BarCount
            // 
            this.BarCount.Location = new System.Drawing.Point(227, 150);
            this.BarCount.Margin = new System.Windows.Forms.Padding(2);
            this.BarCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BarCount.Name = "BarCount";
            this.BarCount.Size = new System.Drawing.Size(84, 23);
            this.BarCount.TabIndex = 9;
            this.BarCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BarAddCustomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 286);
            this.Controls.Add(this.BarCount);
            this.Controls.Add(this.BeatDenom);
            this.Controls.Add(this.BeatNumer);
            this.Controls.Add(this.Direction);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BarAddCustomForm";
            this.Text = "カスタム小節挿入";
            ((System.ComponentModel.ISupportInitialize)(this.BarCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox Direction;
        private System.Windows.Forms.ComboBox BeatNumer;
        private System.Windows.Forms.ComboBox BeatDenom;
        private System.Windows.Forms.NumericUpDown BarCount;
    }
}