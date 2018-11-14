using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Scores;

namespace NE4S.Notes
{
    /// <summary>
    /// 短いノーツ1つ分を表す
    /// </summary>
    public class Note
    {
        private int size;
        private Position pos;
		protected RectangleF hitRect;
        //HACK: ロングノーツでしか使わない（現状そんな気がする）ので、ここで宣言しても本当にいいのかはわかんない
        public int LaneIndex { get; set; } = -1;
        /// <summary>
        /// ノーツを渡すイベントハンドラです（知らんけど）
        /// </summary>
        /// <param name="note"></param>
        public delegate void NoteEventHandler(Note note);

		public Note()
		{
			size = 0;
			pos = null;
			hitRect = new RectangleF();
		}

        public Note(int size, Position pos, PointF location)
        {
			this.size = size;
			this.pos = pos;
			hitRect.Size = new SizeF(ScoreInfo.MinLaneWidth * size, ScoreInfo.NoteHeight);
			hitRect.Location = location;
            //描画中にいい感じにハマるように調節する
            MyUtil.AdjustHitRect(ref hitRect);
        }

        /// <summary>
        /// 1 ≦ Size ≦ 16
        /// </summary>
        public int Size
        {
            get { return size; }
			//外で変更されるの嫌じゃない？知らんけど
			//set { size = value; }
        }

        public Position Pos
        {
            get { return pos; }
			//同上
			//set { pos = value; }
        }

        public PointF Location
        {
            get { return hitRect.Location; }
        }

        /// <summary>
        /// hitrectのWidth
        /// つまりノーツの物理的幅サイズ
        /// </summary>
        public float Width
        {
            get { return hitRect.Width; }
        }

        public bool Contains(PointF location)
        {
            return hitRect.Contains(location);
        }

		public void ReSize(int size)
		{
			this.size = size;
            hitRect.Size = new SizeF(ScoreInfo.MinLaneWidth * size, ScoreInfo.NoteHeight);
            return;
		}

		public void Relocate(Position pos, PointF location)
		{
			Relocate(pos);
			Relocate(location);
			return;
		}

		public void Relocate(Position pos)
		{
			this.pos = pos;
			return;
		}

		public void Relocate(PointF location)
		{
			hitRect.Location = location;
            //描画中にいい感じにハマるように調節する
            MyUtil.AdjustHitRect(ref hitRect);
			return;
		}

        //ノーツ左端からサイズ変更するときに使うために作成したけどなんかやだ
        public void RelocateX(Position newPos, PointF newLocation)
        {
            pos = new Position(pos.Bar, pos.BeatCount, pos.BaseBeat, newPos.Lane);
            hitRect.X = newLocation.X;
            return;
        }

        public virtual void Draw(PaintEventArgs e, int originPosX, int originPosY)
		{
			RectangleF drawRect = new RectangleF(
				hitRect.X - originPosX,
				hitRect.Y - originPosY,
				hitRect.Width,
				hitRect.Height);
			using (SolidBrush myBrush = new SolidBrush(Color.White))
			{
				e.Graphics.FillRectangle(myBrush, drawRect);
			}
			return;
		}
    }
}
