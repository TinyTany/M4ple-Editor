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
            this.label1.Location = new System.Drawing.Point(33, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "挿入位置";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "拍数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "個数";
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(150, 302);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(80, 30);
            this.OK.TabIndex = 3;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(250, 302);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(80, 30);
            this.Cancel.TabIndex = 4;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(322, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 18);
            this.label4.TabIndex = 5;
            this.label4.Text = "/";
            // 
            // Direction
            // 
            this.Direction.FormattingEnabled = true;
            this.Direction.Items.AddRange(new object[] {
            "選択小節の直前",
            "選択小節の直後"});
            this.Direction.Location = new System.Drawing.Point(260, 34);
            this.Direction.Name = "Direction";
            this.Direction.Size = new System.Drawing.Size(185, 26);
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
            this.BeatNumer.Location = new System.Drawing.Point(216, 105);
            this.BeatNumer.Name = "BeatNumer";
            this.BeatNumer.Size = new System.Drawing.Size(100, 26);
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
            this.BeatDenom.Location = new System.Drawing.Point(345, 105);
            this.BeatDenom.Name = "BeatDenom";
            this.BeatDenom.Size = new System.Drawing.Size(100, 26);
            this.BeatDenom.TabIndex = 8;
            // 
            // BarCount
            // 
            this.BarCount.Location = new System.Drawing.Point(325, 180);
            this.BarCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BarCount.Name = "BarCount";
            this.BarCount.Size = new System.Drawing.Size(120, 25);
            this.BarCount.TabIndex = 9;
            this.BarCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BarAddCustomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 344);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
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