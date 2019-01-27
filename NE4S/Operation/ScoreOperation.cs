using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Scores;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class SetScoreOperation : Operation
    {
        public SetScoreOperation(Model model, int beatNumer, int beatDenom, int barCount)
        {
            Invoke += () =>
            {
                model.SetScore(beatNumer, beatDenom, barCount);
            };
            Undo += () =>
            {
                for(int i = 0; i < barCount; ++i)
                {
                    Score score = model.ScoreBook.Last();
                    model.DeleteScore(score, 1);
                }
            };
        }
    }

    public class InsertScoreForwardOperation : Operation
    {
        public InsertScoreForwardOperation(
            Model model, Score score, int beatNumer, int beatDenom, int barCount)
        {
            Invoke += () =>
            {
                model.InsertScoreForward(score, beatNumer, beatDenom, barCount);
            };
            Undo += () =>
            {
                for(int i = 0; i < barCount; ++i)
                {
                    var scoreForDelete = model.ScoreBook.Next(score);
                    model.DeleteScore(scoreForDelete, 1);
                }
            };
        }
    }

    public class InsertScoreForwardWithNoteOperation : Operation
    {
        public InsertScoreForwardWithNoteOperation(
            Model model, Score score, int beatNumer, int beatDenom, int barCount)
        {
            Invoke += () =>
            {
                model.InsertScoreForwardWithNote(score, beatNumer, beatDenom, barCount);
            };
            Undo += () =>
            {
                for (int i = 0; i < barCount; ++i)
                {
                    var scoreForDelete = model.ScoreBook.Next(score);
                    model.DeleteScore(scoreForDelete, 1);
                }

                int deltaTick = barCount * ScoreInfo.MaxBeatDiv * beatNumer / beatDenom;
                model.NoteBook.RelocateNoteTickAfterScoreTick(
                    score.EndTick + 1, -deltaTick);
                model.LaneBook.OnUpdateNoteLocation();
            };
        }
    }

    public class InsertScoreBackwardOperation : Operation
    {
        public InsertScoreBackwardOperation(
            Model model, Score score, int beatNumer, int beatDenom, int barCount)
        {
            Invoke += () =>
            {
                model.InsertScoreBackward(score, beatNumer, beatDenom, barCount);
            };
            Undo += () =>
            {
                for (int i = 0; i < barCount; ++i)
                {
                    var scoreForDelete = model.ScoreBook.Prev(score);
                    model.DeleteScore(scoreForDelete, 1);
                }
            };
        }
    }

    public class InsertScoreBackwardWithNoteOperation : Operation
    {
        public InsertScoreBackwardWithNoteOperation(
            Model model, Score score, int beatNumer, int beatDenom, int barCount)
        {
            Invoke += () =>
            {
                model.InsertScoreBackwardWithNote(score, beatNumer, beatDenom, barCount);
            };
            Undo += () =>
            {
                for (int i = 0; i < barCount; ++i)
                {
                    var scoreForDelete = model.ScoreBook.Prev(score);
                    model.DeleteScore(scoreForDelete, 1);
                }
                int deltaTick = barCount * ScoreInfo.MaxBeatDiv * beatNumer / beatDenom;
                model.NoteBook.RelocateNoteTickAfterScoreTick(
                    score.StartTick, -deltaTick);
                model.LaneBook.OnUpdateNoteLocation();
            };
        }
    }

    public class DeleteScoreOperation : Operation
    {
        public DeleteScoreOperation(
            Model model, Score score, int count)
        {
            Score prev = model.ScoreBook.Prev(score);
            ScoreBook scoreList = new ScoreBook();
            Score itrScore = score;
            for (int i = 0; i < count; ++i)
            {
                scoreList.Add(itrScore);
                itrScore = model.ScoreBook.Next(itrScore);
            }
            Invoke += () =>
            {
                model.DeleteScore(scoreList.First(), count);
            };
            Undo += () =>
            {
                model.LaneBook.InsertScoreForward(
                    model.ScoreBook,
                    prev,
                    scoreList);
            };
        }
    }

    public class DeleteScoreWithNoteOperation : Operation
    {
        // UNDONE
        public DeleteScoreWithNoteOperation(
            Model model, Score score, int count)
        {
            Score prev = model.ScoreBook.Prev(score);
            ScoreBook scoreList = new ScoreBook();
            Score itrScore = score;
            for (int i = 0; i < count; ++i)
            {
                scoreList.Add(itrScore);
                itrScore = model.ScoreBook.Next(itrScore);
            }
            Invoke += () =>
            {
                model.DeleteScoreWithNote(scoreList.First(), count);
            };
            Undo += () =>
            {
                model.LaneBook.InsertScoreForwardWithNote(
                    model.NoteBook,
                    model.ScoreBook,
                    prev,
                    scoreList);
            };
        }
    }
}
