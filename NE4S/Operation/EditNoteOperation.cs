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
}
