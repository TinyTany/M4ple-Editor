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
using System.Drawing.Drawing2D;
using NE4S.Notes;

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
            //
            AutoScaleMode = AutoScaleMode.None;
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
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            DrawNote(e);
            //
            return;
        }

        private void DrawNote(PaintEventArgs e)
        {
            switch (noteType)
            {
                case NoteType.TAP:
                    Tap.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.EXTAP:
                    ExTap.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.EXTAPDOWN:
                    ExTapDown.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.AWEXTAP:
                    AwesomeExTap.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.FLICK:
                    Flick.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.HELL:
                    HellTap.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.HOLD:
                    HoldBegin.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.SLIDE:
                    SlideBegin.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.SLIDECURVE:
                    SlideCurve.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.AIRUPL:
                    break;
                case NoteType.AIRUPC:
                    break;
                case NoteType.AIRUPR:
                    break;
                case NoteType.AIRDOWNL:
                    break;
                case NoteType.AIRDOWNC:
                    break;
                case NoteType.AIRDOWNR:
                    break;
                case NoteType.AIRHOLD:
                    AirAction.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(78, 8));
                    break;
                default:
                    break;
            }
        }
    }
}
