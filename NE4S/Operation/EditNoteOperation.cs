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
        public ReSizeNoteOperation(Note note, int sizeBefore, int sizeAfter, int noteArea)
        {
            Invoke += () =>
            {
                int tmp = Status.SelectedNoteArea;
                Status.SelectedNoteArea = noteArea;
                note.ReSize(sizeAfter);
                Status.SelectedNoteArea = tmp;
            };
            Undo += () =>
            {
                int tmp = Status.SelectedNoteArea;
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
    /// Slideを指定の分数ごとでTick方向にSlideTapまたはSldeRelayで分割します
    /// </summary>
    public class DivideSlideOperation : Operation
    {
        public DivideSlideOperation(Slide slide, Note stepPast, Note stepFuture)
        {
            int stepAddCount = 
                (stepFuture.Position.Tick - stepPast.Position.Tick) * Status.Beat / 192;
            Invoke += () =>
            {
                // UNDONE
            };
            Undo += () =>
            {
                // UNDONE
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
            before.Add(new Note(past) as SlideEnd);
            #endregion
            #region 分割した奥側のSlideノーツを生成
            Slide after = new Slide();
            after.Add(new Note(future) as SlideBegin);
            foreach (var note in slide.Where(x => x.Position.Tick > future.Position.Tick))
            {
                after.Add(note);
            }
            #endregion
            Invoke += () =>
            {
#if DEBUG
                if (!slide.Any() || !before.Any() || !after.Any())
                {
                    System.Diagnostics.Debug.Assert(false, "空のSlideが存在します。");
                    return;
                }
#endif
                model.NoteBook.Delete(slide);
                model.NoteBook.Add(before);
                model.NoteBook.Add(after);
            };
            Undo += () =>
            {
#if DEBUG
                if (!slide.Any() || !before.Any() || !after.Any())
                {
                    System.Diagnostics.Debug.Assert(false, "空のSlideが存在します。");
                    return;
                }
#endif
                model.NoteBook.Add(slide);
                model.NoteBook.Delete(before);
                model.NoteBook.Delete(after);
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
                model.NoteBook.Add(union);
                model.NoteBook.Delete(past);
                model.NoteBook.Delete(future);
            };
            Undo += () =>
            {
                model.NoteBook.Add(past);
                model.NoteBook.Add(future);
                model.NoteBook.Delete(union);
            };
        }
    }

    /// <summary>
    /// あるSlideノーツの指定した範囲のノーツのパターンを複製します
    /// </summary>
    public class ReplicateSlidePatternOperation : Operation
    {

    }
}
