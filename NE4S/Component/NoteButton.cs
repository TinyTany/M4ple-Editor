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
using NoteType = NE4S.Define.NoteType;

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
        public bool IsSelected { get; private set; } = false;

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
            #region ToolTip
            ToolTip toolTip = new ToolTip()
            {
                InitialDelay = 1000,
                ReshowDelay = 500,
                AutoPopDelay = 5000,
                ShowAlways = true
            };
            string tipStr = "Note";
            switch (noteType)
            {
                case NoteType.TAP:
                    tipStr = "Tap";
                    break;
                case NoteType.EXTAP:
                    tipStr = "ExTap";
                    break;
                case NoteType.AWEXTAP:
                    tipStr = "AwesomeExTap";
                    break;
                case NoteType.EXTAPDOWN:
                    tipStr = "ExTapDown";
                    break;
                case NoteType.FLICK:
                    tipStr = "Flick";
                    break;
                case NoteType.HELL:
                    tipStr = "Damage";
                    break;
                case NoteType.HOLD:
                    tipStr = "Hold";
                    break;
                case NoteType.SLIDE:
                    tipStr = "Slide";
                    break;
                case NoteType.SLIDECURVE:
                    tipStr = "SlideCurve";
                    break;
                case NoteType.AIRUPL:   
                case NoteType.AIRUPC:
                case NoteType.AIRUPR:
                    tipStr = "AirUp";
                    break;
                case NoteType.AIRDOWNL:
                case NoteType.AIRDOWNC:
                case NoteType.AIRDOWNR:
                    tipStr = "AirDown";
                    break;
                case NoteType.AIRHOLD:
                    tipStr = "AirHold/AirAction";
                    break;
                case NoteType.BPM:
                    tipStr = "BPM";
                    break;
                case NoteType.HIGHSPEED:
                    tipStr = "HighSpeed";
                    break;
                default:
                    break;
            }
            toolTip.SetToolTip(previewBox, tipStr);
            #endregion
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
            IsSelected = true;
            return;
        }

        public virtual void SetUnSelected()
        {
            BackColor = unSelected;
            IsSelected = false;
            return;
        }

        protected virtual void PreviewBox_Paint(object sender, PaintEventArgs e)
        {
            using(SolidBrush brush = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(brush, previewBox.ClientRectangle);
            }
            //ノーツ画像描画
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            DrawNote(e);
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
                    Hold.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f - 25), new SizeF(70, 50));
                    HoldBegin.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.SLIDE:
                    Slide.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f - 25), new SizeF(70, 50));
                    SlideBegin.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                case NoteType.SLIDECURVE:
                    SlideCurve.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(80, 10));
                    break;
                #region Air系の描画
                //HACK: いろいろとコードがガバガバなので後で直してもええんやで...
                case NoteType.AIRUPL:
                    new AirUpL(8, new Position(), new PointF(previewBox.Width / 2f - 4 * 12, previewBox.Height / 2f + 15), -1).Draw(e.Graphics, new Point());
                    break;
                case NoteType.AIRUPC:
                    new AirUpC(8, new Position(), new PointF(previewBox.Width / 2f - 4 * 12, previewBox.Height / 2f + 15), -1).Draw(e.Graphics, new Point());
                    break;
                case NoteType.AIRUPR:
                    new AirUpR(8, new Position(), new PointF(previewBox.Width / 2f - 4 * 12, previewBox.Height / 2f + 15), -1).Draw(e.Graphics, new Point());
                    break;
                case NoteType.AIRDOWNL:
                    new AirDownL(8, new Position(), new PointF(previewBox.Width / 2f - 4 * 12, previewBox.Height / 2f + 15), -1).Draw(e.Graphics, new Point());
                    break;
                case NoteType.AIRDOWNC:
                    new AirDownC(8, new Position(), new PointF(previewBox.Width / 2f - 4 * 12, previewBox.Height / 2f + 15), -1).Draw(e.Graphics, new Point());
                    break;
                case NoteType.AIRDOWNR:
                    new AirDownR(8, new Position(), new PointF(previewBox.Width / 2f - 4 * 12, previewBox.Height / 2f + 15), -1).Draw(e.Graphics, new Point());
                    break;
                #endregion
                case NoteType.AIRHOLD:
                    AirHold.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f + 25), new SizeF(7, 50));
                    AirAction.Draw(e, new PointF(previewBox.Width / 2f, previewBox.Height / 2f), new SizeF(78, 8));
                    break;
                // HACK: 色や座標に即値を使っているのでよくない
                case NoteType.BPM:
                    e.Graphics.DrawLine(
                        Pens.LimeGreen, 
                        (previewBox.Width - 100) / 2f, 
                        previewBox.Height / 2f, 
                        (previewBox.Width + 100) / 2f, 
                        previewBox.Height / 2f);
                    break;
                // HACK: 同上
                case NoteType.HIGHSPEED:
                    e.Graphics.DrawLine(
                        Pens.Red,
                        (previewBox.Width - 100) / 2f,
                        previewBox.Height / 2f,
                        (previewBox.Width + 100) / 2f,
                        previewBox.Height / 2f);
                    break;
                default:
                    break;
            }
        }
    }
}
