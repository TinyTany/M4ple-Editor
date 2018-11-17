using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Component
{
    public partial class NoteButton : UserControl
    {
        public delegate void NoteButtonEventHandler(NoteButton noteButton);
        public event NoteButtonEventHandler UpdateSelectedNoteButton;
        private int noteType;
        private static readonly Color selected = Color.FromArgb(255, 56, 135, 245);
        private static readonly Color unSelected = SystemColors.Control;

        public NoteButton(int noteType, NoteButtonEventHandler handler)
        {
            InitializeComponent();
            previewBox.Click += PreviewBox_Click;
            previewBox.Paint += PreviewBox_Paint;
            UpdateSelectedNoteButton += handler;
            this.noteType = noteType;
        }

        private void PreviewBox_Click(object sender, EventArgs e)
        {
            UpdateSelectedNoteButton?.Invoke(this);
            return;
        }

        public void SetSelected()
        {
            Status.Note = noteType;
            BackColor = selected;
            return;
        }

        public void SetUnSelected()
        {
            BackColor = unSelected;
            return;
        }

        private void PreviewBox_Paint(object sender, PaintEventArgs e)
        {
            
            return;
        }
    }
}
