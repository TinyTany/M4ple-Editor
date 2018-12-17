namespace NE4S.Component
{
    partial class ExportForm
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
            this.ExportButton = new System.Windows.Forms.Button();
            this.exportCancelButton = new System.Windows.Forms.Button();
            this.exportPathText = new System.Windows.Forms.TextBox();
            this.ExportPathButton = new System.Windows.Forms.Button();
            this.pathClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(12, 426);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(279, 23);
            this.ExportButton.TabIndex = 0;
            this.ExportButton.Text = "エクスポート";
            this.ExportButton.UseVisualStyleBackColor = true;
            // 
            // exportCancelButton
            // 
            this.exportCancelButton.Location = new System.Drawing.Point(297, 426);
            this.exportCancelButton.Name = "exportCancelButton";
            this.exportCancelButton.Size = new System.Drawing.Size(75, 23);
            this.exportCancelButton.TabIndex = 1;
            this.exportCancelButton.Text = "キャンセル";
            this.exportCancelButton.UseVisualStyleBackColor = true;
            // 
            // exportPathText
            // 
            this.exportPathText.Location = new System.Drawing.Point(111, 348);
            this.exportPathText.Name = "exportPathText";
            this.exportPathText.ReadOnly = true;
            this.exportPathText.Size = new System.Drawing.Size(180, 19);
            this.exportPathText.TabIndex = 2;
            // 
            // ExportPathButton
            // 
            this.ExportPathButton.Location = new System.Drawing.Point(297, 346);
            this.ExportPathButton.Name = "ExportPathButton";
            this.ExportPathButton.Size = new System.Drawing.Size(23, 23);
            this.ExportPathButton.TabIndex = 3;
            this.ExportPathButton.Text = "...";
            this.ExportPathButton.UseVisualStyleBackColor = true;
            // 
            // pathClearButton
            // 
            this.pathClearButton.Location = new System.Drawing.Point(326, 346);
            this.pathClearButton.Name = "pathClearButton";
            this.pathClearButton.Size = new System.Drawing.Size(46, 23);
            this.pathClearButton.TabIndex = 4;
            this.pathClearButton.Text = "クリア";
            this.pathClearButton.UseVisualStyleBackColor = true;
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.pathClearButton);
            this.Controls.Add(this.ExportPathButton);
            this.Controls.Add(this.exportPathText);
            this.Controls.Add(this.exportCancelButton);
            this.Controls.Add(this.ExportButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportForm";
            this.Text = "エクスポート";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Button exportCancelButton;
        private System.Windows.Forms.TextBox exportPathText;
        private System.Windows.Forms.Button ExportPathButton;
        private System.Windows.Forms.Button pathClearButton;
    }
}