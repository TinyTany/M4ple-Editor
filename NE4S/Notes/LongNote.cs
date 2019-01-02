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
    public class LongNote : List<Note>
    {
        //帯の描画位置がちょっと上にずれてるので調節用の変数を用意
        protected static readonly PointF drawOffset = new PointF(2, 0);
        //帯の大きさが縦に少し短いので調整
        protected static readonly float deltaHeight = .2f;

        public int StartTick => this.OrderBy(x => x.Position.Tick).First().Position.Tick;

        public int EndTick => this.OrderBy(x => x.Position.Tick).Last().Position.Tick;

        public int LaneLeft => this.OrderBy(x => x.Position.Lane).First().Position.Lane;

        public int LaneRight => this.OrderBy(x => x.Position.Lane + x.Size).Last().Position.Lane;

        public LongNote() { }

        protected virtual bool IsPositionAvailable(Note note, Position position)
        {
            if (position.Tick < this.OrderBy(x => x.Position.Tick).First().Position.Tick) return false;
            foreach (Note itrNote in this.Where(x => x != note))
            {
                if (position.Tick == itrNote.Position.Tick) return false;
            }
            return true;
        }

        public void RelocateNoteTickAfterScoreTick(int scoreTick, int deltaTick)
        {
            this.Where(x => x.Position.Tick >= scoreTick).ToList().ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
        }

        public bool IsDrawable()
        {
            bool isAllNoteBehind = !this.Where(x => x.Position.Tick >= Status.DrawTickFirst).Any();
            bool isAllNoteBeyond = !this.Where(x => x.Position.Tick <= Status.DrawTickLast).Any();
            return !(isAllNoteBehind && isAllNoteBeyond);
        }

        public void UpdateLocation(LaneBook laneBook) => ForEach(x => x.UpdateLocation(laneBook));

        /// <summary>
        /// ノーツ位置のチェックのみ行う
        /// </summary>
        /// <param name="e"></param>
        /// <param name="originPosX"></param>
        /// <param name="originPosY"></param>
        /// <param name="laneBook"></param>
        /// <param name="currentPositionX"></param>
        public virtual void Draw(PaintEventArgs e, int originPosX, int originPosY, LaneBook laneBook, int currentPositionX)
        {
            var list = this.OrderBy(x => x.Position.Tick).ToList();
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
                        lane.LaneRect.Left + future.Position.Lane * ScoreInfo.MinLaneWidth,
                        //HACK: Y座標が微妙にずれるので-1して調節する
                        lane.HitRect.Bottom - (future.Position.Tick - lane.StartTick) * ScoreInfo.MaxBeatHeight - 1);
                    future.RelocateOnly(location, lane.Index);
                }
            }
        }
    }
}