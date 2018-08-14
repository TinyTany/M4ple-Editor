using NE4S.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NE4S.Scores
{
    /// <summary>
    /// 画面上で譜面を表示するための1本分の譜面レーン
    /// </summary>
    public class ScoreLane
    {
        public static double Width { get; set; }
        public static double Height { get; set; }
        private List<Score> scores;
        private List<Note> notes;

        struct Margin
        {
            public const int Top = 5, Bottom = 5, Left = 30, Right = 30;
        }

        public ScoreLane()
        {
            scores = new List<Score>();
            notes = new List<Note>();
            #if DEBUG
            Width = 100;
            Height = 100;
            #endif
        }

        public void PaintLane(PaintEventArgs e, int drawPosX, int drawPosY)
        {
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, (float)Width, (float)Height));
        }
    }
}
