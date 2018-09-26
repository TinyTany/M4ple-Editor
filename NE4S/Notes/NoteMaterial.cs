using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NE4S.Notes
{
	public class NoteMaterial
	{
		private Note note;
		private RectangleF hitRect;

		public NoteMaterial(Note note, RectangleF hitRect)
		{
			this.note = note;
			this.hitRect = hitRect;
		}

		public Note Note
		{
			get { return note; }
		}

		public RectangleF HitRect
		{
			get { return hitRect; }
		}

		public void PaintNote(PaintEventArgs e, int drawPosX)
		{
			RectangleF drawRect = new RectangleF(hitRect.X - drawPosX, hitRect.Y, hitRect.Width, hitRect.Height);
			note.Draw(e, drawRect);
		}
	}
}
