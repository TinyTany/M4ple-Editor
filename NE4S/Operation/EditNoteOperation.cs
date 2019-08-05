using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using NE4S.Define;
using NE4S.Scores;
using NE4S.Component;

namespace NE4S.Operation
{
    public class RelocateNoteOperation : Operation
    {
        public RelocateNoteOperation(Note note, Position before, Position after, LaneBook laneBook)
        {
            Invoke += () =>
            {
                note.Relocate(after);
                note.UpdateLocation(laneBook);
                if (note is AirableNote airable)
                {
                    airable.Air?.UpdateLocation(laneBook);
                    airable.AirHold?.UpdateLocation(laneBook);
                }
            };
            Undo += () =>
            {
                note.Relocate(before);
                note.UpdateLocation(laneBook);
                if (note is AirableNote airable)
                {
                    airable.Air?.UpdateLocation(laneBook);
                    airable.AirHold?.UpdateLocation(laneBook);
                }
            };
        }

        public RelocateNoteOperation(
            List<Note> noteList, List<LongNote> longNoteList, Position diff, LaneBook laneBook)
        {
            Invoke += () =>
            {
                noteList.ForEach(x =>
                {
                    Position positionAfter = new Position(
                        x.Position.Lane + diff.Lane,
                        x.Position.Tick + diff.Tick);
                    x.RelocateOnlyAndUpdate(positionAfter, laneBook);
                });
                longNoteList.ForEach(x =>
                {
                    x.ForEach(y =>
                    {
                        Position positionAfter = new Position(
                            y.Position.Lane + diff.Lane,
                            y.Position.Tick + diff.Tick);
                        y.RelocateOnlyAndUpdate(positionAfter, laneBook);
                    });
                });
            };
            Undo += () =>
            {
                noteList.ForEach(x =>
                {
                    Position positionAfter = new Position(
                        x.Position.Lane - diff.Lane,
                        x.Position.Tick - diff.Tick);
                    x.RelocateOnlyAndUpdate(positionAfter, laneBook);
                });
                longNoteList.ForEach(x =>
                {
                    x.ForEach(y =>
                    {
                        Position positionAfter = new Position(
                            y.Position.Lane - diff.Lane,
                            y.Position.Tick - diff.Tick);
                        y.RelocateOnlyAndUpdate(positionAfter, laneBook);
                    });
                });
            };
        }
    }

    public class ReSizeNoteOperation : Operation
    {
        public ReSizeNoteOperation(Note note, int sizeBefore, int sizeAfter, NoteArea noteArea)
        {
            Invoke += () =>
            {
                NoteArea tmp = Status.SelectedNoteArea;
                Status.SelectedNoteArea = noteArea;
                note.ReSize(sizeAfter);
                Status.SelectedNoteArea = tmp;
            };
            Undo += () =>
            {
                NoteArea tmp = Status.SelectedNoteArea;
                Status.SelectedNoteArea = noteArea;
                note.ReSize(sizeBefore);
                Status.SelectedNoteArea = tmp;
            };
        }
    }

    public class ChangeNoteValueOperation : Operation
    {
        public ChangeNoteValueOperation(AttributeNote note, float valueBefore, float valueAfter)
        {
            Invoke += () =>
            {
                note.NoteValue = valueAfter;
            };
            Undo += () =>
            {
                note.NoteValue = valueBefore;
            };
        }
    }

