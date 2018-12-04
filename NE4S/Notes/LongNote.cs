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
    [Serializable()]
    public class LongNote : List<Note>
    {
        //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
        protected static readonly PointF drawOffset = new PointF(2, 0);
        //帯の大きさが縦に少し短いので調整
        protected static readonly float deltaHeight = .2f;

        public LongNote()
        {
            
        }

        protected bool IsPositionAvailable(Note note, Position position)
        {
            if (position.Tick < this.OrderBy(x => x.Position.Tick).First().Position.Tick) return false;
            foreach (Note itrNote in this.Where(x => x != note))
            {
                if (position.Tick == itrNote.Position.Tick) return false;
            }
            return true;
        }

        public bool IsDrawable() => 
            this.Where(x => Status.DrawTickFirst <= x.Position.Tick && x.Position.Tick <= Status.DrawTickLast).Any();

        public virtual void Draw(PaintEventArgs e, int originPosX, int originPosY, ScoreBook scoreBook, LaneBook laneBook, int currentPositionX) { }
    }
}