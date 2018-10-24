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
    public class Position
    {
		private int bar, baseBeat, beatCount;
		/// <summary>
		/// ノーツの左端のレーン番号（0-15）
		/// </summary>
		private int lane;

		public Position(int bar, int beatCount, int baseBeat, int lane)
        {
			this.bar = bar;
			this.beatCount = beatCount;
			this.baseBeat = baseBeat;
			this.lane = lane;
			RefreshPos();
        }

        /// <summary>
        /// お試し
        /// ロングノーツ描画とかのときにこれらの情報が無いとキビしそうなので無理やり追加してみる
        /// </summary>
        /// <param name="laneIndex"></param>
        /// <param name="location"></param>
        public void SetExtraPosition(int laneIndex, PointF location)
        {
            //やっぱり使わないかな？
            //locationはNoteクラスのhitRectからわかるしlaneIndexもNoteクラスのメンバにしたほうがまるそう
            /*
            this.laneIndex = laneIndex;
            this.location = location;
            //*/
            return;
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

        //TODO: 後で実装する
        //HACK: したけどこれが自然な動作なのかは怪しいし再検討必須そう
        public static Position operator ++(Position position)
        {
            //乗ってるScoreのBeatNumer超えたらどうする…どうする？
            position.beatCount++;
            return position;
        }

		private void RefreshPos()
		{
			int Gcd = MyUtil.Gcd(beatCount, baseBeat);
			beatCount /= Gcd;
			baseBeat /= Gcd;
		}
    }
}
