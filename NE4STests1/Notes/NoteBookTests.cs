using Microsoft.VisualStudio.TestTools.UnitTesting;
using NE4S.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NE4S.Notes.Concrete;
using NE4S.Data;
using NE4S.Notes.Abstract;

namespace NE4S.Notes.Tests
{
    [TestClass()]
    public class NoteBookTests
    {
        [TestMethod()]
        public void ContainsTest()
        {
            var book = new NoteBook();
            var tap = new Tap();
            book.Put(tap);
            Assert.IsTrue(book.Contains(tap));
            Assert.AreEqual(true, book.Contains(tap as AirableNote));
            var airable = tap as AirableNote;
            Assert.AreEqual(true, book.Contains(airable));
            var note = airable as Note;
            Assert.AreEqual(true, book.Contains(note));

            var slide = new Slide();
            slide.Add(new SlideBegin());
            slide.Add(new SlideEnd());
            var rnd = new Random();
            for (int i = 0; i < 100; ++i)
            {
                switch (rnd.Next() % 3)
                {
                    case 0: slide.Add(new SlideTap()); break;
                    case 1: slide.Add(new SlideRelay()); break;
                    case 2: slide.Add(new SlideCurve()); break;
                    default: break;
                }
            }
            var sw = new Stopwatch();
            {
                sw.Start();
                var copy = new Slide(slide);
                sw.Stop();
                Debug.WriteLine($"Copy by constructor(Slide) : {sw.ElapsedMilliseconds} [ms]");
            }
            sw.Reset();
            {
                sw.Start();
                var copy = MyUtil.DeepCopy(slide);
                sw.Stop();
                Debug.WriteLine($"Copy by method(Slide) : {sw.ElapsedMilliseconds} [ms]");
            }
            // コピーコンストラクタの方法よりもディープコピーの方法のほうがだいたい10倍くらい早い（型を調べてるから当然ではあるけど）

            var target = new List<Position>();
            for(int i = 0; i < 10000; ++i)
            {
                target.Add(new Position(rnd.Next(), rnd.Next()));
            }
            sw.Reset();
            {
                sw.Start();
                var copy = new List<Position>(target);
                sw.Stop();
                Debug.WriteLine($"Copy by constructor(List<Position>) : {sw.ElapsedMilliseconds} [ms]");
            }
            sw.Reset();
            {
                sw.Start();
                var copy = MyUtil.DeepCopy(target);
                sw.Stop();
                Debug.WriteLine($"Copy by method(List<Position>) : {sw.ElapsedMilliseconds} [ms]");
            }
            // こっちだとディープコピーのほうが普通に遅い（Listのコンストラクタのほうは要素数10000でも0ms）

            sw.Reset();
            {
                sw.Start();
                foreach(var item in target)
                {
                    var copy = new Position(item);
                }
                sw.Stop();
                Debug.WriteLine($"Copy by constructor(Position) : {sw.ElapsedMilliseconds} [ms]");
            }
            sw.Reset();
            {
                sw.Start();
                foreach (var item in target)
                {
                    var copy = MyUtil.DeepCopy(item);
                }
                sw.Stop();
                Debug.WriteLine($"Copy by method(Position) : {sw.ElapsedMilliseconds} [ms]");
            }
            // これもコピーコンストラクタのほうが早い

            // 基本はコピーコンストラクタを使い、型を調べる必要があるならディープコピーのほうが良い？
        }
    }
}