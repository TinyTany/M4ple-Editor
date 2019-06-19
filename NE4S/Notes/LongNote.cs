using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using NE4S.Scores;

namespace NE4S.Notes
{
    /// <summary>
    /// 長いノーツ(Hold,Slide,AirHold)1つ分を表す
    /// </summary>
    [Serializable()]
    public class LongNote
    {
        protected readonly List<Note> notes = new List<Note>();
        public IReadOnlyList<Note> Notes
        {
            get
            {
                return notes;
            }
        }
        //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
        protected static readonly PointF drawOffset = new PointF(2, 0);
        //帯の大きさが縦に少し短いので調整
        protected static readonly float deltaHeight = .2f;

        public int StartTick => notes.OrderBy(x => x.Position.Tick).First().Position.Tick;

        public int EndTick => notes.OrderBy(x => x.Position.Tick).Last().Position.Tick;

        public int LaneLeft => notes.OrderBy(x => x.Position.Lane).First().Position.Lane;

        public int LaneRight => notes.OrderBy(x => x.Position.Lane + x.Size).Last().Position.Lane;

        public bool Contains(Note item) => notes.Contains(item);

        public bool Any() => notes.Any();

        public LongNote() { }

        protected virtual bool IsPositionTickAvailable(Note note, Position position)
        {
            if (position.Tick < notes.OrderBy(x => x.Position.Tick).First().Position.Tick) return false;
            foreach (Note itrNote in notes.Where(x => x != note))
            {
                if (position.Tick == itrNote.Position.Tick) return false;
            }
            return true;
        }

        public void RelocateNoteTickAfterScoreTick(int scoreTick, int deltaTick)
        {
            notes.Where(x => x.Position.Tick >= scoreTick).ToList().ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
        }

        public bool IsDrawable()
        {
            bool isAllNoteBehind = !notes.Where(x => x.Position.Tick >= Status.DrawTickFirst).Any();
            bool isAllNoteBeyond = !notes.Where(x => x.Position.Tick <= Status.DrawTickLast).Any();
            return !(isAllNoteBehind && isAllNoteBeyond);
        }

        public void UpdateLocation(LaneBook laneBook) => notes.ForEach(x => x.UpdateLocation(laneBook));

        /// <summary>
        /// ノーツ位置のチェックのみ行う
        /// </summary>
        public virtual void Draw(Graphics g, Point drawLocation, LaneBook laneBook)
        {
            var list = notes.OrderBy(x => x.Position.Tick).ToList();
            //ノーツ位置のチェック
            for (Note past = list.First(); past != list.Last(); past = list.Next(past))
            {
                Note future = list.Next(past);
                //
                if (laneBook.Find(x => x.HitRect.Contains(future.Location)) == null)
                {
                    ScoreLane lane = laneBook.Find(x => x.StartTick <= future.Position.Tick && future.Position.Tick <= x.EndTick);
                    if (lane == null) break;
                    PointF location = new PointF(
                        lane.LaneRect.Left + future.Position.Lane * ScoreInfo.UnitLaneWidth,
                        //HACK: Y座標が微妙にずれるので-1して調節する
                        lane.HitRect.Bottom - (future.Position.Tick - lane.StartTick) * ScoreInfo.UnitBeatHeight - 1);
                    future.RelocateOnly(location, lane.Index);
                }
            }
        }
    }
}