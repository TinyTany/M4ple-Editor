using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using NE4S.Notes;
using NE4S.Scores;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NE4S.Notes.Abstract;
using NE4S.Data;

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

        public static int Lcm(int m, int n)
        {
            return m * n / Gcd(m, n);
        }

        public static Position Next(this Position p)
        {
            return new Position(p.Lane, p.Tick + (ScoreInfo.MaxBeatDiv / Status.Beat));
        }

        /// <summary>
        /// 任意の参照型オブジェクトをディープコピーします
        /// パフォーマンスを見ながら、コピーコンストラクタによる方法も検討してください
        /// </summary>
        public static T DeepCopy<T>(this T target) where T : class
        {
            T copy = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, target);
                ms.Position = 0;
                copy = bf.Deserialize(ms) as T;
            }
            return copy;
        }

        /// <summary>
        /// ノーツが選択されたとき、どのへんの領域をクリックしたのか判定します
        /// Editモードでノーツをクリックした位置に応じてサイズ変更か位置変更かを決めるために使う
        /// </summary>
        /// <param name="note"></param>
        /// <param name="location"></param>
        /// <param name="noteArea"></param>
        public static void SetNoteArea(Note note, PointF location, ref Define.NoteArea noteArea)
        {
            //それぞれの領域の割合を設定
            float leftCenter = .33f, centerRight = .66f;
            float locationRatio = (location.X - note.Location.X) / note.Width;
            if (locationRatio <= leftCenter) noteArea = Define.NoteArea.Left;
            else if (locationRatio <= centerRight) noteArea = Define.NoteArea.Center;
            else noteArea = Define.NoteArea.Right;
        }

        /// <summary>
        /// list内のitemの1つ前の要素を返します。itemがlistに含まれていないか、先頭の場合は既定値を返します。
        /// </summary>
        public static T Prev<T>(this List<T> list, T item)
        {
            if (!list.Contains(item)) return default;
            if (list.IndexOf(item) <= 0) return default;
            return list.ElementAt(list.IndexOf(item) - 1);
        }

        /// <summary>
        /// list内のitemの1つ次の要素を返します。itemがlistに含まれていないか、末尾の場合は既定値を返します。
        /// </summary>
        public static T Next<T>(this List<T> list, T item)
        {
            if (!list.Contains(item)) return default;
            if (list.IndexOf(item) >= list.Count - 1) return default;
            return list.ElementAt(list.IndexOf(item) + 1);
        }

        /// <summary>
        /// 小数第3位以下を四捨五入する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal MyRound(this decimal value)
        {
            return Math.Round(value * 100m) / 100m;
        }

        public static float MyRound(this float value)
        {
            return (float)Math.Round(value * 100f) / 100f;
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

        public static PointF Mult(this PointF pointF, float k)
        {
            return new PointF(pointF.X * k, pointF.Y * k);
        }

        public static Point AddX(this Point point, int x)
        {
            return new Point(point.X + x, point.Y);
        }

        public static Point AddY(this Point point, int y)
        {
            return new Point(point.X, point.Y + y);
        }

        public static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        public static Point Sub(this Point point, Point other)
        {
            return new Point(point.X - other.X, point.Y - other.Y);
        }

        /// <summary>
        /// GraphicsPathを平行移動します。
        /// </summary>
        /// <param name="graphicsPath"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static void Translate(this GraphicsPath graphicsPath, float dx, float dy)
        {
            var mat = new Matrix();
            mat.Translate(dx, dy, MatrixOrder.Append);
            graphicsPath.Transform(mat);
            //return graphicsPath;
        }

        /// <summary>
        /// いまのところSlideカーブでの当たり判定用パスを作るためのみに使う
        /// 汎用性を高めればDrawSlideCurveメソッドとかをスッキリできそう
        /// </summary>
        /// <param name="past"></param>
        /// <param name="curve"></param>
        /// <param name="future"></param>
        /// <param name="drawOffset"></param>
        /// <returns></returns>
        public static GraphicsPath CreateSlideCurvePath(Note past, Note curve, Note future, PointF drawOffset)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            PointF pastRerativeLocation = new PointF(past.Location.X, past.Location.Y);
            float positionDistanceFuture = (future.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            float positionDistanceCurve = (curve.Position.Tick - past.Position.Tick) * ScoreInfo.UnitBeatHeight;
            float diffXFuture = (future.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;
            float diffXCurve = (curve.Position.Lane - past.Position.Lane) * ScoreInfo.UnitLaneWidth;
            //ノーツfutureの位置はノーツpastの位置に2ノーツの距離を引いて表す。またTopRightの水平位置はfutureのWidthを使うことに注意
            PointF topLeft = pastRerativeLocation.Add(diffXFuture, -positionDistanceFuture).Add(drawOffset);
            PointF topRight = pastRerativeLocation.Add(diffXFuture, -positionDistanceFuture).Add(-drawOffset.X, drawOffset.Y).AddX(future.Width);
            //以下の2つはレーンをまたがないときと同じ
            PointF bottomLeft = pastRerativeLocation.Add(drawOffset);
            PointF bottomRight = pastRerativeLocation.Add(-drawOffset.X, drawOffset.Y).AddX(past.Width);
            //3つのそれぞれのノーツの中心の座標
            PointF topCenter = topLeft.AddX(future.Width / 2f - drawOffset.X);
            PointF bottomCenter = bottomLeft.AddX(past.Width / 2f - drawOffset.X);
            PointF curveCenter = pastRerativeLocation.Add(diffXCurve, -positionDistanceCurve).AddX(curve.Width / 2f);
            //
            //下からアンカーまでの比率
            float ratio = (curveCenter.Y - bottomCenter.Y) / (topCenter.Y - bottomCenter.Y);
            //カーブノーツのY座標で水平にスライドを切ったときのスライド幅
            float widthAnchor = (topRight.X - topLeft.X) * ratio + (bottomRight.X - bottomLeft.X) * (1 - ratio);
            //
            graphicsPath.AddBezier(bottomLeft, curveCenter.AddX(-widthAnchor / 2f), topLeft);
            graphicsPath.AddLine(topLeft, topRight);
            graphicsPath.AddBezier(topRight, curveCenter.AddX(widthAnchor / 2f), bottomRight);
            graphicsPath.AddLine(bottomLeft, bottomRight);
            return graphicsPath;
        }

        /// <summary>
        /// 現在の図形に２次ベジエ曲線を追加します
        /// </summary>
        /// <param name="graphicsPath"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="anchor"></param>
        public static void AddBezier(this GraphicsPath graphicsPath, PointF begin, PointF anchor, PointF end)
        {
            float ratio = 2 / 3f;
            PointF beginAnchor = begin.Add(anchor.Sub(begin).Mult(ratio));
            PointF endAnchor = end.Add(anchor.Sub(end).Mult(ratio));
            graphicsPath?.AddBezier(begin, beginAnchor, endAnchor, end);
        }

        /// <summary>
        /// ２次ベジエ曲線を描画します
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="anchor"></param>
        public static void DrawBezier(this Graphics graphics, Pen pen, PointF begin, PointF anchor, PointF end)
        {
            float ratio = 2 / 3f;
            PointF beginAnchor = begin.Add(anchor.Sub(begin).Mult(ratio));
            PointF endAnchor = end.Add(anchor.Sub(end).Mult(ratio));
            graphics?.DrawBezier(pen, begin, beginAnchor, endAnchor, end);
        }

        public static GraphicsPath RoundedPath(this RectangleF rectangle)
        {
            float roundRatio = .25f;
            SizeF rectSize = new SizeF(rectangle.Width * roundRatio, rectangle.Height * roundRatio);
            RectangleF rectTopLeft = new RectangleF(rectangle.X, rectangle.Y, rectSize.Width, rectSize.Height);
            RectangleF rectTopRight = new RectangleF(rectangle.X + rectangle.Width * (1 - roundRatio), rectangle.Y, rectSize.Width, rectSize.Height);
            RectangleF rectBottomLeft = new RectangleF(rectangle.X, rectangle.Y + rectangle.Height * (1 - roundRatio), rectSize.Width, rectSize.Height);
            RectangleF rectBottomRight = new RectangleF(rectangle.X + rectangle.Width * (1 - roundRatio), rectangle.Y + rectangle.Height * (1 - roundRatio), rectSize.Width, rectSize.Height);
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddArc(rectTopLeft, 180, 90);
            graphicsPath.AddArc(rectTopRight, 270, 90);
            graphicsPath.AddArc(rectBottomRight, 0, 90);
            graphicsPath.AddArc(rectBottomLeft, 90, 90);
            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        public static int ChangeZto35(char c)
        {
            if ('0' <= c && c <= '9') return (int)(c - '0');
            if ('A' <= c && c <= 'Z') return (int)(c - 'A') + 10;
            if ('a' <= c && c <= 'z') return (int)(c - 'a') + 10;
            return 0;
        }

        public static char Change35toZ(int num)
        {
            if (0 <= num && num <= 9) return (char)('0' + num);
            if (10 <= num && num <= 35) return (char)(num - 10 + 'A');
            return '0';
        }

        public static int ToIntAs36(string str)
        {
            str = "00000000" + str.ToUpper();
            str = str.Substring(str.Length - 8);
            int l = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                l = l * 36 + ChangeZto35(str[i]);
            }
            return l;
        }

        public static string ToStringAs36(int num)
        {
            if (num < 0) return "0";

            string str = "";
            do
            {
                str = Change35toZ(num % 36) + str;
                num = num / 36;
            } while (num > 0);
            return str;
        }

        public static int ToInt(string str)
        {
            /* 例外対策ちゃんと書かないとまずい */
            return Convert.ToInt32(str);
        }

        public static double ToDouble(string str)
        {
            /* 例外対策ちゃんと書かないとまずい */
            return Convert.ToDouble(str);
        }

        public static string ToString(double num)
        {
            /* 例外対策ちゃんと書かないとまずい */
            return Convert.ToString(num);
        }

        public static string GetRawString(string str)
        {
            if (str.Length == 0) return str;

            if (str[0] == '"' && str[str.Length - 1] == '"') return str.Substring(1, str.Length - 2);

            return str;
        }

        /// <summary>
        /// ノーツのNoteType情報をもとに、SUS仕様のノーツIDを取得します
        /// </summary>
        public static int GetSusNoteID(Note note)
        {
            switch (note.NoteType)
            {
                case NoteType.Tap: return 1;
                case NoteType.ExTap: return 2;
                case NoteType.Flick: return 3;
                case NoteType.Damage: return 4;
                case NoteType.AwExTap: return 5;
                case NoteType.ExTapDown: return 6;
                case NoteType.HoldBegin: return 1;
                case NoteType.HoldEnd: return 2;
                case NoteType.SlideBegin: return 1;
                case NoteType.SlideEnd: return 2;
                case NoteType.SlideTap: return 3;
                case NoteType.SlideCurve: return 4;
                case NoteType.SlideRelay: return 5;
                case NoteType.AirHoldBegin: return 1;
                case NoteType.AirHoldEnd: return 2;
                case NoteType.AirAction: return 3;
                case NoteType.AirUpC: return 1;
                case NoteType.AirDownC: return 2;
                case NoteType.AirUpL: return 3;
                case NoteType.AirUpR: return 4;
                case NoteType.AirDownR: return 5;
                case NoteType.AirDownL: return 6;
                default: return 0;
            }
        }
    }
}