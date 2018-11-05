using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NE4S
{
	public static class MyUtil
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

        public static PointF AddX(this PointF pointF, float x)
        {
            return new PointF(pointF.X + x, pointF.Y);
        }

        public static PointF AddY(this PointF pointF, float y)
        {
            return new PointF(pointF.X, pointF.Y + y);
        }

        public static PointF Add(this PointF pointF, float x, float y)
        {
            return new PointF(pointF.X + x, pointF.Y + y);
        }

        public static PointF Add(this PointF pointF, PointF other)
        {
            return new PointF(pointF.X + other.X, pointF.Y + other.Y);
        }

        public static PointF Add(this PointF pointF, Point other)
        {
            return new PointF(pointF.X + other.X, pointF.Y + other.Y);
        }

        public static PointF Sub(this PointF pointF, PointF other)
        {
            return new PointF(pointF.X - other.X, pointF.Y - other.Y);
        }

        public static PointF Sub(this PointF pointF, Point other)
        {
            return new PointF(pointF.X - other.X, pointF.Y - other.Y);
        }

        public static Point Add(this Point point, int x)
        {
            return new Point(point.X + x, point.Y);
        }

        public static Point Add(this Point pointF, Point other)
        {
            return new Point(pointF.X + other.X, pointF.Y + other.Y);
        }

        public static Point Sub(this Point pointF, Point other)
        {
            return new Point(pointF.X - other.X, pointF.Y - other.Y);
        }
    }
}