    /// <summary>
    /// 指定したSlide帯でSlideを2つに切断します
    /// </summary>
    public class CutSlideOperation : Operation
    {
        public CutSlideOperation(Model model, Slide slide, Note past, Note future)
        {
            #region 分割した手前側のSlideノーツを生成
            Slide before = new Slide();
            foreach(var note in slide.Where(x => x.Position.Tick < past.Position.Tick))
            {
                before.Add(note);
            }
            before.Add(new SlideEnd(past));
            #endregion
            #region 分割した奥側のSlideノーツを生成
            Slide after = new Slide();
            after.Add(new SlideBegin(future));
            foreach (var note in slide.Where(x => x.Position.Tick > future.Position.Tick))
            {
                after.Add(note);
            }
            #endregion
            Invoke += () =>
            {
                if (!slide.Any() || !before.Any() || !after.Any())
                {
                    Logger.Warn("空のSlideが存在します。", true);
                    return;
                }
                model.NoteBook.UnPut(slide);
                model.NoteBook.Put(before);
                model.NoteBook.Put(after);
            };
            Undo += () =>
            {
                if (!slide.Any() || !before.Any() || !after.Any())
                {
                    Logger.Warn("空のSlideが存在します。", true);
                    return;
                }
                model.NoteBook.Put(slide);
                model.NoteBook.UnPut(before);
                model.NoteBook.UnPut(after);
            };
        }
    }
    
    /// <summary>
    /// 指定した2つのSlideを1つのSlideへ結合します。
    /// 2つのSlideは時間順的に重なっていないことを前提とする。
    /// </summary>
    public class ConnectSlideOperation : Operation
    {
        /// <summary>
        /// 2つのSlideは時間順的に正しくなるように与えられていることを前提とする。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="past"></param>
        /// <param name="future"></param>
        public ConnectSlideOperation(Model model, Slide past, Slide future)
        {
            // 一応確認をしてアサートが出るようにはしておくか...
            System.Diagnostics.Debug.Assert(
                past.EndTick < future.StartTick, 
                "Slideの位置関係が不適切です。");
            #region 結合したSlide（union）を作成
            Slide union = new Slide();
            var end = past.Find(x => x is SlideEnd);
            System.Diagnostics.Debug.Assert(end != null, "Slide終点が見つかりません。");
            foreach(var note in past.Where(x => x != end))
            {
                union.Add(note);
            }
            union.Add(new Note(end) as SlideTap);
            var begin = future.Find(x => x is SlideBegin);
            System.Diagnostics.Debug.Assert(begin != null, "Slide始点が見つかりません。");
            union.Add(new Note(begin) as SlideTap);
            foreach(var note in future.Where(x => x != begin))
            {
                union.Add(note);
            }
            #endregion
            Invoke += () =>
            {
                model.NoteBook.Put(union);
                model.NoteBook.UnPut(past);
                model.NoteBook.UnPut(future);
            };
            Undo += () =>
            {
                model.NoteBook.Put(past);
                model.NoteBook.Put(future);
                model.NoteBook.UnPut(union);
            };
        }
    }

    /// <summary>
    /// 指定したSlideノーツのパターンを複製します
    /// </summary>
    public class ReplicateSlidePatternOperation : Operation
    {
        public ReplicateSlidePatternOperation(
            Model model, List<PartialSlide> partialList, int tickInterval)
        {
            var slideList = new List<Slide>();
            var afterList = new List<Slide>();
            partialList.ForEach(x => slideList.Add(x.Slide));
            #region パターン複製処理（複製したSlideをafterListに格納）
            foreach(var x in partialList)
            {
                if (x.Slide == null || x.Partial == null || !x.Partial.Any())
                {
                    System.Diagnostics.Debug.Assert(false, "パターン複製に失敗しました");
                    continue;
                }
                var after = new Slide(x.Slide);
                var pattern = ReplicatePattern(x.Partial).OrderBy(y => y.Position.Tick).ToList();
                var patternLength = pattern.Last().Position.Tick - pattern.First().Position.Tick;
                pattern.ForEach(y =>
                {
                    var newTick = y.Position.Tick + patternLength + tickInterval;
                    var sameTickNote = after.Find(z => z.Position.Tick == newTick);
                    if (sameTickNote != null && !(sameTickNote is SlideEnd)) { after.Remove(sameTickNote); }
                    y.RelocateOnly(new Position(y.Position.Lane, newTick));
                    after.Add(y);
                });
                var end = after.Find(y => y is SlideEnd);
                System.Diagnostics.Debug.Assert(end != null, "無理");
                // NOTE: この時点でのafterのLast()は追加されたパターンの一番最後のノーツになるはず
                var last = after.Last();
                if (last.Position.Tick >= end.Position.Tick)
                {
                    end.RelocateOnly(last.Position);
                    after.Remove(last);
                }
                afterList.Add(after);
            }
            #endregion
            Invoke += () =>
            {
                var lst = model.NoteBook.SlideNotes.Where(x => slideList.Contains(x));
                model.NoteBook.UnPutRange(lst);
                model.NoteBook.PutRange(afterList);
            };
            Undo += () =>
            {
                var lst = model.NoteBook.SlideNotes.Where(x => afterList.Contains(x));
                model.NoteBook.UnPutRange(lst);
                model.NoteBook.PutRange(slideList);
            };
        }

