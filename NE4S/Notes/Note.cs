using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace NE4S.Notes
{
    /// <summary>
    /// 短いノーツ1つ分を表す
    /// </summary>
    public class Note
    {
        private int size;
        private Pos pos;

		public Note()
		{
			size = 0;
			pos = null;
		}

        public Note(int size, Pos pos)
        {
			this.size = size;
			this.pos = pos;
        }

        public int Size
        {
            get { return this.size; }
        }

        public Pos Pos
        {
            get { return this.pos; }
			set { pos = value; }
        }

		public virtual void Draw(PaintEventArgs e, RectangleF hitRect)
		{
			using (SolidBrush myBrush = new SolidBrush(Color.White))
			{
				e.Graphics.FillRectangle(myBrush, hitRect);
			}
			return;
		}
    }
}
