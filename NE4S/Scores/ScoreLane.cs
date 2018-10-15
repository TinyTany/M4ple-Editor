﻿using NE4S.Notes;
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
		private RectangleF drawRegion, hitRect;
        private List<ScoreMaterial> scoreMaterialList;

        /// <summary>
        /// ScoreLaneに入っているScoreたちの総サイズ
        /// </summary>
        public float CurrentBarSize
        {
            get { return currentBarSize; }
        }

		// <summary>
		/// ScoreLane当たり判定領域
		/// PictureBoxの左上を原点としたときの座標
		/// index,currentBarSizeが変わると変わる
		/// </summary>
		public RectangleF HitRect
		{
			get { return hitRect; }
		}

		private void refreshRects()
		{
			hitRect.Size = new SizeF(
				scoreWidth,
				ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * currentBarSize);
			hitRect.Location = new PointF(
				index * (Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right) + ScoreInfo.PanelMargin.Left + Margin.Left,
				//HACK: 当たり判定の最上部のピクセル座標を調節のため高さに+1をする（iピクセル下げる）
				ScoreInfo.PanelMargin.Top + Height - Margin.Bottom - hitRect.Height + 1);
			drawRegion.Size = new SizeF(
				Margin.Left + scoreWidth + Margin.Right,
				Margin.Top + maxScoreHeight + Margin.Bottom);
			drawRegion.Location = new PointF(
				index * (Width + ScoreInfo.PanelMargin.Left + ScoreInfo.PanelMargin.Right) + ScoreInfo.PanelMargin.Left,
				ScoreInfo.PanelMargin.Top);
		}

		private void refreshScoreMaterialList()
		{
			//Scoreの当たり判定を更新する
			float sumHeight = 0;
			//当たり判定が縦に1ピクセル分ずれているので調整用の変数を作成
			int normalizeDeltaHeight = 1;
			foreach (ScoreMaterial sMaterial in scoreMaterialList)
			{
				sMaterial.HitRect = new RectangleF(
					hitRect.X,
					ScoreInfo.PanelMargin.Top + Height - Margin.Bottom - sumHeight - sMaterial.Height + normalizeDeltaHeight,
					sMaterial.Width,
					sMaterial.Height);
				sumHeight += sMaterial.Height;
			}
		}

		public int Index
        {
            get { return index; }
            set { index = value; refreshRects(); refreshScoreMaterialList(); }
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
            scoreMaterialList = new List<ScoreMaterial>();
            currentBarSize = 0;
            hitRect = new RectangleF();
			drawRegion = new RectangleF();
            index = -1;
        }

        /// <summary>
        /// scoreがレーンに含まれているか判定
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool Contains(Score score)
        {
            if (scoreMaterialList.Find(x => x.Score.Equals(score)) != null) return true;
            else return false;
        }

        /// <summary>
        /// 指定したScore全体がこのレーン内に完全に含まれているか判定
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool IsScoreClose(Score score)
        {
            if (scoreMaterialList.Find(x => x.Score.Equals(score)).Range.Size() == score.BeatNumer) return true;
            else return false;
        }

        /// <summary>
        /// tScoresに要素が含まれているか判定
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return scoreMaterialList.Any();
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
				float physicalHeight = newScore.Height * newRange.Size() / newScore.BeatNumer;
				//当たり判定が縦に1ピクセル分ずれているので調整用の変数を作成
				int normalizeDeltaHeight = 1;
				//Scoreの当たり判定矩形を作成
				RectangleF newScoreHitRect = new RectangleF(
					hitRect.X,
                    ScoreInfo.PanelMargin.Top + Height - Margin.Bottom - currentSumScoreHeight - physicalHeight + normalizeDeltaHeight,
					newScore.Width,
					physicalHeight);
                //各リストに新たなScoreとその範囲と当たり判定を格納
                scoreMaterialList.Add(new ScoreMaterial(newScore, newRange, newScoreHitRect));
                currentBarSize += newRange.Size() / (float)newScore.BeatDenom;
				//HACK :currentBarSizeの値が変わるときに呼ぶ
				//あんまり良くないので改善したい
				refreshRects();
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
                currentBarSize -= scoreMaterialList.Find(x => x.Score.Equals(score)).Range.Size() / (float)score.BeatDenom;
				//HACK :currentBarSizeの値が変わるときに呼ぶ
				//あんまり良くないので改善したい
				refreshRects();
				//
                scoreMaterialList.Remove(scoreMaterialList.Find(x => x.Score.Equals(score)));
				refreshScoreMaterialList();
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
            if(scoreMaterialList.Any()) return scoreMaterialList.First().Score;
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
            if (scoreMaterialList.Any()) return scoreMaterialList.First().Range;
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
			if(scoreMaterialList.Find(x => x.HitRect.Contains(pX, pY)) == null)
			{
				System.Diagnostics.Debug.WriteLine("SelectedScore(Point) : null");
				return null;
			}
			return scoreMaterialList.Find(x => x.HitRect.Contains(pX, pY)).Score;
        }

#if DEBUG
		/// <summary>
		/// 試作
		/// </summary>
		public Pos GetPos(int pX, int pY)
		{
            ScoreMaterial selectedScoreMaterial =
                scoreMaterialList.Find(x => x.HitRect.Contains(pX, pY));
			if (selectedScoreMaterial != null)
			{
				return selectedScoreMaterial.CalculatePos(pX, pY);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("GetPos(Point) : selectedTScore = null");
				return null;
			}
		}

        public Pos GetPos(Point p)
        {
            return GetPos(p.X, p.Y);
        }
#endif

		/// <summary>
		/// originPosXとoriginPosYは、ScorePanelでのcurrentPositionXと0が入る
		/// </summary>
		/// <param name="e"></param>
		/// <param name="originPosX"></param>
		/// <param name="originPosY"></param>
		public void PaintLane(PaintEventArgs e, int originPosX, int originPosY)
        {
			float drawPosX = drawRegion.X - originPosX;
			float drawPosY = drawRegion.Y - originPosY;
            //レーン背景を黒塗り
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, Width, Height));
            //Score描画用のY座標の初期座標を画面最下に設定
            float currentDrawPosY = drawPosY + Height - Margin.Bottom;
            //リスト内のScoreについてY座標を変更しながら描画
            foreach (ScoreMaterial material in scoreMaterialList)
            {
                currentDrawPosY -= material.Score.Height * material.Range.Size() / material.Score.BeatNumer;
                material.Score.PaintScore(e, drawPosX + Margin.Left, currentDrawPosY, material.Range);
#if DEBUG
				e.Graphics.DrawString(
					material.HitRect.Location.ToString(), new Font("MS UI Gothic", 10, FontStyle.Bold), Brushes.Red, new PointF(drawPosX + 10, currentDrawPosY + 10));
#endif
			}
            //tScoresの最後の要素のScoreが閉じているか判定
            if (scoreMaterialList.Any() && scoreMaterialList.Last().Range.Sup == scoreMaterialList.Last().Score.BeatNumer)
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
