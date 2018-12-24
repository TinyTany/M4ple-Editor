namespace NE4S.Component
{
    partial class NewScoreForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewScoreForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.beatNumer = new System.Windows.Forms.ComboBox();
            this.beatDenom = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numOfMeasure = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numOfMeasure)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(105, 200);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(56, 25);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(175, 200);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 25);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // beatNumer
            // 
            this.beatNumer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.beatNumer.FormattingEnabled = true;
            this.beatNumer.Items.AddRange(new object[] {
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
            this.beatNumer.Location = new System.Drawing.Point(139, 63);
            this.beatNumer.Name = "beatNumer";
            this.beatNumer.Size = new System.Drawing.Size(72, 23);
            this.beatNumer.TabIndex = 1;
            // 
            // beatDenom
            // 
            this.beatDenom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.beatDenom.FormattingEnabled = true;
            this.beatDenom.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32",
            "64"});
            this.beatDenom.Location = new System.Drawing.Point(235, 63);
            this.beatDenom.Name = "beatDenom";
            this.beatDenom.Size = new System.Drawing.Size(72, 23);
            this.beatDenom.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(217, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "/";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "基本拍子数";
            // 
            // numOfMeasure
            // 
            this.numOfMeasure.Location = new System.Drawing.Point(187, 122);
            this.numOfMeasure.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numOfMeasure.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOfMeasure.Name = "numOfMeasure";
            this.numOfMeasure.Size = new System.Drawing.Size(120, 23);
            this.numOfMeasure.TabIndex = 3;
            this.numOfMeasure.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "小節数";
            // 
            // NewScoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 240);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numOfMeasure);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.beatDenom);
            this.Controls.Add(this.beatNumer);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewScoreForm";
            this.Text = "新規作成";
            ((System.ComponentModel.ISupportInitialize)(this.numOfMeasure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox beatNumer;
        private System.Windows.Forms.ComboBox beatDenom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numOfMeasure;
        private System.Windows.Forms.Label label3;
    }
}