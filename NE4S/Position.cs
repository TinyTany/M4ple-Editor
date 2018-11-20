﻿using System;
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
    public class Position : IComparable<Position>
    {
		private int bar, baseBeat, beatCount;
		/// <summary>
		/// ノーツの左端のレーン番号（0-15）
		/// </summary>
		private int lane;
        //Slideで使う
        /// <summary>
        /// 0 ≦ size ＜ beatNumer/(float)beatDenom
        /// </summary>
        private float size;

		public Position(int bar, int beatCount, int baseBeat, int lane)
        {
            //barは1始まり(ScoreMaterialのCalculatePosメソッド参照)
			this.bar = bar;
			this.beatCount = beatCount;
			this.baseBeat = baseBeat;
			this.lane = lane;
			RefreshPos();
        }

		public void PrintPos()
		{
			System.Diagnostics.Debug.WriteLine(bar + "(" + beatCount + "/" + baseBeat + "), " + lane);
		}

        /// <summary>
        /// 小節番号
        /// 1スタート
        /// </summary>
		public int Bar
		{
			get { return bar; }
		}

		public int BaseBeat
		{
			get { return baseBeat; }
		}

		public int BeatCount
		{
			get { return beatCount; }
		}

		public int Lane
		{
			get { return lane; }
		}

        public float Size
        {
            get { return size; }
        }

        //TODO: 後で実装する
        //HACK: したけどこれが自然な動作なのかは怪しいし再検討必須そう
        public static Position operator ++(Position position)
        {
            //乗ってるScoreのBeatNumer超えたらどうする…どうする？
            position.beatCount++;
            return position;
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
			int Gcd = MyUtil.Gcd(beatCount, baseBeat);
			beatCount /= Gcd;
			baseBeat /= Gcd;
            size = beatCount / (float)baseBeat;
		}

        /// <summary>
        /// otherと比べて自分が小さければ-1、同じなら0、大きければ1を返す
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Position other)
        {
            if(bar < other.Bar) { return -1; }
            else if(bar > other.Bar) { return 1; }
            else if(beatCount / (float)baseBeat < other.BeatCount / (float)other.BaseBeat) { return -1; }
            else if(beatCount / (float)baseBeat > other.BeatCount / (float)other.BaseBeat) { return 1; }
            else { return 0; }
        }

        
    }
}
