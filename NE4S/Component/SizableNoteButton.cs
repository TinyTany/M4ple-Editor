using NE4S.Define;
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
    /// <summary>
    /// ノーツサイズを変更できるノーツボタン
    /// </summary>
    public class SizableNoteButton : NoteButton
    {
        private int noteSize;
        private static readonly int virtualButtonWeight = 20;
        protected enum ButtonArea : int
        {
            None = 0, Top = 1, Center = 2, Bottom = 3
        }
        private static class VirtualButtonRect
        {
            public static RectangleF Top { get; set; }
            public static RectangleF Center { get; set; }
            public static RectangleF Bottom { get; set; }
        }
        protected ButtonArea buttonArea;
        /// <summary>
        /// このボタン上にマウスがあるか判定
        /// </summary>
        protected bool isMouseEnter;
        /// <summary>
        /// このボタンが現在選択されているか判定
        /// </summary>
        protected bool isSelected;
        /// <summary>
        /// このボタンが現在押されっぱなしになっているか判定
        /// </summary>
        protected bool isMousePressed;
        /// <summary>
        /// クリックされた位置
        /// </summary>
        protected Point pressedLocation;
        /// <summary>
        /// クリックされた位置と現在のポインタ位置の差から計算したサイズを変化させる分の大きさ
        /// </summary>
        private int sizeDelta;

        public SizableNoteButton(int noteType, NoteButtonEventHandler handler) : base(noteType, handler)
        {
            noteSize = Status.NoteSize;
            isMouseEnter = false;
            isSelected = false;
            isMousePressed = false;
            pressedLocation = new Point();
            sizeDelta = 0;
            buttonArea = ButtonArea.None;
            previewBox.MouseEnter += PreviewBox_MouseEnter;
            previewBox.MouseLeave += PreviewBox_MouseLeave;
            previewBox.MouseMove += PreviewBox_MouseMove;
            previewBox.MouseUp += PreviewBox_MouseUp;
            //
            VirtualButtonRect.Top = new RectangleF(0, 0, previewBox.Width, virtualButtonWeight);
            VirtualButtonRect.Center = new RectangleF(0, virtualButtonWeight, previewBox.Width, previewBox.Height - virtualButtonWeight * 2);
            VirtualButtonRect.Bottom = new RectangleF(0, previewBox.Height - virtualButtonWeight, previewBox.Width, virtualButtonWeight);
        }

        protected override void PreviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            isMousePressed = true;
            pressedLocation = e.Location;
            if (!isSelected)
            {
                base.PreviewBox_MouseDown(sender, e);
            }
            else
            {
                RefreshButtonArea(e.Location);
                if(buttonArea == ButtonArea.Top)
                {
                    Status.NoteSize = noteSize < 16 ? ++noteSize : noteSize;
                }
                else if(buttonArea == ButtonArea.Bottom)
                {
                    Status.NoteSize = noteSize > 1 ? --noteSize : noteSize;
                }
            }
        }

        private void PreviewBox_MouseEnter(object sender, EventArgs e)
        {
            isMouseEnter = true;
            previewBox.Refresh();
        }

        private void PreviewBox_MouseLeave(object sender, EventArgs e)
        {
            isMouseEnter = false;
            previewBox.Refresh();
        }

        private void PreviewBox_MouseMove(object sender, MouseEventArgs e)
        {
            RefreshButtonArea(e.Location);
            if (isSelected && buttonArea == ButtonArea.Center)
            {
                Cursor = Cursors.SizeNS;
            }
            else
            {
                Cursor = Cursors.Default;
            }
            if (isMousePressed)
            {
                int pixelPerSize = 15;
                sizeDelta = -(e.Y - pressedLocation.Y) / pixelPerSize;
                if (noteSize + sizeDelta < 1) {
                    sizeDelta = 1 - noteSize;
                }
                else if (noteSize + sizeDelta > 16)
                {
                    sizeDelta = 16 - noteSize;
                }
            }
            previewBox.Refresh();
        }

        private void PreviewBox_MouseUp(object sender, MouseEventArgs e)
        {
            isMousePressed = false;
            Status.NoteSize = noteSize += sizeDelta;
            sizeDelta = 0;
        }

        protected void RefreshButtonArea(Point location)
        {
            if (VirtualButtonRect.Top.Contains(location))
            {
                buttonArea = ButtonArea.Top;
            }
            else if (VirtualButtonRect.Center.Contains(location))
            {
                buttonArea = ButtonArea.Center;
            }
            else if (VirtualButtonRect.Bottom.Contains(location))
            {
                buttonArea = ButtonArea.Bottom;
            }
            else
            {
                buttonArea = ButtonArea.None;
            }
            return;
        }

        public override void SetSelected()
        {
            base.SetSelected();
            Status.NoteSize = noteSize;
            isSelected = true;
        }

        public override void SetUnSelected()
        {
            base.SetUnSelected();
            isSelected = false;
        }

        protected override void PreviewBox_Paint(object sender, PaintEventArgs e)
        {
            base.PreviewBox_Paint(sender, e);
            if (isMouseEnter && isSelected)
            {
                Color guideColor = Color.FromArgb(150, 255, 255, 255);
                using (Pen pen = new Pen(guideColor))
                {
                    e.Graphics.DrawLine(pen, new Point(0, virtualButtonWeight), new Point(previewBox.Width, virtualButtonWeight));
                    e.Graphics.DrawLine(pen, new Point(0, previewBox.Height - virtualButtonWeight), new Point(previewBox.Width, previewBox.Height - virtualButtonWeight));
                    
                }
                using (SolidBrush brush = new SolidBrush(guideColor))
                {
                    if (buttonArea == ButtonArea.Top)
                    {
                        e.Graphics.FillRectangle(brush, VirtualButtonRect.Top);
                    }
                    else if (buttonArea == ButtonArea.Bottom)
                    {
                        e.Graphics.FillRectangle(brush, VirtualButtonRect.Bottom);
                    }
                }
            }
            using (Font myFont = new Font("MS UI Gothic", ScoreInfo.FontSize, FontStyle.Bold))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(
                "NoteSize: " + (noteSize + sizeDelta),
                myFont,
                Brushes.White,
                new PointF(10, 70));
            }
        }
    }
}
