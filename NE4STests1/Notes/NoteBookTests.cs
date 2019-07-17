using Microsoft.VisualStudio.TestTools.UnitTesting;
using NE4S.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            book.PutNote(tap);
            Assert.IsTrue(book.Contains(tap));
            Assert.AreEqual(true, book.Contains(tap as AirableNote));
            var airable = tap as AirableNote;
            Assert.AreEqual(true, book.Contains(airable));
            var note = airable as Note;
            Assert.AreEqual(true, book.Contains(note));
        }
    }
}