        public ReplicateSlidePatternOperation(
            Model model, Slide slide, List<Note> pattern, int times, int tickInterval)
        {
            #region 複製したSlideを新規作成
            var after = new Slide(slide);
            var patternLength =
                pattern.OrderBy(x => x.Position.Tick).Last().Position.Tick -
                pattern.OrderBy(x => x.Position.Tick).First().Position.Tick;
            for (int i = 1; i <= times; ++i)
            {
                var addPattern = ReplicatePattern(pattern);
                addPattern.ForEach(x =>
                {
                    var tick = x.Position.Tick;
                    tick += i * (patternLength + tickInterval);
                    x.RelocateOnly(new Position(x.Position.Lane, tick));
                    slide.Add(x);
                    var sameTick = slide.Find(y => y.Position.Tick == x.Position.Tick);
                    if (sameTick != null)
                    {
                        slide.Remove(sameTick);
                    }
                });
            }
            var slideEnd = after.FindLast(x => x is SlideEnd);
            var endTap = after.Last();
            if (slideEnd.Position.Tick <= endTap.Position.Tick)
            {
                slideEnd.RelocateOnly(endTap.Position);
                after.Remove(endTap);
            }
            #endregion
            Invoke += () =>
            {
                model.NoteBook.UnPut(slide);
                model.NoteBook.Put(after);
            };
            Undo += () =>
            {
                model.NoteBook.UnPut(after);
                model.NoteBook.Put(slide);
            };
        }

        private static bool IsPatternValid(Slide slide, List<Note> pattern)
        {
            if (pattern.Count < 2) { return false; }
            foreach(var note in pattern)
            {
                if (!slide.Contains(note)) { return false; }
            }
            var patternStartTick = pattern.OrderBy(x => x.Position.Tick).First().Position.Tick;
            var patternEndTick = pattern.OrderBy(x => x.Position.Tick).Last().Position.Tick;
            if (slide.Where(
                x => patternStartTick <= x.Position.Tick && x.Position.Tick <= patternEndTick)
                .Count() != pattern.Count) { return false; }
            return true;
        }

        /// <summary>
        /// Slideを構成するノーツのリストを複製しSlide中継点ノーツとしたリストを返します
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private static List<Note> ReplicatePattern(List<Note> pattern)
        {
            // NOTE: SlideBeginかSlideEndがきたらひとまずSlideTapにすることにする
            var replica = new List<Note>();
            foreach(var note in pattern)
            {
                if(note is SlideBegin) { replica.Add(new SlideTap(note)); }
                else if (note is SlideTap) { replica.Add(new SlideTap(note)); }
                else if (note is SlideRelay) { replica.Add(new SlideRelay(note)); }
                else if (note is SlideCurve) { replica.Add(new SlideCurve(note)); }
                else if (note is SlideEnd) { replica.Add(new SlideTap(note)); }
            }
            return replica;
        }
    }

