using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NE4S.Notes
{
    /// <summary>
    /// 長いノーツ(Hold,Slide,AirHold)1つ分を表す
    /// </summary>
    public class LongNote
    {
        private List<Note> noteList;

        public LongNote()
        {
            noteList = new List<Note>();
        }

        public List<Note> Notes
        {
            get { return this.noteList; }
        }

		public virtual void Draw(PaintEventArgs e)
		{

		}
    }
}
