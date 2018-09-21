using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S
{
    /// <summary>
    /// 置いたノーツの位置情報
    /// </summary>
    public class Pos
    {
		private int bar, baseBeat, beatCount;
		/// <summary>
		/// ノーツの左端のレーン番号（0-15）
		/// </summary>
		private int lane;

		public Pos(int bar, int beatCount, int baseBeat, int lane)
        {
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

		private void RefreshPos()
		{
			int Gcd = MyUtil.Gcd(beatCount, baseBeat);
			beatCount /= Gcd;
			baseBeat /= Gcd;
		}
    }
}
