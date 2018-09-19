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
		private static float scoreWidth = ScoreInfo.Lanes * ScoreInfo.MinLaneWidth;
		private static float maxScoreHeight = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * ScoreInfo.LaneMaxBar;
        public static float Width { get; set; } = scoreWidth + Margin.Left + Margin.Right;
        public static float Height { get; set; } = maxScoreHeight + Margin.Top + Margin.Bottom;
        private float currentBarSize;
        private int index;
        private RectangleF hitRect;
        private List<Note> notes;
        private List<Tuple<Score, Range, RectangleF>> tScores;

        /// <summary>
        /// ScoreLaneに入っているScoreたちの総サイズ
        /// </summary>
        public double CurrentBarSize
        {
            get { return currentBarSize; }
        }

        /// <summary>
        /// ScoreLane当たり判定領域
        /// PictureBoxの左上を原点としたときの座標
        /// </summary>
        public RectangleF HitRect
        {
            get {
                hitRect.Size = new SizeF(
					scoreWidth, 
					ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * currentBarSize);
                hitRect.Location = new PointF(
                    index * (Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right) + ScoreInfo.PanelMargin.Left + Margin.Left,
                    ScoreInfo.PanelMargin.Top + Height - Margin.Bottom - hitRect.Height);
                return hitRect; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
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
            tScores = new List<Tuple<Score, Range, RectangleF>>();
            currentBarSize = 0;
            hitRect = new RectangleF();
            index = -1;
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
                float currentSumScoreHeight = ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight * currentBarSize;
				//Scoreの当たり判定矩形を作成
				RectangleF newScoreHitRect = new RectangleF(
					HitRect.X,
                    //HACK: マジックナンバーを使っているのであとで改善する。
                    ScoreInfo.PanelMargin.Top + Height - Margin.Bottom  - currentSumScoreHeight - newScore.Height + 1,
					newScore.Width,
					newScore.Height);
                //各リストに新たなScoreとその範囲と当たり判定を格納
                tScores.Add(new Tuple<Score, Range, RectangleF>(newScore, newRange, newScoreHitRect));
                currentBarSize += newRange.Size() / (float)newScore.BeatDenom;
                //
                newScore.LinkCount++;
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
                currentBarSize -= tScores.Find(x => x.Item1.Equals(score)).Item2.Size() / (float)score.BeatDenom;
                tScores.Remove(tScores.Find(x => x.Item1.Equals(score)));
				//Scoreの当たり判定を更新する
				float sumHeight = 0;
				for(int i = 0; i < tScores.Count; ++i)
				{
					tScores[i] = new Tuple<Score, Range, RectangleF>(
						tScores[i].Item1,
						tScores[i].Item2,
						new RectangleF(HitRect.X, maxScoreHeight - sumHeight - tScores[i].Item1.Height, tScores[i].Item1.Width, tScores[i].Item1.Height));
					sumHeight += tScores[i].Item1.Height;
				}
				//
                score.LinkCount--;
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
                System.Diagnostics.Debug.WriteLine("BeginScore() : tScoresは空です");
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
                System.Diagnostics.Debug.WriteLine("BeginRange() : tScoresは空です");
#endif
                return null;
            }
        }

        /// <summary>
        /// クリックされたScoreを特定
        /// </summary>
        /// <param name="e">クリックされたマウス情報</param>
        /// <returns>クリックされたScore</returns>
        public Score SelectedScore(int pX, int pY)
        {
			//FIX: エラーが出るから修正する。クリックに対して正しくScoreを検索できていない。
			if(tScores.Find(x => x.Item3.Contains(pX, pY)) == null)
			{
				System.Diagnostics.Debug.WriteLine("SelectedScore(Point) : null");
				return null;
			}
			return tScores.Find(x => x.Item3.Contains(pX, pY)).Item1;
        }

#if DEBUG
		/// <summary>
		/// 試作
		/// </summary>
		public void GetPos(int pX, int pY)
		{
            Tuple<Score, Range, RectangleF> selectedTScore =
                tScores.Find(x => x.Item3.Contains(pX, pY));
			if (selectedTScore != null)
			{
				Point normalizedP = new Point(
                    pX - (int)selectedTScore.Item3.X,
                    (int)selectedTScore.Item1.Height - (pY - (int)selectedTScore.Item3.Y));
				selectedTScore.Item1.CalculatePos(normalizedP);
			}
            if (selectedTScore == null) System.Diagnostics.Debug.WriteLine("GetPos(Point) : selectedTScore = null");
		}

        public void GetPos(Point p)
        {
            GetPos(p.X, p.Y);
        }
#endif

		/// <summary>
		/// 描画する
		/// </summary>
		/// <param name="e">描画対象</param>
		/// <param name="drawPosX">描画位置の右上のX座標</param>
		/// <param name="drawPosY">描画位置の右上のY座標</param>
		public void PaintLane(PaintEventArgs e, int drawPosX, int drawPosY)
        {
            //レーン背景を黒塗り
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, Width, Height));
            //Score描画用のY座標の初期座標を画面最下に設定
            float currentDrawPosY = drawPosY + Height - Margin.Bottom;
            //リスト内のScoreについてY座標を変更しながら描画
            foreach (Tuple<Score, Range, RectangleF> tScore in tScores)
            {
                currentDrawPosY -= tScore.Item1.Height * tScore.Item2.Size() / tScore.Item1.BeatNumer;
                tScore.Item1.PaintScore(e, drawPosX + Margin.Left, currentDrawPosY, tScore.Item2);
            }
            //tScoresの最後の要素のScoreが閉じているか判定
            if (tScores.Any() && tScores.Last().Item2.Sup == tScores.Last().Item1.BeatNumer)
            {
                //閉じていた場合
                //最後の小節を黄色線で閉じる
                using (Pen myPen = new Pen(Color.Yellow, 1))
                {
                    e.Graphics.DrawLine(
                        myPen,
                        drawPosX + Margin.Left, currentDrawPosY,
                        drawPosX + Margin.Left + scoreWidth, currentDrawPosY
                        );
                }
            }
            //レーン上部の余白の部分は灰色(黒以外の色)に描画して未使用領域とする
            currentDrawPosY -= Margin.Top;
            //上部が余ってるか余ってない（ぴったり描画された）か判定
            if(currentDrawPosY > drawPosY)
            {
                //余ってる部分は塗りつぶす
                e.Graphics.FillRectangle(Brushes.LightGray, new RectangleF(drawPosX, drawPosY, Width, currentDrawPosY - drawPosY));
            }
#if DEBUG
            e.Graphics.DrawString(Index.ToString(), new Font("MS UI Gothic", 10, FontStyle.Bold), Brushes.Red, new PointF(drawPosX, drawPosY));
            e.Graphics.DrawString(hitRect.Location.ToString(), new Font("MS UI Gothic", 10, FontStyle.Bold), Brushes.Red, new PointF(drawPosX, drawPosY + 20));
#endif
        }
    }
}
