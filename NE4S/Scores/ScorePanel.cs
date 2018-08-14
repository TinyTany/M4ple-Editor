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
        private int width, height, tabScoreWidth, tabScoreHeight;
        private int currentPositionX;
        private List<ScoreLane> lanes;

        struct Margin
        {
            public const int Top = 10, Bottom = 10, Left = 10, Right = 10;
        }

        public ScorePanel(int tabScoreWidth, int tabScoreHeight)
        {
            width = widthMax;
            height = tabScoreHeight;
            this.tabScoreWidth = tabScoreWidth;
            this.tabScoreHeight = tabScoreHeight;
            currentPositionX = 0;
            lanes = new List<ScoreLane>();
            #if DEBUG
            for(int i=0; i<10; ++i) lanes.Add(new ScoreLane());
            #endif
        }

        public void MouseScroll(int delta)
        {
            currentPositionX += delta;
            if (currentPositionX < 0) currentPositionX = 0;
            else if (widthMax < currentPositionX) currentPositionX = widthMax;
        }

        public void PaintPanel(PaintEventArgs e)
        {
            for(int i = 0; i < lanes.Count; ++i)
            {
                if(currentPositionX < (ScoreLane.Width + Margin.Left + Margin.Right) * (i + 1) && 
                    (ScoreLane.Width + Margin.Left + Margin.Right) * i < currentPositionX + tabScoreWidth)//ScoreLaneが表示範囲内にあるか
                {
                    int drawPosX = ((int)ScoreLane.Width + Margin.Left + Margin.Right) * i - currentPositionX + Margin.Left;//ScoreLaneの相対位置のX座標を設定
                    int drawPosY = Margin.Top;
                    lanes[i].PaintLane(e, drawPosX, drawPosY);//ScoreLaneを描画
                }
            }
        }
    }
}
