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
    /// TODO: 汚くなってきたしクラス分けたりしたほうがいいんじゃない？
    /// </summary>
    public partial class NoteButton : UserControl
    {
        public delegate void NoteButtonEventHandler(NoteButton noteButton);
        public event NoteButtonEventHandler UpdateSelectedNoteButton;
        private int noteType, noteSize;
        private static readonly Color selected = Color.FromArgb(255, 56, 135, 245);
        private static readonly Color unSelected = SystemColors.Control;
        public static readonly int margin = 5;
        private bool isMouseEnter = false;
        private bool isSelected = false;
        public static readonly int virtualButtonWeight = 20;
        private enum ButtonArea : int
        {
            None = 0, Top = 1, Bottom = 2, Left = 3, Right = 4, Center = 5
        }
        private static class VirtualButtonRect
        {
            public static RectangleF Top { get; set; }
            public static RectangleF Bottom { get; set; }
            public static RectangleF Center { get; set; }
            public static RectangleF Left { get; set; }
            public static RectangleF Right { get; set; }
        }
        private ButtonArea buttonArea;
        

        public NoteButton(int noteType, NoteButtonEventHandler handler)
        {
            InitializeComponent();
            previewBox.MouseDown += PreviewBox_MouseDown;
            previewBox.Paint += PreviewBox_Paint;
            UpdateSelectedNoteButton += handler;
            previewBox.MouseEnter += PreviewBox_MouseEnter;
            previewBox.MouseLeave += PreviewBox_MouseLeave;
            previewBox.MouseMove += PreviewBox_MouseMove;
            this.noteType = noteType;
            noteSize = Status.NoteSize;
            buttonArea = ButtonArea.None;
            //
            Size = new Size(150, 100);
            previewBox.Size = new Size(Width - margin * 2, Height - margin * 2);
            previewBox.Location = new Point(margin - 1, margin - 1);
            VirtualButtonRect.Top = new RectangleF(0, 0, previewBox.Width, virtualButtonWeight);
            VirtualButtonRect.Bottom = new RectangleF(0, previewBox.Height - virtualButtonWeight, previewBox.Width, virtualButtonWeight);
            VirtualButtonRect.Center = new RectangleF(virtualButtonWeight, virtualButtonWeight, previewBox.Width - virtualButtonWeight * 2, previewBox.Height - virtualButtonWeight * 2);
            VirtualButtonRect.Left = new RectangleF(0, virtualButtonWeight, virtualButtonWeight, previewBox.Height - virtualButtonWeight * 2);
            VirtualButtonRect.Right = new RectangleF(previewBox.Width - virtualButtonWeight, virtualButtonWeight, virtualButtonWeight, previewBox.Height - virtualButtonWeight * 2);
        }

        private void PreviewBox_MouseEnter(object sender, EventArgs e)
        {
            isMouseEnter = true;
            previewBox.Refresh();
            return;
        }

        private void PreviewBox_MouseLeave(object sender, EventArgs e)
        {
            isMouseEnter = false;
            previewBox.Refresh();
            return;
        }

        private void PreviewBox_MouseDown(object sender, MouseEventArgs e)
        {
            //最初選択されてなかったときは選択のみ。選択されてた場合はクリックされた場所から各種処理（ノーツのサイズや向き変更）をする。
            if (!isSelected)
            {
                UpdateSelectedNoteButton?.Invoke(this);
            }
            else
            {
                RefreshButtonArea(e.Location);
                #region クリックされたエリアに応じてノーツのサイズや向きを変更する処理
                switch (buttonArea)
                {
                    case ButtonArea.Top:
                        Status.NoteSize = noteSize < 16 ? ++noteSize : noteSize;
                        break;
                    case ButtonArea.Bottom:
                        Status.NoteSize = noteSize > 1 ? --noteSize : noteSize;
                        break;
                    case ButtonArea.Center:
                        if (noteType == NoteType.AIRUPL || noteType == NoteType.AIRUPR)
                        {
                            Status.Note = noteType = NoteType.AIRUPC;
                        }
                        else if (noteType == NoteType.AIRDOWNL || noteType == NoteType.AIRDOWNR)
                        {
                            Status.Note = noteType = NoteType.AIRDOWNC;
                        }
                        break;
                    case ButtonArea.Left:
                        if (noteType == NoteType.AIRUPC || noteType == NoteType.AIRUPR)
                        {
                            Status.Note = noteType = NoteType.AIRUPL;
                        }
                        else if (noteType == NoteType.AIRDOWNC || noteType == NoteType.AIRDOWNR)
                        {
                            Status.Note = noteType = NoteType.AIRDOWNL;
                        }
                        break;
                    case ButtonArea.Right:
                        if (noteType == NoteType.AIRUPL || noteType == NoteType.AIRUPC)
                        {
                            Status.Note = noteType = NoteType.AIRUPR;
                        }
                        else if (noteType == NoteType.AIRDOWNL || noteType == NoteType.AIRDOWNC)
                        {
                            Status.Note = noteType = NoteType.AIRDOWNR;
                        }
                        break;
                    default:
                        break;
                }
                #endregion
            }
            return;
        }

        private void PreviewBox_MouseMove(object sender, MouseEventArgs e)
        {
            RefreshButtonArea(e.Location);
            previewBox.Refresh();
            return;
        }

        private void RefreshButtonArea(Point location)
        {
            if (VirtualButtonRect.Top.Contains(location))
            {
                buttonArea = ButtonArea.Top;
            }
            else if(VirtualButtonRect.Bottom.Contains(location))
            {
                buttonArea = ButtonArea.Bottom;
            }
            else if(VirtualButtonRect.Center.Contains(location))
            {
                buttonArea = ButtonArea.Center;
            }
            else if(VirtualButtonRect.Left.Contains(location))
            {
                buttonArea = ButtonArea.Left;
            }
            else if(VirtualButtonRect.Right.Contains(location))
            {
                buttonArea = ButtonArea.Right;
            }
            else
            {
                buttonArea = ButtonArea.None;
            }
            return;
        }

        public void SetSelected()
        {
            Status.Note = noteType;
            BackColor = selected;
            isSelected = true;
            Status.NoteSize = noteSize;
            return;
        }

        public void SetUnSelected()
        {
            BackColor = unSelected;
            isSelected = false;
            return;
        }

        private void PreviewBox_Paint(object sender, PaintEventArgs e)
        {
            using(SolidBrush brush = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(brush, previewBox.ClientRectangle);
            }
            //
            //TODO: このへんでノーツ画像描画？
            //
            if (isMouseEnter && isSelected)
            {
                Color guideColor = Color.FromArgb(150, 255, 255, 255);
                using (Pen pen = new Pen(guideColor))
                {
                    if(noteType != NoteType.AIRHOLD)
                    {
                        e.Graphics.DrawLine(pen, new Point(0, virtualButtonWeight), new Point(previewBox.Width, virtualButtonWeight));
                        e.Graphics.DrawLine(pen, new Point(0, previewBox.Height - virtualButtonWeight), new Point(previewBox.Width, previewBox.Height - virtualButtonWeight));
                    }
                    //HACK: Defineの順番を信じてこのコードは動いているのでそこのところ注意
                    if(noteType >= NoteType.AIRUPC && noteType <= NoteType.AIRDOWNR)
                    {
                        e.Graphics.DrawLine(pen, new Point(virtualButtonWeight, virtualButtonWeight), new Point(virtualButtonWeight, previewBox.Height - virtualButtonWeight));
                        e.Graphics.DrawLine(
                            pen,
                            new Point(previewBox.Width - virtualButtonWeight, virtualButtonWeight), 
                            new Point(previewBox.Width - virtualButtonWeight, previewBox.Height - virtualButtonWeight)
                            );
                    }
                }
                using (SolidBrush brush = new SolidBrush(guideColor))
                {
                    if (noteType >= NoteType.AIRUPC && noteType <= NoteType.AIRDOWNR)
                    {
                        if(buttonArea == ButtonArea.Center)
                        {
                            e.Graphics.FillRectangle(brush, VirtualButtonRect.Center);
                        }
                        else if(buttonArea == ButtonArea.Left)
                        {
                            e.Graphics.FillRectangle(brush, VirtualButtonRect.Left);
                        }
                        else if(buttonArea == ButtonArea.Right)
                        {
                            e.Graphics.FillRectangle(brush, VirtualButtonRect.Right);
                        }
                    }
                    else if(noteType != NoteType.AIRHOLD)
                    {
                        if(buttonArea == ButtonArea.Top)
                        {
                            e.Graphics.FillRectangle(brush, VirtualButtonRect.Top);
                        }
                        else if(buttonArea == ButtonArea.Bottom)
                        {
                            e.Graphics.FillRectangle(brush, VirtualButtonRect.Bottom);
                        }
                    }
                }
            }
            return;
        }
    }
}
