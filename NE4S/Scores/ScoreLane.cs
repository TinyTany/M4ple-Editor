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
    [Serializable()]
    public class ScoreLane
    {
		public static readonly float scoreWidth = ScoreInfo.Lanes * ScoreInfo.MinLaneWidth;
		private static float maxScoreHeight = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * ScoreInfo.LaneMaxBar;
        public static float Width { get; set; } = scoreWidth + Margin.Left + Margin.Right;
        public static float Height { get; set; } = maxScoreHeight + Margin.Top + Margin.Bottom;
        private int index;
		private RectangleF drawRegion;
        private List<ScoreMaterial> scoreMaterialList;

        /// <summary>
        /// ScoreLaneに入っているScoreたちの総サイズ
        /// </summary>
        public float CurrentBarSize { get; private set; }

        // <summary>
        /// ScoreLane当たり判定領域
        /// PictureBoxの左上を原点としたときの座標
        /// index,currentBarSizeが変わると変わる
        /// </summary>
        public RectangleF HitRect { get; private set; }

        public RectangleF LaneRect { get; private set; }

        public static void RefreshLaneSize()
        {
            maxScoreHeight = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * ScoreInfo.LaneMaxBar;
            Height = maxScoreHeight + Margin.Top + Margin.Bottom;
        }

        /// <summary>
        /// レーンの当たり判定矩形と描画領域矩形を更新
        /// </summary>
		private void RefreshRects()
		{
			SizeF hitRectSize = new SizeF(
				scoreWidth + Margin.Left + Margin.Right,
				ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv * CurrentBarSize);
			PointF hitRectLocation = new PointF(
				index * (Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right) + ScorePanel.Margin.Left,
				//HACK: 当たり判定の最上部のピクセル座標を調節のため高さに+1をする（1ピクセル下げる）
				ScorePanel.Margin.Top + Height - Margin.Bottom - hitRectSize.Height + 1);
            HitRect = new RectangleF(hitRectLocation, hitRectSize);
			drawRegion.Size = new SizeF(
				Margin.Left + scoreWidth + Margin.Right,
				Margin.Top + maxScoreHeight + Margin.Bottom);
			drawRegion.Location = new PointF(
				index * (Width + ScorePanel.Margin.Left + ScorePanel.Margin.Right) + ScorePanel.Margin.Left,
				ScorePanel.Margin.Top);
            LaneRect = new RectangleF(HitRect.X + Margin.Left, HitRect.Y, scoreWidth, HitRect.Height);
		}

        /// <summary>
        /// ScoreMaterialの当たり判定矩形を更新
        /// </summary>
		private void RefreshScoreMaterialList()
		{
			//Scoreの当たり判定を更新する
			float sumHeight = 0;
			//当たり判定が縦に1ピクセル分ずれているので調整用の変数を作成
			int normalizeDeltaHeight = 1;
			foreach (ScoreMaterial sMaterial in scoreMaterialList)
			{
				sMaterial.HitRect = new RectangleF(
					HitRect.X + Margin.Left,
					ScorePanel.Margin.Top + Height - Margin.Bottom - sumHeight - sMaterial.Height + normalizeDeltaHeight,
					sMaterial.Width,
					sMaterial.Height);
				sumHeight += sMaterial.Height;
			}
		}

		public int Index
        {
            get { return index; }
            set { index = value; RefreshRects(); RefreshScoreMaterialList(); }
        }

        /// <summary>
        /// このレーンに対するStartTickを取得します
        /// </summary>
        public int StartTick
        {
            get
            {
                if (!scoreMaterialList.Any())
                {
                    return -1;
                }
                else
                {
                    return scoreMaterialList.First().StartTick;
                }
            }
        }

        /// <summary>
        /// このレーンに対するEndTickを取得します
        /// </summary>
        public int EndTick
        {
            get
            {
                if (!scoreMaterialList.Any())
                {
                    return -1;
                }
                else
                {
                    return scoreMaterialList.Last().EndTick;
                }
            }
        }

        public static class Margin
        {
            public static readonly int
                Top = 5,
                Bottom = 5,
                Left = 60,
                Right = 60;
        }

        public ScoreLane()
        {
            scoreMaterialList = new List<ScoreMaterial>();
            CurrentBarSize = 0;
            HitRect = new RectangleF();
            LaneRect = new RectangleF();
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
        /// noteがレーンに含まれているか判定
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool Contains(Note note)
        {
            if(scoreMaterialList.Find(
                x => 
                x.StartTick <= note.Position.Tick &&
                note.Position.Tick <= x.EndTick) != null)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 指定したScore全体がこのレーン内に完全に含まれているか判定
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool IsScoreClose(Score score)
        {
            if (scoreMaterialList.Find(x => x.Score.Equals(score)).Range.Size == score.BeatNumer) return true;
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
                float currentSumScoreHeight = ScoreInfo.MaxBeatDiv * ScoreInfo.MaxBeatHeight * CurrentBarSize;
				float physicalHeight = newScore.Height * newRange.Size / newScore.BeatNumer;
				//当たり判定が縦に1ピクセル分ずれているので調整用の変数を作成
				int normalizeDeltaHeight = 1;
				//Scoreの当たり判定矩形を作成
				RectangleF newScoreHitRect = new RectangleF(
					HitRect.X + Margin.Left,
                    ScorePanel.Margin.Top + Height - Margin.Bottom - currentSumScoreHeight - physicalHeight + normalizeDeltaHeight,
					newScore.Width,
					physicalHeight);
                //各リストに新たなScoreとその範囲と当たり判定を格納
                scoreMaterialList.Add(new ScoreMaterial(newScore, newRange, newScoreHitRect));
                CurrentBarSize += newRange.Size / (float)newScore.BeatDenom;
				//HACK :currentBarSizeの値が変わるときに呼ぶ
				//あんまり良くないので改善したい
				RefreshRects();
                //
                newScore.LinkCount++;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "AddScore失敗");
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
                CurrentBarSize -= scoreMaterialList.Find(x => x.Score.Equals(score)).Range.Size / (float)score.BeatDenom;
				//HACK :currentBarSizeの値が変わるときに呼ぶ
				//あんまり良くないので改善したい
				RefreshRects();
				//
                scoreMaterialList.Remove(scoreMaterialList.Find(x => x.Score.Equals(score)));
				RefreshScoreMaterialList();
                score.LinkCount--;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "DeleteScore失敗");
            }
        }

        public Score FirstScore
        {
            get
            {
                if (scoreMaterialList.Any()) return scoreMaterialList.First().Score;
                else
                {
                    System.Diagnostics.Debug.Assert(false, "FirstScore() : tScoresは空です");
                    return null;
                }
            }
        }

        public Score LastScore
        {
            get
            {
                if (scoreMaterialList.Any()) return scoreMaterialList.Last().Score;
                else
                {
                    System.Diagnostics.Debug.Assert(false, "FirstScore() : tScoresは空です");
                    return null;
                }
            }            
        }

        public Range FirstRange
        {
            get
            {
                if (scoreMaterialList.Any()) return scoreMaterialList.First().Range;
                else
                {
                    System.Diagnostics.Debug.Assert(false, "FirstRange() : tScoresは空です");
                    return null;
                }
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

        public Score SelectedScore(Point location)
        {
            return SelectedScore(location.X, location.Y);
        }

		/// <summary>
		/// グリッド座標からそれに対応する相対座標を取得します
		/// </summary>
		public Position GetLocalPosition(PointF location)
		{
            if (location.X < HitRect.X + Margin.Left)
            {
                location.X = HitRect.X + Margin.Left;
            }
            else if (location.X > HitRect.Right - Margin.Right - 1)
            {
                location.X = HitRect.Right - Margin.Right - 1;
            }
            ScoreMaterial selectedScoreMaterial =
                scoreMaterialList.Find(x => x.HitRect.Contains(location));
			if (selectedScoreMaterial != null)
			{
                return selectedScoreMaterial.GetPosition(location);
			}
			else
			{
                System.Diagnostics.Debug.WriteLine( "ノーツの相対位置を計算できませんでした");
				return null;
			}
		}

		/// <summary>
		/// originPosXとoriginPosYは、ScorePanelでのcurrentPositionXと0が入る
		/// </summary>
		/// <param name="e"></param>
		/// <param name="originPosX"></param>
		/// <param name="originPosY"></param>
		public void PaintLane(PaintEventArgs e, Point originLocation)
        {
			float drawPosX = drawRegion.X - originLocation.X;
			float drawPosY = drawRegion.Y - originLocation.Y;
            //レーン背景を黒塗り
            e.Graphics.FillRectangle(Brushes.Black, new RectangleF(drawPosX, drawPosY, Width, Height));
            //Score描画用のY座標の初期座標を画面最下に設定
            float currentDrawPosY = drawPosY + Height - Margin.Bottom;
            //リスト内のScoreについてY座標を変更しながら描画
            foreach (ScoreMaterial material in scoreMaterialList)
            {
                currentDrawPosY -= material.Score.Height * material.Range.Size / material.Score.BeatNumer;
                material.Score.PaintScore(e, drawPosX + Margin.Left, currentDrawPosY, material.Range);
			}
            //tScoresの最後の要素のScoreが閉じているか判定
            if (scoreMaterialList.Any() && scoreMaterialList.Last().Range.Max == scoreMaterialList.Last().Score.BeatNumer)
            {
                //閉じていた場合
                //最後の小節を黄色線で閉じる
                using (Pen myPen = new Pen(Score.measureBorder, 1))
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
		}
    }
}
