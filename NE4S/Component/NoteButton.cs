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
        public static readonly int virtualButtonWidth = 20;
        private enum ButtonArea : int
        {
            None = 0, Top = 1, Bottom = 2, Left = 3, Right = 4, Center = 5
        }
        

        public NoteButton(int noteType, NoteButtonEventHandler handler)
        {
            InitializeComponent();
            previewBox.MouseDown += PreviewBox_MouseDown;
            previewBox.Paint += PreviewBox_Paint;
            UpdateSelectedNoteButton += handler;
            previewBox.MouseEnter += PreviewBox_MouseEnter;
            previewBox.MouseLeave += PreviewBox_MouseLeave;
            this.noteType = noteType;
            noteSize = Status.NoteSize;
            //
            Size = new Size(150, 100);
            previewBox.Size = new Size(Width - margin * 2, Height - margin * 2);
            previewBox.Location = new Point(margin - 1, margin - 1);
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
                ButtonArea buttonArea = GetClickedArea(e.Location);
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

        private ButtonArea GetClickedArea(Point location)
        {
            ButtonArea buttonArea = ButtonArea.None;
            if (location.Y <= virtualButtonWidth)
            {
                buttonArea = ButtonArea.Top;
            }
            else if(location.Y >= previewBox.Height - virtualButtonWidth)
            {
                buttonArea = ButtonArea.Bottom;
            }
            else if(location.X <= virtualButtonWidth)
            {
                buttonArea = ButtonArea.Left;
            }
            else if(location.X >= previewBox.Width - virtualButtonWidth)
            {
                buttonArea = ButtonArea.Right;
            }
            else
            {
                buttonArea = ButtonArea.Center;
            }
            return buttonArea;
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
                using (Pen pen = new Pen(Color.FromArgb(150, 255, 255, 255)))
                {
                    e.Graphics.DrawLine(pen, new Point(0, virtualButtonWidth), new Point(previewBox.Width, virtualButtonWidth));
                    e.Graphics.DrawLine(pen, new Point(0, previewBox.Height - virtualButtonWidth), new Point(previewBox.Width, previewBox.Height - virtualButtonWidth));
                    //HACK: Defineの順番を信じてこのコードは動いているのでそこのところ注意
                    if(noteType >= NoteType.AIRUPC && noteType <= NoteType.AIRDOWNR)
                    {
                        e.Graphics.DrawLine(pen, new Point(virtualButtonWidth, virtualButtonWidth), new Point(virtualButtonWidth, previewBox.Height - virtualButtonWidth));
                        e.Graphics.DrawLine(
                            pen,
                            new Point(previewBox.Width - virtualButtonWidth, virtualButtonWidth), 
                            new Point(previewBox.Width - virtualButtonWidth, previewBox.Height - virtualButtonWidth)
                            );
                    }
                }
            }
            return;
        }
    }
}
