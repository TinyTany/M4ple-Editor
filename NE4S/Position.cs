using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NE4S
{
    /// <summary>
    /// 置いたノーツの位置情報
    /// </summary>
    [Serializable()]
    public class Position : IComparable<Position>
    {
        /// <summary>
        /// 小節番号
        /// 1スタート
        /// </summary>
		public int Bar { get; }
        public int BaseBeat { get; private set; }
        public int BeatCount { get; private set; }
        /// <summary>
        /// ノーツの左端のレーン番号（0-15）
        /// </summary>
        public int Lane { get; private set; }
        public float Size { get; private set; }

        public Position(int bar, int beatCount, int baseBeat, int lane)
        {
            //barは1始まり(ScoreMaterialのCalculatePosメソッド参照)
			Bar = bar;
			BeatCount = beatCount;
			BaseBeat = baseBeat;
			Lane = lane;
			RefreshPos();
        }

		public void PrintPos()
		{
			System.Diagnostics.Debug.WriteLine(Bar + "(" + BeatCount + "/" + BaseBeat + "), " + Lane);
		}

        public Position Next()
        {
            Position nextPosition = new Position(Bar, BeatCount, BaseBeat, Lane);
            nextPosition.BeatCount++;
            nextPosition.RefreshPos();
            return nextPosition;
        }

        public override bool Equals(object obj)
        {
            return CompareTo((Position)obj) == 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private void RefreshPos()
		{
			int Gcd = MyUtil.Gcd(BeatCount, BaseBeat);
			BeatCount /= Gcd;
			BaseBeat /= Gcd;
            Size = BeatCount / (float)BaseBeat;
		}

        /// <summary>
        /// otherと比べて自分が小さければ-1、同じなら0、大きければ1を返す
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Position other)
        {
            if(Bar < other.Bar) { return -1; }
            else if(Bar > other.Bar) { return 1; }
            else if(BeatCount / (float)BaseBeat < other.BeatCount / (float)other.BaseBeat) { return -1; }
            else if(BeatCount / (float)BaseBeat > other.BeatCount / (float)other.BaseBeat) { return 1; }
            else { return 0; }
        }

        
    }
}
