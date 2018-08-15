using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Scores
{
    /// <summary>
    /// MainFormのTabPageのScoreに貼るやつ
    /// </summary>
    public class ScorePanel
    {
        private const int widthMax = 40000;
        private int width, height;
        private int currentPositionX, currentWidthMax;
        private List<ScoreLane> lanes;
        private Model model;

        class Margin
        {
            public static int
                Top = ScoreInfo.PanelMargin.Top,
                Bottom = ScoreInfo.PanelMargin.Bottom,
                Left = ScoreInfo.PanelMargin.Left,
                Right = ScoreInfo.PanelMargin.Right;
        }

        public ScorePanel(int tabScoreWidth, int tabScoreHeight)
        {
            width = tabScoreWidth;
            height = tabScoreHeight;
            currentPositionX = 0;
            lanes = new List<ScoreLane>();
            model = new Model();
#if DEBUG
            for(int i=0; i<100; ++i) lanes.Add(new ScoreLane());
#endif
        }

        private void SetLane(int length)
        {
            currentWidthMax = (int)ScoreLane.Width * length;
        }

        public void MouseScroll(int delta, HScrollBar hSBar)
        {
            currentPositionX -= delta;
            if (currentPositionX < 0) currentPositionX = 0;
            else if (widthMax < currentPositionX) currentPositionX = widthMax;
            hSBar.Value = currentPositionX;
        }

        public void HSBarScroll(ScrollEventArgs e)
        {
            currentPositionX += (e.NewValue - e.OldValue);
        }

        public void PaintPanel(PaintEventArgs e)
        {
#if DEBUG
            int cnt = 0;
#endif
            for(int i = 0; i < lanes.Count; ++i)
            {
                if(currentPositionX < (ScoreLane.Width + Margin.Left + Margin.Right) * (i + 1) && 
                    (ScoreLane.Width + Margin.Left + Margin.Right) * i < currentPositionX + width)//ScoreLaneが表示範囲内にあるか
                {
                    int drawPosX = ((int)ScoreLane.Width + Margin.Left + Margin.Right) * i - currentPositionX + Margin.Left;//ScoreLaneの相対位置のX座標を設定
                    int drawPosY = Margin.Top;
                    lanes[i].PaintLane(e, drawPosX, drawPosY);//ScoreLaneを描画
#if DEBUG
                    ++cnt;
#endif
                }
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(cnt);
#endif
        }
    }
}
