using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization;

namespace NE4S.Scores
{
    /// <summary>
    /// 譜面そのもの
    /// </summary>
    [Serializable()]
    public class Score
    {
        /// <summary>
        /// 主線の色情報
        /// </summary>
        public static readonly Color laneMain = Color.FromArgb(120, 255, 255, 255);
        /// <summary>
        /// 副線の色情報
        /// </summary>
        public static readonly Color laneSub = Color.FromArgb(40, 255, 255, 255);
        /// <summary>
        /// 小節開始の色情報
        /// </summary>
        public static readonly Color measureBorder = Color.FromArgb(120, 255, 255, 0);
        public float Width { get; }
        public float Height { get; private set; }
        public int BeatNumer { get; set; }
        public int BeatDenom { get; set; }
        public float BarSize { get; set; }
        private int startTick, endTick;
        public int StartTick
        {
            get { return startTick; }
            set {
                startTick = value;
                endTick = startTick + (int)(BarSize * ScoreInfo.MaxBeatDiv) - 1;
            }
        }
        public int EndTick
        {
            get { return endTick; }
        }
        public int TickSize
        {
            get { return endTick - startTick + 1; }
        }

        /// <summary>
        /// この小節の拍子を描画するかどうかを表す
        /// </summary>
        public bool IsBeatVisible { get; set; }

        /// <summary>
        /// 小節番号
        /// 0スタート
        /// </summary>
        public int Index { get; set; }

        public int LinkCount { get; set; }

        public Score(int beatNumer, int beatDenom)
        {
            BeatNumer = beatNumer;//非負整数
            BeatDenom = beatDenom;//非負整数かつ2のべき乗のもの
            BarSize = beatNumer / (float)beatDenom;
            Width = ScoreInfo.UnitLaneWidth * ScoreInfo.Lanes;
            Height = ScoreInfo.UnitBeatHeight * ScoreInfo.MaxBeatDiv * BarSize;
            Index = -1;
            LinkCount = 0;
        }

        public void RefreshHeight()
        {
            Height = ScoreInfo.UnitBeatHeight * ScoreInfo.MaxBeatDiv * BarSize;
        }

        /// <summary>
        /// 描画する
        /// </summary>
        /// <param name="e">描画対象</param>
        /// <param name="drawPosX">描画位置の右上のX座標</param>
        /// <param name="drawPosY">描画位置の右上のY座標</param>
        /// <param name="range">描画するScoreの範囲</param>
        public void PaintScore(Graphics g, float drawPosX, float drawPosY, ScoreRange range)
        {
            //レーンを区切る縦線を描画
            for (int i = 0; i <= ScoreInfo.Lanes; ++i)
            {
                if(i % 2 != 0)
                {
                    //副線の描画
                    using (Pen myPen = new Pen(laneSub, 1))
                    {
                        g.DrawLine(
                        myPen,
                        drawPosX + i * ScoreInfo.UnitLaneWidth,
                        drawPosY,
                        drawPosX + i * ScoreInfo.UnitLaneWidth,
                        drawPosY + Height * range.Size / BeatNumer
                        );
                    }
                }
                else
                {
                    //主線の描画
                    using(Pen myPen = new Pen(laneMain, 1))
                    {
                        g.DrawLine(
                        myPen,
                        drawPosX + i * ScoreInfo.UnitLaneWidth,
                        drawPosY,
                        drawPosX + i * ScoreInfo.UnitLaneWidth,
                        drawPosY + Height * range.Size / BeatNumer
                        );
                    }
                }
            }
            //指定範囲の開始が1拍目か判定
            if(range.Min == 1)
            {
                //1拍目に小節開始の黄色線を描画
                using (Pen myPen = new Pen(measureBorder, 1))
                {
                    g.DrawLine(
                        myPen,
                        drawPosX,
                        drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight * BarSize * range.Size / BeatNumer,
                        drawPosX + ScoreInfo.Lanes * ScoreInfo.UnitLaneWidth,
                        drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight * BarSize * range.Size / BeatNumer
                        );
                }
                //小節数を描画
                using (Font myFont = new Font("MS UI Gothic", ScoreInfo.FontSize, FontStyle.Bold))
                {
                    // 小節数を描画
                    float dX = -23.5f, dY = -9;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(
                    (Index + 1).ToString().PadLeft(3, '0'),
                    myFont,
                    Brushes.White,
                    new PointF(
                        drawPosX + dX,
                        drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight * BarSize * range.Size / BeatNumer + dY));
                    // 必要に応じて拍数を描画
                    if (IsBeatVisible)
                    {
                        dX -= 30;
                        g.DrawString(
                        BeatNumer + "/" + BeatDenom,
                        myFont,
                        Brushes.Orange,
                        new PointF(
                        drawPosX + dX,
                        drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight * BarSize * range.Size / BeatNumer + dY));
                    }
                }
            }
            else
            {
                //白線を描画
                using (Pen myPen = new Pen(laneMain, 1))
                {
                    g.DrawLine(
                        myPen,
                        drawPosX,
                        drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight * BarSize * range.Size / BeatNumer,
                        drawPosX + ScoreInfo.Lanes * ScoreInfo.UnitLaneWidth,
                        drawPosY + ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight * BarSize * range.Size / BeatNumer
                        );
                }
            }
            //拍子分母の間隔で白線を描画
            for(int i = 0; i < range.Size; ++i)
            {
                using (Pen myPen = new Pen(laneMain, 1))
                {
                    g.DrawLine(
                        myPen,
                        drawPosX,
                        drawPosY + i * ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight / BeatDenom,
                        drawPosX + ScoreInfo.Lanes * ScoreInfo.UnitLaneWidth,
                        drawPosY + i * ScoreInfo.MaxBeatDiv * ScoreInfo.UnitBeatHeight / BeatDenom
                        );
                } 
            }
        }
    }
}
