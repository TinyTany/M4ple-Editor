using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Scores;
using NE4S.Define;
using NE4S.Notes.Interface;

namespace NE4S.Notes.Abstract
{
    /// <summary>
    /// 短いノーツ1つ分を表す
    /// </summary>
    [Serializable()]
    public abstract class Note : ISizableNote
    {
        public virtual NoteType NoteType => NoteType.Unknown;
        public virtual int NoteID => -1;
        /// <summary>
        /// 1 ≦ Size ≦ 16
        /// </summary>
        public int Size { get; protected set; }
        public Position Position { get; protected set; }
		protected RectangleF noteRect;
        protected PointF adjustNoteRect = new PointF(0, -2);
        public virtual int LaneIndex { get; protected set; } = -1;

        [field: NonSerialized]
        public int TimeLineIndex { get; set; } = 0;

		protected Note()
		{
			Size = 0;
			Position = null;
			noteRect = new RectangleF();
		}

        protected Note(Note note)
        {
            if (note == null)
            {
                Logger.Error("Noteのコピーに失敗しました。引数がnullです。");
                Size = 0;
                Position = new Position();
                noteRect = new RectangleF();
                return;
            }
            InitializeInstance(note.Size, note.Position, note.Location, note.LaneIndex);
        }

        protected Note(int size, Position pos, PointF location, int laneIndex)
        {
            InitializeInstance(size, pos, location, laneIndex);
        }

        private void InitializeInstance(int size, Position pos, PointF location, int laneIndex)
        {
            Size = size;
            Position = new Position(pos);
            noteRect.Size = new SizeF(ScoreInfo.UnitLaneWidth * size, ScoreInfo.NoteHeight);
            noteRect.Location = location;
            LaneIndex = laneIndex;
        }

        public PointF Location
        {
            get { return noteRect.Location; }
        }

        /// <summary>
        /// hitrectのWidth
        /// つまりノーツの物理的幅サイズ
        /// </summary>
        public float Width
        {
            get { return noteRect.Width; }
        }

        public virtual bool Contains(PointF location)
        {
            RectangleF hitRect = new RectangleF(noteRect.X + adjustNoteRect.X, noteRect.Y + adjustNoteRect.Y, noteRect.Width, noteRect.Height);
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
            int diffSize = size - Size;
            Size = size;
            noteRect.Size = new SizeF(ScoreInfo.UnitLaneWidth * size, ScoreInfo.NoteHeight);
            if (Status.SelectedNoteArea == NoteArea.Left)
            {
                noteRect.X -= diffSize * ScoreInfo.UnitLaneWidth;
                Position = new Position(Position.Lane - diffSize, Position.Tick);
            }
            return;
        }

        public virtual void Relocate(Position pos, PointF location, int laneIndex)
		{
            RelocateOnly(pos, location, laneIndex);
			return;
		}

        public void RelocateOnly(Position pos, PointF location, int laneIndex)
        {
            RelocateOnly(pos);
            RelocateOnly(location, laneIndex);
            return;
        }

        public virtual void Relocate(Position pos)
		{
            RelocateOnly(pos);
			return;
		}

        public void RelocateOnly(Position pos)
        {
            Position = pos;
            return;
        }

        public virtual void Relocate(PointF location, int laneIndex)
		{
            RelocateOnly(location, laneIndex);
			return;
		}

        public void RelocateOnly(PointF location, int laneIndex)
        {
            noteRect.Location = location;
            LaneIndex = laneIndex;
            return;
        }

        public virtual void RelocateOnlyAndUpdate(Position position, LaneBook laneBook)
        {
            RelocateOnly(position);
            UpdateLocation(laneBook);
        }
        #endregion

        public void UpdateLocation(LaneBook laneBook)
        {
            ScoreLane lane = laneBook.Find(x => x.StartTick <= Position.Tick && Position.Tick <= x.EndTick);
            if (lane == null) return;
            PointF location = new PointF(
                lane.LaneRect.Left + Position.Lane * ScoreInfo.UnitLaneWidth,
                //HACK: Y座標が微妙にずれるので-1して調節する
                lane.HitRect.Bottom - (Position.Tick - lane.StartTick) * ScoreInfo.UnitBeatHeight - 1);
            RelocateOnly(location, lane.Index);
        }

        public virtual void Draw(Graphics g, Point drawLocation)
		{
			RectangleF drawRect = new RectangleF(
				noteRect.X - drawLocation.X + adjustNoteRect.X,
				noteRect.Y - drawLocation.Y + adjustNoteRect.Y,
				noteRect.Width,
				noteRect.Height);
			using (SolidBrush myBrush = new SolidBrush(Color.White))
			{
				g.FillRectangle(myBrush, drawRect);
			}
			return;
		}
    }
}
