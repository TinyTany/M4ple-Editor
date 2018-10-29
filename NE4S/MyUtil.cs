﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NE4S
{
	public class MyUtil
	{
		public static int Gcd(int m, int n)
		{
			if (m < n) return Gcd(n, m);
			if (m == 0 && n == 0) return 1;
			if (n == 0) return m;
			return Gcd(n, m % n);
		}

        public static void AdjustHitRect(ref RectangleF hitRect)
        {
            //描画中にいい感じにハマるように調節する
            //--hitRect.Width; ++hitRect.X;//アンチエイリアスしたらちょっとずれて見えたからこの処理スキップしてみる
            hitRect.Y -= 2;
            return;
        }

        public static float PositionDistance(Position past, Position future)
        {
            float distance = 0;

            return distance;
        }
	}
}