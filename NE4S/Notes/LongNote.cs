using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Notes
{
    /// <summary>
    /// 長いノーツ(Hold,Slide,AirHold)1つ分を表す
    /// </summary>
    public class LongNote : List<Note>
    {

        public LongNote()
        {
            
        }

		public virtual void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX)
		{

		}
    }
}
