using NE4S.Scores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Component
{
    public class SpeedNoteButton : ValueNoteButton
    {
        public SpeedNoteButton(int noteType, NoteButtonEventHandler handler) : base(noteType, handler) { }

        protected override void PreviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            base.PreviewBox_MouseDown(sender, e);
            if (e.Button == MouseButtons.Right)
            {

            }
        }

        protected override void DrawValue(PaintEventArgs e)
        {
            using (Font myFont = new Font("MS UI Gothic", ScoreInfo.FontSize, FontStyle.Bold))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(
                "Speed: x" + (CurrentValue + valueDelta),
                myFont,
                Brushes.Red,
                new PointF(1, 78));
            }
        }
    }
}
