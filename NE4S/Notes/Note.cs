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

        #region ノーツの位置やサイズを変えるメソッドたち
        public virtual void ReSize(int size)
        {
            ReSizeOnly(size);
            return;
		}

        /// <summary>
        /// ノーツの種類にかかわらずノーツのサイズを変更することのみ行います
        /// </summary>
        /// <param name="size"></param>
        public void ReSizeOnly(int size)
        {
            int diffSize = size - this.size;
            this.size = size;
            noteRect.Size = new SizeF(ScoreInfo.MinLaneWidth * size, ScoreInfo.NoteHeight);
            if (Status.SelectedNoteArea == NoteArea.LEFT)
            {
                noteRect.X -= diffSize * ScoreInfo.MinLaneWidth;
                pos = new Position(pos.Bar, pos.BeatCount, pos.BaseBeat, pos.Lane - diffSize);
            }
            return;
        }

        public virtual void Relocate(Position pos, PointF location)
		{
            RelocateOnly(pos, location);
			return;
		}

        public void RelocateOnly(Position pos, PointF location)
        {
            RelocateOnly(pos);
            RelocateOnly(location);
            return;
        }

        public virtual void Relocate(Position pos)
		{
            RelocateOnly(pos);
			return;
		}

        public void RelocateOnly(Position pos)
        {
            this.pos = pos;
            return;
        }

        public virtual void Relocate(PointF location)
		{
            RelocateOnly(location);
			return;
		}

        public void RelocateOnly(PointF location)
        {
            noteRect.Location = location;
            return;
        }
        #endregion

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
