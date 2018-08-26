using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Component;

namespace NE4S.Scores
{
    /// <summary>
    /// MainFormのTabPageのScoreに貼るやつ
    /// </summary>
    public class ScorePanel
    {
        private Size panelSize;
        private int currentPositionX, currentWidthMax;
        private List<ScoreLane> lanes;
        private Model model;
        private HScrollBar hSBar;
        private PictureBox pBox;

        class Margin
        {
            public static int
                Top = ScoreInfo.PanelMargin.Top,
                Bottom = ScoreInfo.PanelMargin.Bottom,
                Left = ScoreInfo.PanelMargin.Left,
                Right = ScoreInfo.PanelMargin.Right;
        }

        public ScorePanel(PictureBox pBox, HScrollBar hSBar)
        {
            this.pBox = pBox;
            panelSize = pBox.Size;
            currentPositionX = 0;
            currentWidthMax = 0;
            lanes = new List<ScoreLane>();
            model = new Model();
            this.hSBar = hSBar;
            hSBar.Minimum = 0;
#if DEBUG
            //*
            SetScore(4, 4, 10);
            SetScore(3, 4, 5);
            SetScore(6, 8, 8);
            
            SetScore(2, 64, 32);
            SetScore(13, 4, 2);
            SetScore(2, 4, 8);
            SetScore(26, 8, 2);
            SetScore(1, 32, 32);
            //*/
            SetScore(7, 1, 1);
            SetScore(4, 4, 1);
            SetScore(8, 8, 1);
            SetScore(16, 16, 1);

            //System.Diagnostics.Debug.WriteLine(lanes.Count);
#endif
        }

        /// <summary>
        /// 末尾に指定した拍子数の譜面を指定した個数追加
        /// </summary>
        /// <param name="beatNumer">拍子分子</param>
        /// <param name="beatDenom">拍子分母</param>
        /// <param name="barCount">個数</param>
        private void SetScore(int beatNumer, int beatDenom, int barCount)
        {
            //新たに追加する譜面たちをリストでまとめる
            List<Score> newScores = new List<Score>();
            for (int i = 0; i < barCount; ++i) newScores.Add(new Score(beatNumer, beatDenom));
            //まとめた譜面たちをmodelに入れる
            model.AppendScore(newScores);
            //新規追加前のレーンリストの要素数を記録
            int pastLaneCount = lanes.Count;
            //新譜面たちをレーンに割り当て
            foreach(Score newScore in newScores)
            {
                //newScoreが1つのレーンの最大サイズで収まるか判定
                if(newScore.BarSize <= ScoreInfo.LaneMaxBar)
                {
                    //そもそも現在のレーンリストが空の時は新レーンを1つ補充
                    if (!lanes.Any())
                    {
                        lanes.Add(new ScoreLane());
                    }
                    //現在のリストにあるレーンの末尾にまだnewScoreを入れる余裕があるか判定
                    if (lanes.Last().CurrentBarSize + newScore.BarSize > ScoreInfo.LaneMaxBar)
                    {
                        //余裕がないときは新たな空レーンを追加
                        lanes.Add(new ScoreLane());
                    }
                    //レーン末尾にnewScoreを格納
                    lanes.Last().AddScore(newScore, new Range(1, newScore.BeatNumer));
                }
                //収まらなかった場合
                else
                {
                    //なんやかんやで分割して複数レーンに格納する
                    for(int i = 0; i < newScore.BarSize / ScoreInfo.LaneMaxBar; ++i)
                    {
                        //新たにレーンを追加
                        lanes.Add(new ScoreLane());
                        //末尾のレーンに新たなScoreを範囲を指定して格納
                        lanes.Last().AddScore(
                            newScore, 
                            new Range(
                                (int)(i * newScore.BeatDenom * ScoreInfo.LaneMaxBar + 1),
                                Math.Min(newScore.BeatNumer, (int)((i + 1) * newScore.BeatDenom * ScoreInfo.LaneMaxBar))));
                    }
                }
            }
            //レーンの増分だけパネルの最大幅を更新
            currentWidthMax += (int)(ScoreLane.Width + Margin.Left + Margin.Right) * (lanes.Count - pastLaneCount);
            hSBar.Maximum = currentWidthMax < panelSize.Width ? 0 : currentWidthMax - panelSize.Width;
            //ScoreLaneのインデックスを設定
            SetLaneIndex();
        }

        /// <summary>
        /// indexの直後に新たにScoreを挿入
        /// </summary>
        /// <param name="index"></param>
        /// <param name="beatNumer"></param>
        /// <param name="beatDenom"></param>
        /// <param name="barCount"></param>
        public void InsertScoreForward(int index, int beatNumer, int beatDenom, int barCount)
        {
            
        }

        /// <summary>
        /// indexの直前に新たにScoreを挿入
        /// </summary>
        /// <param name="index"></param>
        /// <param name="beatNumer"></param>
        /// <param name="beatDenom"></param>
        /// <param name="barCount"></param>
        public void InsertScoreBackward(int index, int beatNumer, int beatDenom, int barCount)
        {
            
        }

