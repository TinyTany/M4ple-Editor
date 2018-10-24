using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NE4S.Notes
{
    public class Slide : LongNote
    {
        public Slide()
        {

        }

        public Slide(int size, Position pos, PointF location)
        {
            Add(new SlideBegin(size, pos, location));
            //TODO: posとかlocationとかをいい感じに設定する
            location.Y -= 20;
            SlideEnd slideEnd = new SlideEnd(size, ++pos, location);
            Add(slideEnd);
            Status.selectedNote = slideEnd;
        }

        /// <summary>
        /// ノーツ間を繋ぐ帯の描画
        /// </summary>
        /// <param name="past"></param>
        /// <param name="future"></param>
        private void DrawSlideLine(Note past, Note future)
        {
            
        }

        public override void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
			foreach(Note note in this)
            {
                note.Draw(e, originPosX, originPosY);
                if (!(note is SlideEnd))
                {
                    Note next = this.ElementAt(IndexOf(note));

                }
            }
		}
	}
}
