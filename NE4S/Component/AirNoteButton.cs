using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NE4S.Define;

namespace NE4S.Component
{
    /// <summary>
    /// ノーツの向きを変更できるノーツボタン
    /// </summary>
    public class AirNoteButton : NoteButton
    {
        private int noteSize;
        private static readonly int virtualButtonWeight = 30;
        private enum ButtonArea : int
        {
            None = 0, Left = 1, Center = 2, Right = 3
        }
        private static class VirtualButtonRect
        {
            public static RectangleF Left { get; set; }
            public static RectangleF Center { get; set; }
            public static RectangleF Right { get; set; }
        }
        private ButtonArea buttonArea;
        private bool isMouseEnter, isSelected;

        public AirNoteButton(int noteType, NoteButtonEventHandler handler) : base(noteType, handler)
        {
            noteSize = Status.NoteSize;
            isMouseEnter = false;
            isSelected = false;
            buttonArea = ButtonArea.None;
            previewBox.MouseEnter += PreviewBox_MouseEnter;
            previewBox.MouseLeave += PreviewBox_MouseLeave;
            previewBox.MouseMove += PreviewBox_MouseMove;
            //
            VirtualButtonRect.Left = new RectangleF(0, 0, virtualButtonWeight, previewBox.Height);
            VirtualButtonRect.Center = new RectangleF(
                virtualButtonWeight, 0, previewBox.Width - virtualButtonWeight * 2, previewBox.Height);
            VirtualButtonRect.Right = new RectangleF(
                previewBox.Width - virtualButtonWeight, 0, virtualButtonWeight, previewBox.Height);
        }

        public void ChangeNoteDirection(int direction)
        {
            if(direction == NoteArea.LEFT)
            {
                if (noteType == NoteType.AIRDOWNC) { noteType = NoteType.AIRDOWNL; }
                else if(noteType == NoteType.AIRUPC) { noteType = NoteType.AIRUPL; }
                else if(noteType == NoteType.AIRDOWNR) { noteType = NoteType.AIRDOWNC; }
                else if(noteType == NoteType.AIRUPR) { noteType = NoteType.AIRUPC; }
            }
            else if(direction == NoteArea.RIGHT)
            {
                if (noteType == NoteType.AIRDOWNC) { noteType = NoteType.AIRDOWNR; }
                else if (noteType == NoteType.AIRUPC) { noteType = NoteType.AIRUPR; }
                else if (noteType == NoteType.AIRDOWNL) { noteType = NoteType.AIRDOWNC; }
                else if (noteType == NoteType.AIRUPL) { noteType = NoteType.AIRUPC; }
            }
            Status.Note = noteType;
        }

        protected override void PreviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (isSelected)
            {
                RefreshButtonArea(e.Location);
                if (buttonArea == ButtonArea.Left)
                {
                    if(noteType == NoteType.AIRUPC || noteType == NoteType.AIRUPR)
                    {
                        Status.Note = noteType = NoteType.AIRUPL;
                    }
                    else if (noteType == NoteType.AIRDOWNC || noteType == NoteType.AIRDOWNR)
                    {
                        Status.Note = noteType = NoteType.AIRDOWNL;
                    }
                }
                else if (buttonArea == ButtonArea.Center)
                {
                    if (noteType == NoteType.AIRUPL || noteType == NoteType.AIRUPR)
                    {
                        Status.Note = noteType = NoteType.AIRUPC;
                    }
                    else if (noteType == NoteType.AIRDOWNL || noteType == NoteType.AIRDOWNR)
                    {
                        Status.Note = noteType = NoteType.AIRDOWNC;
                    }

                }
                else if (buttonArea == ButtonArea.Right)
                {
                    if (noteType == NoteType.AIRUPL || noteType == NoteType.AIRUPC)
                    {
                        Status.Note = noteType = NoteType.AIRUPR;
                    }
                    else if (noteType == NoteType.AIRDOWNL || noteType == NoteType.AIRDOWNC)
                    {
                        Status.Note = noteType = NoteType.AIRDOWNR;
                    }
                }
            }
            base.PreviewBox_MouseDown(sender, e);
            return;
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
            return;
        }

        private void PreviewBox_MouseMove(object sender, MouseEventArgs e)
        {
            RefreshButtonArea(e.Location);
            previewBox.Refresh();
        }

        private void RefreshButtonArea(Point location)
        {
            if (VirtualButtonRect.Left.Contains(location))
            {
                buttonArea = ButtonArea.Left;
            }
            else if (VirtualButtonRect.Center.Contains(location))
            {
                buttonArea = ButtonArea.Center;
            }
            else if (VirtualButtonRect.Right.Contains(location))
            {
                buttonArea = ButtonArea.Right;
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
            return;
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
                    e.Graphics.DrawLine(pen, new Point(virtualButtonWeight, 0), new Point(virtualButtonWeight, previewBox.Height));
                    e.Graphics.DrawLine(pen, new Point(previewBox.Width - virtualButtonWeight, 0), new Point(previewBox.Width - virtualButtonWeight, previewBox.Height));
                }
                using (SolidBrush brush = new SolidBrush(guideColor))
                {
                    if (buttonArea == ButtonArea.Left)
                    {
                        e.Graphics.FillRectangle(brush, VirtualButtonRect.Left);
                    }
                    else if (buttonArea == ButtonArea.Center)
                    {
                        e.Graphics.FillRectangle(brush, VirtualButtonRect.Center);
                    }
                    else if (buttonArea == ButtonArea.Right)
                    {
                        e.Graphics.FillRectangle(brush, VirtualButtonRect.Right);
                    }
                }
            }
        }
    }
}