        /// <summary>
        /// 指定されたScore削除
        /// </summary>
        /// <param name="lane">対象のScoreが入っているレーン</param>
        /// <param name="score">削除対象のScore</param>
        public void DeleteScore(ScoreLane lane, Score score)
        {
            DeleteScore(lane, score, 1);
        }

        /// <summary>
        /// 指定されたScoreからcount個のScoreを削除
        /// </summary>
        /// <param name="lane">対象のScoreが入っているレーン</param>
        /// <param name="score">削除開始のScore</param>
        /// <param name="count">削除する個数</param>
        public void DeleteScore(ScoreLane lane, Score score, int count)
        {
            //削除前のレーンリストの要素数を記録
            int pastLaneCount = lanes.Count;
            //scoreからcount個Scoreを削除
            for(Score itrScore = score; itrScore != null && itrScore.ScoreIndex - score.ScoreIndex < count; itrScore = model.ScoreNext(itrScore))
            {
                //選択されたScoreが初めて含まれるレーンを特定
                ScoreLane laneBegin = lanes.Find(x => x.Contains(itrScore));
                if (laneBegin.IsScoreClose(itrScore))
                {
                    laneBegin.DeleteScore(itrScore);
                    //削除処理によってレーンが空になっていないか判定
                    if (!laneBegin.Any()) lanes.Remove(laneBegin);
                }
                else
                {
                    //初めて含まれるレーンから削除するScoreのサイズに応じて複数レーンにわたってScoreを削除
                    //空レーン判定は後でまとめて行う（そうしないとうまくいかない）
                    for (int i = 0; i < itrScore.BarSize / ScoreInfo.LaneMaxBar; ++i)
                    {
                        lanes.ElementAt(laneBegin.LaneIndex + i).DeleteScore(itrScore);
                    }
                    //空レーン判定を行い空のレーンは削除
                    for (int i = 0; i < itrScore.BarSize / ScoreInfo.LaneMaxBar; ++i)
                    {
                        //削除処理によってレーンが空になっていないか判定
                        if (!lanes.ElementAt(laneBegin.LaneIndex).Any()) lanes.RemoveAt(laneBegin.LaneIndex);
                    }
                }
                //ScoreLaneのインデックスを設定
                SetLaneIndex();
            }
            //modelから該当範囲のScoreを削除
            model.DeleteScore(score.ScoreIndex, count);
            //レーンの増分だけパネルの最大幅を更新
            currentWidthMax += (int)(ScoreLane.Width + Margin.Left + Margin.Right) * (lanes.Count - pastLaneCount);
            hSBar.Maximum = currentWidthMax < panelSize.Width ? 0 : currentWidthMax - panelSize.Width;
            //PictureBoxを更新
            pBox.Refresh();
        }

        private void SetLaneIndex()
        {
            for (int i = 0; i < lanes.Count; ++i)
            {
                //LaneIndexを設定
                lanes[i].LaneIndex = i;
                //レーンの当たり判定はLaneIndexに依存しているので同時に設定する
                lanes[i].UpdateHitRect();
            }
        }

        public void MouseClick(MouseEventArgs e)
        {
#if DEBUG
            //クリックされたレーンを特定
            ScoreLane selectedLane =
                lanes.Find(x => x.HitRect.Contains(e.X + currentPositionX, e.Y));
            if (selectedLane != null && selectedLane.SelectedScore(e) != null && e.Button == MouseButtons.Right)
            {
                new EditCMenu(this, selectedLane, selectedLane.SelectedScore(e)).Show(pBox, e.Location);
            }
#endif
        }

        public void MouseScroll(int delta)
        {
            currentPositionX -= delta;
            if (currentPositionX < hSBar.Minimum) currentPositionX = hSBar.Minimum;
            else if (hSBar.Maximum < currentPositionX) currentPositionX = hSBar.Maximum;
            hSBar.Value = currentPositionX;
        }

        public void HSBarScroll(ScrollEventArgs e)
        {
            currentPositionX += (e.NewValue - e.OldValue);
        }

        public void PaintPanel(PaintEventArgs e)
        {
            for(int i = 0; i < lanes.Count; ++i)
            {
                //ScoreLaneが表示範囲内にあるか
                if (currentPositionX < (ScoreLane.Width + Margin.Left + Margin.Right) * (i + 1) && 
                    (ScoreLane.Width + Margin.Left + Margin.Right) * i < currentPositionX + panelSize.Width)
                {
                    //ScoreLaneの相対位置のX座標を設定
                    int drawPosX = ((int)ScoreLane.Width + Margin.Left + Margin.Right) * i - currentPositionX + Margin.Left;
                    int drawPosY = Margin.Top;
                    //ScoreLaneを描画
                    lanes[i].PaintLane(e, drawPosX, drawPosY);
                }
            }
        }
    }
}
