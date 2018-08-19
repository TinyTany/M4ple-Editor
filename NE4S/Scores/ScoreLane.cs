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
        public static double Height { get; set; } = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * ScoreInfo.LaneMaxBar + Margin.Top + Margin.Bottom;
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

        /// <summary>
        /// 描画する
        /// </summary>
        /// <param name="e">描画対象</param>
        /// <param name="drawPosX">描画位置の右上のX座標</param>
        /// <param name="drawPosY">描画位置の右上のY座標</param>
        public void PaintLane(PaintEventArgs e, int drawPosX, int drawPosY)
        {
            //レーン背景を黒塗り
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, (float)Width, (float)Height));
            //Score描画用のY座標の初期座標を最下に設定
            double currentDrawPosY = drawPosY + Height - Margin.Bottom;
            //リスト内のScoreについてY座標を変更しながら描画
            foreach (Score score in scores)
            {
                currentDrawPosY -= score.Height;
                score.PaintScore(e, drawPosX + Margin.Left, currentDrawPosY);
            }
            //最後の小節を黄色線で閉じる
            e.Graphics.DrawLine(
                new Pen(Color.Yellow, 1),
                drawPosX + Margin.Left, (float)currentDrawPosY,
                (float)(drawPosX + Margin.Left + ScoreInfo.Lanes * ScoreInfo.LaneWidth), (float)currentDrawPosY
                );
            //レーン上部の余白の部分は灰色(黒以外の色)に描画して未使用領域とする
            currentDrawPosY -= Margin.Top;
            //上部が余ってるか余ってない（ぴったり描画された）か判定
            if(currentDrawPosY > drawPosY)
            {
                e.Graphics.FillRectangle(Brushes.White, new RectangleF(drawPosX, drawPosY, (float)Width, (float)(currentDrawPosY - drawPosY)));
            }
        }
    }
}
