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
        public static double Width { get; set; } = ScoreInfo.Lanes * ScoreInfo.LaneWidth + Margin.Left + Margin.Right;
        public static double Height { get; set; } = ScoreInfo.LaneHeight * ScoreInfo.MaxBeatDiv * ScoreInfo.BarPerLane;
        private List<Score> scores;
        private List<Note> notes;

        class Margin
        {
            public static int 
                Top = ScoreInfo.LaneMargin.Top,
                Bottom = ScoreInfo.LaneMargin.Bottom,
                Left = ScoreInfo.LaneMargin.Left,
                Right = ScoreInfo.LaneMargin.Right;
        }

        public ScoreLane()
        {
            scores = new List<Score>();
            notes = new List<Note>();
        }

        public void PaintLane(PaintEventArgs e, int drawPosX, int drawPosY)
        {
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, (float)Width, (float)Height));
        }
    }
}
