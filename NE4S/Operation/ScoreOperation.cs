using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Scores;

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
            List<Score> scoreList = new List<Score>();
            Score prev = model.ScoreBook.Prev(score);
            Invoke += () =>
            {
                Score itrScore = score;
                for(int i = 0; i < count; ++i)
                {
                    scoreList.Add(itrScore);
                    itrScore = model.ScoreBook.Next(itrScore);
                }
                model.DeleteScore(score, count);
            };
            Undo += () =>
            {
                scoreList.ForEach(
                    x => model.InsertScoreForward(
                        prev, x.BeatNumer, x.BeatDenom, 1));
            };
        }
    }

    public class DeleteScoreWithNoteOperation : Operation
    {
        public DeleteScoreWithNoteOperation(
            Model model, Score score, int count)
        {
            Invoke += () =>
            {
                model.DeleteScoreWithNote(score, count);
            };
            Undo += () =>
            {
                //UNDONE
            };
        }
    }
}
