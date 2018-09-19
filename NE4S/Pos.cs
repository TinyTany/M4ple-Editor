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
		public int Bar { get; set; } = 1;
		private int baseBeat, beatCount;

        public Pos()
        {
			baseBeat = 1;
			beatCount = 1;
        }

		public void PrintPos()
		{
			int GCD = (int)MyUtil.GCD((uint)beatCount, (uint)baseBeat);
			System.Diagnostics.Debug.WriteLine(Bar + "(" + beatCount / GCD + "/" + baseBeat / GCD + ")");
		}

		public int BaseBeat
		{
			get { return baseBeat; }
			set { baseBeat = value; RefreshPos(); }
		}

		public int BeatCount
		{
			get { return beatCount; }
			set { beatCount = value; RefreshPos(); }
		}

		private void RefreshPos()
		{
			
		}
    }
}
