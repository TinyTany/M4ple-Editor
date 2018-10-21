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
			//描画時にレーンの線の間にノーツがうまくハマるようにする
			++this.hitRect.X; --this.hitRect.Width; this.hitRect.Y -= 2;
		}

		public void RefreshLocation(Point location, Position newPos)
		{
			hitRect.Location = location;
			//描画時にレーンの線の間にノーツがうまくハマるようにする
			//変化するのは位置のみなので位置のみ調整
			++this.hitRect.X; this.hitRect.Y -= 2;
			//TODO: Posの座標も更新すること
			note.Pos = newPos;
		}

		public Note Note
		{
			get { return note; }
		}

		public RectangleF HitRect
		{
			get { return hitRect; }
		}

		public void PaintNote(PaintEventArgs e, float originPosX, float originPosY)
		{
			RectangleF drawRect = new RectangleF(hitRect.X - originPosX, hitRect.Y - originPosY, hitRect.Width, hitRect.Height);
			note.Draw(e, drawRect);
		}
	}
}
