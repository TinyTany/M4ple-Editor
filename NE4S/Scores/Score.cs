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
    /// 譜面そのもの
    /// </summary>
    public class Score
    {
        private int beatNumer, beatDenom;
        private double width, height, barSize;

        public Score(int beatNumer, int beatDenom)
        {
            this.beatNumer = beatNumer;//非負整数
            this.beatDenom = beatDenom;//非負整数かつ2のべき乗のもの
            barSize = beatNumer / (double)beatDenom;
            width = ScoreInfo.LaneWidth * ScoreInfo.Lanes;
            height = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * barSize;
        }

        public double Width
        {
            get { return this.width; }
        }

        public double Height
        {
            get { return this.height; }
        }

        public int BeatNumer
        {
            get { return beatNumer; }
            set { beatNumer = value; }
        }

        public int BeatDenom
        {
            get { return beatDenom; }
            set { beatDenom = value; }
        }

        public double BarSize
        {
            get { return barSize; }
            set { barSize = value; }
        }

        public void PaintScore(PaintEventArgs e, double DrawPosX, double DrawPosY)
        {
            e.Graphics.DrawLine(
                new Pen(Color.Yellow, 1),
                (float)DrawPosX,
                (float)DrawPosY,
                (float)(DrawPosX + ScoreInfo.Lanes * ScoreInfo.LaneWidth),
                (float)DrawPosY
                );
            for(int i = 1; i < beatNumer; ++i)
            {
                e.Graphics.DrawLine(
                    new Pen(Color.White, 0.5f),
                    (float)DrawPosX,
                    (float)(DrawPosY + i * ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight / beatDenom),
                    (float)(DrawPosX + ScoreInfo.Lanes * ScoreInfo.LaneWidth),
                    (float)(DrawPosY + i * ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight / beatDenom)
                    );
            }
        }
    }
}
