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
        private double currentBarSize;
        private int laneIndex;
        private RectangleF hitRect;
        private List<Note> notes;
        private List<Tuple<Score, Range>> tScores;

        /// <summary>
        /// ScoreLaneに入っているScoreたちの総サイズ
        /// </summary>
        public double CurrentBarSize
        {
            get { return currentBarSize; }
        }

        /// <summary>
        /// ScoreLane当たり判定領域
        /// 絶対座標的な値
        /// </summary>
        public RectangleF HitRect
        {
            get { return hitRect; }
        }

        /// <summary>
        /// 範囲は[0, lanes.count - 1]
        /// </summary>
        public int LaneIndex
        {
            get { return laneIndex; }
            set { laneIndex = value; }
        }

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
            notes = new List<Note>();
            tScores = new List<Tuple<Score, Range>>();
            currentBarSize = 0;
            hitRect = new RectangleF();
        }

        /// <summary>
        /// scoreがレーンに含まれているか判定
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool Contains(Score score)
        {
            if (tScores.Find(x => x.Item1.Equals(score)) != null) return true;
            else return false;
        }

        /// <summary>
        /// 指定したScore全体がこのレーン内に完全に含まれているか判定
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool IsScoreClose(Score score)
        {
            if (tScores.Find(x => x.Item1.Equals(score)).Item2.Size() == score.BeatNumer) return true;
            else return false;
        }

        /// <summary>
        /// tScoresに要素が含まれているか判定
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return tScores.Any();
        }

        /// <summary>
        /// ScoreLaneに新たなScoreとそのRegionを追加
        /// </summary>
        /// <param name="newScore">追加するScore</param>
        /// <param name="newRange">追加するRegion</param>
        public void AddScore(Score newScore, Range newRange)
        {
            if (newScore != null && newRange != null)
            {
                //各リストに新たなScoreとその範囲を格納
                tScores.Add(new Tuple<Score, Range>(newScore, newRange));
                currentBarSize += newRange.Size() / (double)newScore.BeatDenom;
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("AddScore失敗");
#endif
            }
        }

        /// <summary>
        /// 新たなScoreの全体を追加
        /// </summary>
        /// <param name="newScore"></param>
        public void AddScore(Score newScore)
        {
            AddScore(newScore, new Range(1, newScore.BeatNumer));
        }

        /// <summary>
        /// 指定されたScoreを削除
        /// </summary>
        /// <param name="score">削除対象のScore</param>
        public void DeleteScore(Score score)
        {
            if (score != null && Contains(score))
            {
                currentBarSize -= tScores.Find(x => x.Item1.Equals(score)).Item2.Size() / (double)score.BeatDenom;
                tScores.Remove(tScores.Find(x => x.Item1.Equals(score)));
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("DeleteScore失敗");
#endif
            }
        }

        public Score BeginScore()
        {
            if(tScores.Any()) return tScores.First().Item1;
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("tScoresは空です");
#endif
                return null;
            }
        }

        public Range BeginRange()
        {
            if (tScores.Any()) return tScores.First().Item2;
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("tScoresは空です");
#endif
                return null;
            }
        }

        /// <summary>
        /// レーンの当たり判定を設定
        /// laneIndexに依存
        /// </summary>
        public void UpdateHitRect()
        {
            hitRect.Size = new SizeF((float)Width, (float)(Height * currentBarSize / ScoreInfo.LaneMaxBar));
            hitRect.Location = 
                new PointF(
                    (float)(laneIndex * (Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right) + ScoreInfo.PanelMargin.Left),
                    (float)(Height - hitRect.Size.Height + ScoreInfo.PanelMargin.Top));
        }

        /// <summary>
        /// クリックされたScoreを特定
        /// </summary>
        /// <param name="e">クリックされたマウス情報</param>
        /// <returns>クリックされたScore</returns>
        public Score SelectedScore(MouseEventArgs e)
        {
            Score selectedScore = null;
            double posY = Height - Margin.Bottom;
            foreach(Tuple<Score, Range> tScore in tScores)
            {
                if(posY - tScore.Item1.Height * tScore.Item2.Size() / (double)tScore.Item1.BeatNumer < e.Y - ScoreInfo.PanelMargin.Top && 
                    e.Y - ScoreInfo.PanelMargin.Top <= posY)
                {
                    selectedScore = tScore.Item1;
                    break;
                }
                else
                {
                    posY -= tScore.Item1.Height * tScore.Item2.Size() / (double)tScore.Item1.BeatNumer;
                }
            }
            return selectedScore;
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
            //Score描画用のY座標の初期座標を画面最下に設定
            double currentDrawPosY = drawPosY + Height - Margin.Bottom;
            //リスト内のScoreについてY座標を変更しながら描画
            foreach (Tuple<Score, Range> tScore in tScores)
            {
                currentDrawPosY -= tScore.Item1.Height * tScore.Item2.Size() / tScore.Item1.BeatNumer;
                tScore.Item1.PaintScore(e, drawPosX + Margin.Left, currentDrawPosY, tScore.Item2);
            }
            //tScoresの最後の要素のScoreが閉じているか判定
            if (tScores.Any() && tScores.Last().Item2.Sup == tScores.Last().Item1.BeatNumer)
            {
                //閉じていた場合
                //最後の小節を黄色線で閉じる
                e.Graphics.DrawLine(
                    new Pen(Color.Yellow, 1),
                    drawPosX + Margin.Left, (float)currentDrawPosY,
                    (float)(drawPosX + Margin.Left + ScoreInfo.Lanes * ScoreInfo.LaneWidth), (float)currentDrawPosY
                    );
            }
            //レーン上部の余白の部分は灰色(黒以外の色)に描画して未使用領域とする
            currentDrawPosY -= Margin.Top;
            //上部が余ってるか余ってない（ぴったり描画された）か判定
            if(currentDrawPosY > drawPosY)
            {
                //余ってる部分は塗りつぶす
                e.Graphics.FillRectangle(Brushes.White, new RectangleF(drawPosX, drawPosY, (float)Width, (float)(currentDrawPosY - drawPosY)));
            }
#if DEBUG
            e.Graphics.DrawString(laneIndex.ToString(), new Font("MS UI Gothic", 10, FontStyle.Bold), Brushes.Red, new PointF(drawPosX, drawPosY));
            e.Graphics.DrawString(hitRect.Location.ToString(), new Font("MS UI Gothic", 10, FontStyle.Bold), Brushes.Red, new PointF(drawPosX, drawPosY + 20));
#endif
        }
    }
}
