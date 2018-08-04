using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Scores
{
    public class ScorePanel
    {
        private const int widthMax = 40000, margin = 10;
        private int width, height;
        private int currentPositionX;
        private List<ScoreLane> lanes;

        public ScorePanel(int tabScoreHeight)
        {
            width = widthMax;
            height = tabScoreHeight;
            currentPositionX = 0;
            lanes = new List<ScoreLane>();
        }

        public void MouseScroll(int delta)
        {
            currentPositionX += delta;
            if (currentPositionX < 0) currentPositionX = 0;
            else if (widthMax < currentPositionX) currentPositionX = widthMax;
        }

        public void PaintPanel(PaintEventArgs e)
        {
            
        }
    }
}
