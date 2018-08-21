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

        /// <summary>
        /// 描画する
        /// </summary>
        /// <param name="e">描画対象</param>
        /// <param name="drawPosX">描画位置の右上のX座標</param>
        /// <param name="drawPosY">描画位置の右上のY座標</param>
        /// <param name="range">描画するScoreの範囲</param>
        public void PaintScore(PaintEventArgs e, double drawPosX, double drawPosY, Range range)
        {
            //主線の色情報
            Color laneMain = Color.FromArgb(180, 255, 255, 255);
            //副線の色情報
            Color laneSub = Color.FromArgb(80, 255, 255, 255);
            //レーンを区切る縦線を描画
            for (int i = 0; i <= ScoreInfo.Lanes; ++i)
            {
                if(i % 2 != 0)
                {
                    //副線の描画
                    e.Graphics.DrawLine(
                    new Pen(laneSub, 1),
                    (float)(drawPosX + i * ScoreInfo.LaneWidth),
                    (float)drawPosY,
                    (float)(drawPosX + i * ScoreInfo.LaneWidth),
                    (float)(drawPosY + height * range.Size() / beatNumer)
                    );
                }
                else
                {
                    //主線の描画
                    e.Graphics.DrawLine(
                    new Pen(laneMain, 1),
                    (float)(drawPosX + i * ScoreInfo.LaneWidth),
                    (float)drawPosY,
                    (float)(drawPosX + i * ScoreInfo.LaneWidth),
                    (float)(drawPosY + height * range.Size() / beatNumer)
                    );
                }
            }
            //指定範囲の開始が1拍目か判定
            if(range.Inf == 1)
            {
                //1拍目に小節開始の黄色線を描画
                e.Graphics.DrawLine(
                    new Pen(Color.Yellow, 1),
                    (float)drawPosX,
                    (float)(drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight * barSize * range.Size() / beatNumer),
                    (float)(drawPosX + ScoreInfo.Lanes * ScoreInfo.LaneWidth),
                    (float)(drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight * barSize * range.Size() / beatNumer)
                    );
            }
            else
            {
                //白線を描画
                e.Graphics.DrawLine(
                    new Pen(laneMain, 1),
                    (float)drawPosX,
                    (float)(drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight * barSize * range.Size() / beatNumer),
                    (float)(drawPosX + ScoreInfo.Lanes * ScoreInfo.LaneWidth),
                    (float)(drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight * barSize * range.Size() / beatNumer)
                    );
            }
            //拍子分母の間隔で白線を描画
            for(int i = 0; i < range.Size(); ++i)
            {
                e.Graphics.DrawLine(
                    new Pen(laneMain, 1),
                    (float)drawPosX,
                    (float)(drawPosY + i * ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight / beatDenom),
                    (float)(drawPosX + ScoreInfo.Lanes * ScoreInfo.LaneWidth),
                    (float)(drawPosY + i * ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight / beatDenom)
                    );
            }
        }
    }
}
