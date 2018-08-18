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
        public static double Height { get; set; } = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * ScoreInfo.BarPerLane + Margin.Top + Margin.Bottom;
        public double CurrentBarSize { get; set; }
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
            CurrentBarSize = 0;
        }

        public void AddScore(Score newScore)
        {
            if(newScore != null) scores.Add(newScore);
        }

        public void PaintLane(PaintEventArgs e, int drawPosX, int drawPosY)
        {
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, (float)Width, (float)Height));
            double currentDrawPosY = drawPosY + Height - Margin.Bottom;
            e.Graphics.DrawLine(
                new Pen(Color.Yellow, 1),
                drawPosX + Margin.Left, (float)currentDrawPosY,
                (float)(drawPosX + Margin.Left + ScoreInfo.Lanes * ScoreInfo.LaneWidth), (float)currentDrawPosY
                );
            foreach (Score score in scores)
            {
                currentDrawPosY -= score.Height;
                score.PaintScore(e, drawPosX + Margin.Left, currentDrawPosY);
            }
        }
    }
}
