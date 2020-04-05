using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using NE4S.Scores;
using NE4S.Notes.Interface;
using NE4S.Data;

namespace NE4S.Notes.Abstract
{
    // HACK: 多重継承とかあればBegin, Step, Endを同じ基底クラスから派生させたりできたかもしれないが、現状どうしてもまとまらなかったのでWETになっている...
    //       インターフェースを使う？
    #region
    [Serializable()]
    public abstract class LongNoteBegin : Note
    {
        public abstract event Func<Note, Position, bool> PositionChanging;

        protected LongNoteBegin() { }
        protected LongNoteBegin(Note note) : base(note) { }
        protected LongNoteBegin(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }
    }

    [Serializable()]
    public abstract class LongNoteStep : Note
    {
        public abstract event Func<Note, Position, bool> PositionChanging;

        protected LongNoteStep() { }
        protected LongNoteStep(Note note) : base(note) { }
        protected LongNoteStep(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }
    }

    [Serializable()]
    public abstract class LongNoteEnd : AirableNote
    {
        public abstract event Func<Note, Position, bool> PositionChanging;

        protected LongNoteEnd() { }
        protected LongNoteEnd(Note note) : base(note) { }
        protected LongNoteEnd(int size, Position pos, PointF location, int laneIndex)
            : base(size, pos, location, laneIndex) { }
    }
    #endregion

    [Serializable()]
    public abstract class LongNote
    {
        public abstract LongNoteType LongNoteType { get; }
        public abstract int StartTick { get; }
        public abstract int EndTick { get; }
    }

    [Serializable()]
    public abstract class LongNote<TBegin, TEnd> : LongNote
        where TBegin : LongNoteBegin
        where TEnd : LongNoteEnd
    {
        public TBegin BeginNote { get; protected set; }
        public TEnd EndNote { get; protected set; }

        public override int StartTick => BeginNote?.Position.Tick ?? -1;
        public override int EndTick => EndNote?.Position.Tick ?? -1;

        protected abstract bool IsPositionAvailable(Note note, Position position);

        public abstract void Draw(Graphics g, Point drawLocation, LaneBook laneBook);
    }

    /// <summary>
    /// 長いノーツ(Hold,Slide,AirHold)1つ分を表す
    /// </summary>
    [Serializable()]
    public abstract class LongNote<TBegin, TStep, TEnd> : LongNote<TBegin, TEnd>
        where TBegin : LongNoteBegin
        where TStep : LongNoteStep
        where TEnd : LongNoteEnd
    {
        // 帯の描画位置がちょっと上にずれてるので調節用の変数を用意
        protected static readonly PointF drawOffset = new PointF(2, 0);
        // 帯の大きさが縦に少し短いので調整
        protected static readonly float deltaHeight = .2f;

        protected readonly List<TStep> steps = new List<TStep>();

        public IReadOnlyList<TStep> Steps => steps;

        public int LaneLeft
        {
            get
            {
                var stepMin = steps.Min(x => x.Position.Lane);
                return Math.Min(stepMin, Math.Min(BeginNote.Position.Lane, EndNote.Position.Lane));
            }
        }

        public int LaneRight
        {
            get
            {
                var stepMax = steps.Max(x => x.Position.Lane + x.NoteSize);
                var begin = BeginNote.Position.Lane + BeginNote.NoteSize;
                var end = EndNote.Position.Lane + EndNote.NoteSize;
                return Math.Max(stepMax, Math.Max(begin, end));
            }
        }

        protected LongNote() { }

        public abstract bool Put(TStep step);

        public abstract bool UnPut(TStep step);

        public bool IsDrawable(Range<int> tickRange)
        {
            // NOTE: HACK: 範囲の上限が正しく設定されているかはRangeクラス側で責任を持つべきなのでは？
            if (!(tickRange.Min < tickRange.Max))
            {
                Logger.Error("Tick範囲が不正です", true);
                return false;
            }
            bool isAllNoteBehind = !steps.Where(x => x.Position.Tick >= tickRange.Min).Any();
            bool isAllNoteBeyond = !steps.Where(x => x.Position.Tick <= tickRange.Max).Any();
            return !(isAllNoteBehind && isAllNoteBeyond);
        }

        public void UpdateLocation(LaneBook laneBook) => steps.ForEach(x => x.UpdateLocation(laneBook));
    }
}