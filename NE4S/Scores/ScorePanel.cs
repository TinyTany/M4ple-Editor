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
    /// MainFormのTabPageのScoreに貼るやつ
    /// </summary>
    public class ScorePanel
    {
        private Size panelSize;
        private int currentPositionX, currentWidthMax;
        private List<ScoreLane> lanes;
        private Model model;
        private HScrollBar hSBar;

        class Margin
        {
            public static int
                Top = ScoreInfo.PanelMargin.Top,
                Bottom = ScoreInfo.PanelMargin.Bottom,
                Left = ScoreInfo.PanelMargin.Left,
                Right = ScoreInfo.PanelMargin.Right;
        }

        public ScorePanel(Size tabScoreSize, HScrollBar hSBar)
        {
            panelSize = tabScoreSize;
            currentPositionX = 0;
            currentWidthMax = 0;
            lanes = new List<ScoreLane>();
            model = new Model();
            this.hSBar = hSBar;
            hSBar.Minimum = 0;
            SetScore(4, 4, 4);
            SetScore(3, 4, 5);
            SetScore(6, 8, 8);
            SetScore(2, 64, 32);
        }

        /// <summary>
        /// 末尾に指定した拍子数の譜面を指定した個数追加
        /// </summary>
        /// <param name="beatNumer">拍子分子</param>
        /// <param name="beatDenom">拍子分母</param>
        /// <param name="barCount">個数</param>
        private void SetScore(int beatNumer, int beatDenom, int barCount)
        {
            List<Score> newScores = new List<Score>();
            for (int i = 0; i < barCount; ++i) newScores.Add(new Score(beatNumer, beatDenom));
            model.AppendScore(newScores);
            int requiredLane = 0;
            foreach(Score newScore in newScores)
            {
                if(newScore.BarSize <= ScoreInfo.LaneMaxBar)
                {
                    if (!lanes.Any())
                    {
                        lanes.Add(new ScoreLane());
                        requiredLane++;
                        lanes.Last().AddScore(newScore);
                        lanes.Last().CurrentBarSize += newScore.BarSize;
                        continue;
                    }
                    if (lanes.Last().CurrentBarSize % ScoreInfo.LaneMaxBar + newScore.BarSize > ScoreInfo.LaneMaxBar || lanes.Last().CurrentBarSize % ScoreInfo.LaneMaxBar == 0)
                    {
                        lanes.Add(new ScoreLane());
                        requiredLane++;
                    }
                    lanes.Last().AddScore(newScore);
                    lanes.Last().CurrentBarSize += newScore.BarSize;
                }
                else
                {

                }
            }
            currentWidthMax += (int)(ScoreLane.Width + Margin.Left + Margin.Right) * requiredLane;
            hSBar.Maximum = currentWidthMax < panelSize.Width ? 0 : currentWidthMax - panelSize.Width; 
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
