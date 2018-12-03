using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NE4S.Scores;

namespace NE4S
{
    /// <summary>
    /// 置いたノーツの位置情報
    /// </summary>
    [Serializable()]
    public class Position : IComparable<Position>
    {
        /// <summary>
        /// ノーツの左端のレーン番号（0-15）
        /// </summary>
        public int Lane { get; private set; }
        public int Tick { get; private set; }

        public Position(int lane, int tick)
        {
            Lane = lane;
            Tick = tick;
        }

		public void PrintPosition()
		{
            System.Diagnostics.Debug.WriteLine("(Lane, Tick) = (" + Lane + ", " + Tick + ")");
        }

        public Position Next()
        {
            return new Position(Lane, Tick + (ScoreInfo.MaxBeatDiv / Status.Beat));
        }

        public override bool Equals(object obj)
        {
            return CompareTo((Position)obj) == 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// otherと比べて自分が小さければ負の値、同じなら0、大きければ正の値を返す
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Position other)
        {
            return Tick - other.Tick;
        }

        
    }
}
