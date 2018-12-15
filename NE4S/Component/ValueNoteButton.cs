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
    public class ValueNoteButton : SizableNoteButton
    {
        public float ValueMin { get; set; }
        public float ValueMax { get; set; }
        public float CurrentValue { get; set; }
        protected float valueDelta;

        public ValueNoteButton(int noteType, NoteButtonEventHandler handler) : base(noteType, handler)
        {
            valueDelta = 0;
        }

        protected override void ChangeValueByButton()
        {
            if (buttonArea == ButtonArea.Top)
            {
                Status.CurrentValue = CurrentValue <= ValueMax - 1 ? ++CurrentValue : CurrentValue = ValueMax;
            }
            else if (buttonArea == ButtonArea.Bottom)
            {
                Status.CurrentValue = CurrentValue >= ValueMin + 1 ? --CurrentValue : CurrentValue = ValueMin;
            }
        }

        protected override void ChangeValueByMouse(Point location)
        {
            int pixelPerSize = 15;
            valueDelta = -(location.Y - pressedLocation.Y) / pixelPerSize;
            if(ModifierKeys == Keys.Shift)
            {
                valueDelta /= 100f;
                valueDelta = valueDelta.MyRound();
            }
            if (CurrentValue + valueDelta < ValueMin)
            {
                valueDelta = (int)(ValueMin - CurrentValue);
            }
            else if (CurrentValue + valueDelta > ValueMax)
            {
                valueDelta = (int)(ValueMax - CurrentValue);
            }
        }

        protected override void PreviewBox_MouseUp(object sender, MouseEventArgs e)
        {
            isMousePressed = false;
            Status.CurrentValue = CurrentValue += valueDelta;
            valueDelta = 0;
        }

        public override void SetSelected()
        {
            base.SetSelected();
            Status.CurrentValue = CurrentValue;
        }

        protected override void DrawValue(PaintEventArgs e)
        {
            using (Font myFont = new Font("MS UI Gothic", ScoreInfo.FontSize, FontStyle.Bold))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(
                "Value: " + (CurrentValue + valueDelta),
                myFont,
                Brushes.White,
                new PointF(1, 78));
            }
        }
    }
}