    /// <summary>
    /// 指定したロングノーツを同種ロングノーツ内で最前面に配置します
    /// </summary>
    public class LongNoteToFrontOperation : Operation
    {
        public LongNoteToFrontOperation(Model model, LongNote longNote)
        {
            var noteBook = model.NoteBook;
            var index = -1;
            bool result = false;
            Invoke += () =>
            {
                if (longNote is Hold hold)
                {
                    index = noteBook.HoldNotes.ToList().IndexOf(hold);
                    result = noteBook.MoveIndexTo(int.MaxValue, hold);
                }
                else if (longNote is Slide slide)
                {
                    index = noteBook.SlideNotes.ToList().IndexOf(slide);
                    result = noteBook.MoveIndexTo(int.MaxValue, slide);
                }
                else if (longNote is AirHold airHold)
                {
                    index = noteBook.AirHoldNotes.ToList().IndexOf(airHold);
                    result = noteBook.MoveIndexTo(int.MaxValue, airHold);
                }
                if (!result)
                {
                    Logger.Error("操作に失敗しました。", true);
                }
            };
            Undo += () =>
            {
                if (!noteBook.MoveIndexTo(index, longNote))
                {
                    Logger.Error("操作に失敗しました。", true);
                }
            };
        }
    }

    /// <summary>
    /// 指定したロングノーツを同種ロングノーツ内で最背面に配置します
    /// </summary>
    public class LongNoteToBackOperation : Operation
    {
        public LongNoteToBackOperation(Model model, LongNote longNote)
        {
            var noteBook = model.NoteBook;
            var index = -1;
            bool result = false;
            Invoke += () =>
            {
                if (longNote is Hold hold)
                {
                    index = noteBook.HoldNotes.ToList().IndexOf(hold);
                    result = noteBook.MoveIndexTo(0, hold);
                }
                else if (longNote is Slide slide)
                {
                    index = noteBook.SlideNotes.ToList().IndexOf(slide);
                    result = noteBook.MoveIndexTo(0, slide);
                }
                else if (longNote is AirHold airHold)
                {
                    index = noteBook.AirHoldNotes.ToList().IndexOf(airHold);
                    result = noteBook.MoveIndexTo(0, airHold);
                }
                if (!result)
                {
                    Logger.Error("操作に失敗しました。", true);
                }
            };
            Undo += () =>
            {
                if (!noteBook.MoveIndexTo(index, longNote))
                {
                    Logger.Error("操作に失敗しました。", true);
                }
            };
        }
    }

    public class SlideTapToRelayOperation : Operation
    {
        public SlideTapToRelayOperation(Slide slide, List<Note> noteList)
        {
            var tapList = noteList.Where(x => slide.Contains(x) && x is SlideTap).ToList();
            var relayList = new List<SlideRelay>();
            tapList.ForEach(x => relayList.Add(new SlideRelay(x)));
            Invoke += () =>
            {
                tapList.ForEach(x => slide.Remove(x));
                relayList.ForEach(x => slide.Add(x));
            };
            Undo += () =>
            {
                relayList.ForEach(x => slide.Remove(x));
                tapList.ForEach(x => slide.Add(x));
            };
        }
    }

    public class SlideRelayToTapOperation : Operation
    {
        public SlideRelayToTapOperation(Slide slide, List<Note> noteList)
        {
            var tapList = new List<SlideTap>();
            var relayList = noteList.Where(x => slide.Contains(x) && x is SlideRelay).ToList();
            relayList.ForEach(x => tapList.Add(new SlideTap(x)));
            Invoke += () =>
            {
                relayList.ForEach(x => slide.Remove(x));
                tapList.ForEach(x => slide.Add(x));
            };
            Undo += () =>
            {
                tapList.ForEach(x => slide.Remove(x));
                relayList.ForEach(x => slide.Add(x));
            };
        }
    }

    public class SlideTapRelayReverseOperation : Operation
    {
        public SlideTapRelayReverseOperation(Slide slide, List<Note> noteList)
        {
            var tapList = noteList.Where(x => slide.Contains(x) && x is SlideTap).ToList();
            var relayList = noteList.Where(x => slide.Contains(x) && x is SlideRelay).ToList();
            var tapToRelay = new SlideTapToRelayOperation(slide, tapList);
            var relayToTap = new SlideRelayToTapOperation(slide, relayList);
            Invoke += () =>
            {
                tapToRelay.Invoke();
                relayToTap.Invoke();
            };
            Undo += () =>
            {
                relayToTap.Undo();
                tapToRelay.Undo();
            };
        }
    }
}
