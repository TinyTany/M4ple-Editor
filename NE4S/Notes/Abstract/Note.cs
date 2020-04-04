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
using NE4S.Data;

namespace NE4S.Notes.Abstract
{
    /// <summary>
    /// 短いノーツ1つ分を表す
    /// </summary>
    [Serializable()]
    public abstract class Note : ISizableNote
    {
        public abstract NoteType NoteType { get; }

        /// <summary>
        /// 1 ≦ NoteSize ≦ 16
        /// </summary>
        public int NoteSize { get; protected set; }

        public Position Position { get; protected set; }

        protected RectangleF noteRect;

        // NOTE: LongNoteの帯を何レーンに渡って描画するかをO(1)で取りたいのでLaneIndexとして保持する
        //       なので、このプロパティの値はLongNoteを構成するノーツ以外では基本的に使わない（読み取らない）はず
        public int LaneIndex { get; private set; }

        public PointF Location => noteRect.Location;

        public float Width => noteRect.Width;

        // HACK: なんとかしたい...
        // NOTE: 当たり判定や描画位置のための矩形のLocationとして、NoteRectのものにこれを足して使用している気がする
        protected readonly PointF adjustNoteRect = new PointF(0, -2);

        [field: NonSerialized]
        public int TimeLineIndex { get; set; } = 0;

        protected Note() { }

        protected Note(Note note)
        {
            if (note == null)
            {
                Logger.Error("Noteのコピーに失敗しました。引数がnullです。");
                NoteSize = 0;
                Position = new Position();
                noteRect = new RectangleF();
                return;
            }
            InitializeInstance(note.NoteSize, note.Position, note.Location, note.LaneIndex);
        }

        protected Note(int size, Position pos, PointF location, int laneIndex)
        {
            InitializeInstance(size, pos, location, laneIndex);
        }

        private void InitializeInstance(int size, Position pos, PointF location, int laneIndex)
        {
            NoteSize = size;
            Position = new Position(pos);
            noteRect.Size = new SizeF(ScoreInfo.UnitLaneWidth * size, ScoreInfo.NoteHeight);
            noteRect.Location = location;
            LaneIndex = laneIndex;
        }

        public virtual bool Contains(PointF location)
        {
            RectangleF hitRect = new RectangleF(noteRect.X + adjustNoteRect.X, noteRect.Y + adjustNoteRect.Y, noteRect.Width, noteRect.Height);
            return hitRect.Contains(location);
        }

        /// <summary>
        /// ノーツのサイズを変更します
        /// ノーツの種類によって、ノーツのサイズ変更に伴った追加の処理なども行うことも仕様とします
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual bool ReSize(int size)
        {
            if (size <= 0 || ScoreInfo.Lanes < size)
            {
                Logger.Error("引数sizeの値が不正です。");
                return false;
            }
            if (Position.Lane + size > ScoreInfo.Lanes)
            {
                Logger.Error("ノーツがレーン範囲外になるため、操作は実行できません。");
                return false;
            }
            int diffSize = size - NoteSize;
            NoteSize = size;
            noteRect.Size = new SizeF(ScoreInfo.UnitLaneWidth * size, ScoreInfo.NoteHeight);
            // HACK: こういう操作をここでやるのはちょっと...
            if (Status.SelectedNoteArea == NoteArea.Left)
            {
                noteRect.X -= diffSize * ScoreInfo.UnitLaneWidth;
                Position = new Position(Position.Lane - diffSize, Position.Tick);
            }
            return true;
        }

        /// <summary>
        /// ノーツのPositionを変更します
        /// ノーツの種類や状態によって、ノーツのPosition変更に伴った追加の処理なども行うことを仕様とします
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual bool Relocate(Position pos)
        {
            if (pos is null) 
            {
                Logger.Error("引数がnullのため、操作を実行できません。", true);
                return false;
            }
            if (pos.Lane + NoteSize > ScoreInfo.Lanes)
            {
                Logger.Error("ノーツがレーン範囲外になるため、操作は実行できません。");
                return false;
            }
            Position = pos;
            return true;
        }

        /// <summary>
        /// Positionの値から、ノーツの絶対位置(noteRect.Location)を更新します。
        /// </summary>
        /// <param name="laneBook"></param>
        public void UpdateLocation(LaneBook laneBook)
        {
            ScoreLane lane = laneBook.Find(x => x.StartTick <= Position.Tick && Position.Tick <= x.EndTick);
            if (lane == null) return;
            noteRect.Location = new PointF(
                lane.LaneRect.Left + Position.Lane * ScoreInfo.UnitLaneWidth,
                // HACK: Y座標が微妙にずれるので-1して調節する
                lane.HitRect.Bottom - (Position.Tick - lane.StartTick) * ScoreInfo.UnitBeatHeight - 1);
            LaneIndex = lane.Index;
        }

        public abstract void Draw(Graphics g, Point drawLocation);
    }
}
