using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Define;

namespace NE4S.Component
{
    /// <summary>
    /// 基本となるノーツボタンクラス
    /// </summary>
    public partial class NoteButton : UserControl
    {
        public delegate void NoteButtonEventHandler(NoteButton noteButton);
        public event NoteButtonEventHandler UpdateSelectedNoteButton;
        protected int noteType;
        private static readonly Color selected = Color.FromArgb(255, 56, 135, 245);
        private static readonly Color unSelected = SystemColors.Control;
        private static readonly int margin = 5;

        public NoteButton(int noteType, NoteButtonEventHandler handler)
        {
            InitializeComponent();
            previewBox.MouseDown += PreviewBox_MouseDown;
            previewBox.Paint += PreviewBox_Paint;
            UpdateSelectedNoteButton += handler;
            this.noteType = noteType;
            //
            Size = new Size(150, 100);
            previewBox.Size = new Size(Width - margin * 2, Height - margin * 2);
            previewBox.Location = new Point(margin - 1, margin - 1);
        }

        protected virtual void PreviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateSelectedNoteButton?.Invoke(this);
            return;
        }

        public virtual void SetSelected()
        {
            Status.Note = noteType;
            BackColor = selected;
            return;
        }

        public virtual void SetUnSelected()
        {
            BackColor = unSelected;
            return;
        }

        protected virtual void PreviewBox_Paint(object sender, PaintEventArgs e)
        {
            using(SolidBrush brush = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(brush, previewBox.ClientRectangle);
            }
            //
            //TODO: このへんでノーツ画像描画？
            //
            return;
        }
    }
}
