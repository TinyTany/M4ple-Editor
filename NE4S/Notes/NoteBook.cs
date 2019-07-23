using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Scores;
using NE4S.Define;
using System.Diagnostics;

namespace NE4S.Notes
{
    /// <summary>
    /// 置かれている全ノーツをまとめる
    /// </summary>
    [Serializable()]
    public class NoteBook
    {
        private readonly List<Note> shortNotes = new List<Note>();
        private readonly List<Hold> holdNotes = new List<Hold>();
        private readonly List<Slide> slideNotes = new List<Slide>();
        private readonly List<AirHold> airHoldNotes = new List<AirHold>();
        private readonly List<Air> airNotes = new List<Air>();
        private readonly List<AttributeNote> attributeNotes = new List<AttributeNote>();

        public NoteBook()
        {
            // HACK: 開始時BPMを無理やり設定
            Status.CurrentValue = 120;
            attributeNotes.Add(new BPM(
                new Position(0, 0),
                new PointF(
                    ScorePanel.Margin.Left + ScoreLane.Margin.Left,
                    ScorePanel.Margin.Top + ScoreLane.Height - ScoreLane.Margin.Bottom),
                Status.CurrentValue,
                0));
        }

        /// <summary>
        /// あるノーツが配置済みノーツリスト内に存在するかどうかを判断します。
        /// </summary>
        public bool Contains(Note note)
        {
            if (note == null)
            {
                Logger.Error("引数noteがnullです。", true);
                return false;
            }
            switch (note)
            {
                case Tap _:
                case ExTap _:
                case AwesomeExTap _:
                case ExTapDown _:
                case Flick _:
                case HellTap _:
                    {
                        return shortNotes.Contains(note);
                    }
                case Air air:
                    {
                        return airNotes.Contains(air);
                    }
                case HoldBegin _:
                case HoldEnd _:
                    {
                        var hold = holdNotes.Find(x => x.Contains(note));
                        return hold != null;
                    }
                case SlideBegin _:
                case SlideEnd _:
                case SlideTap _:
                case SlideRelay _:
                case SlideCurve _:
                    {
                        var slide = slideNotes.Find(x => x.Contains(note));
                        return slide != null;
                    }
                case AirHoldBegin _:
                case AirHoldEnd _:
                case AirAction _:
                    {
                        var ah = airHoldNotes.Find(x => x.Contains(note));
                        return ah != null;
                    }
                case AttributeNote att:
                    {
                        return attributeNotes.Contains(att);
                    }
                default:
                    {
                        Logger.Warn("不明なノーツです。");
                        return false;
                    }
            }
        }

        /// <summary>
        /// あるロングノーツが配置済みノーツリスト内に存在するかどうかを判断します。
        /// </summary>
        public bool Contains(LongNote lnote)
        {
            if (lnote == null)
            {
                Logger.Error("引数longNoteがnullです。", true);
                return false;
            }
            switch (lnote)
            {
                case Hold hold:
                    {
                        return holdNotes.Contains(hold);
                    }
                case Slide slide:
                    {
                        return slideNotes.Contains(slide);
                    }
                case AirHold ah:
                    {
                        return airHoldNotes.Contains(ah);
                    }
                default:
                    {
                        Logger.Warn("不明なロングノーツです。");
                        return false;
                    }
            }
        }

        /// <summary>
        /// ショートノーツまたはアトリビュートノーツを配置します。
        /// ショートノーツにAirやAirHoldが付随していた場合、それらも配置します。
        /// </summary>
		public bool Put(Note note)
		{
            if (note == null)
            {
                Logger.Error("ノーツを追加できません。引数newNoteがnullです。", true);
                return false;
            }
            switch (note)
            {
                case Tap _:
                case ExTap _:
                case AwesomeExTap _:
                case ExTapDown _:
                case Flick _:
                case HellTap _:
                    {
                        shortNotes.Add(note);
                        var airable = note as AirableNote;
                        if (airable.IsAirAttached)
                        {
                            airNotes.Add(airable.Air);
                        }
                        if (airable.IsAirHoldAttached)
                        {
                            airHoldNotes.Add(airable.AirHold);
                        }
                        return true;
                    }
                case AttributeNote att:
                    {
                        attributeNotes.Add(att);
                        return true;
                    }
                default:
                    {
                        Logger.Warn("不適切なノーツを追加できません。", true);
                        return false;
                    }
            }
        }

        /// <summary>
        /// Hold, Slideを配置します。
        /// </summary>
        public bool Put(LongNote longNote)
        {
            if (longNote == null)
            {
                Logger.Error("ロングノーツを追加できません。引数がnullです。", true);
                return false;
            }
            switch (longNote)
            {
                case Hold hold:
                    {
                        holdNotes.Add(hold);
                    }
                    break;
                case Slide slide:
                    {
                        slideNotes.Add(slide);
                    }
                    break;
                default:
                    {
                        Logger.Warn("不適切なロングノーツを追加できません。", true);
                        return false;
                    }
            }
            return true;
        }

        public static bool _PutStepToSlide(Slide slide, Note step)
        {
            if (slide == null || step == null)
            {
                Logger.Error("引数にnullが含まれるため、操作を行えません。", true);
                return false;
            }
            switch (step)
            {
                case SlideTap _:
                case SlideRelay _:
                case SlideCurve _:
                    {
                        if (!slide.Add(step))
                        {
                            Logger.Warn("スライドへのステップノーツ追加に失敗しました。");
                            return false;
                        }
                        return true;
                    }
                default:
                    {
                        Logger.Warn("スライドのステップノーツとして不適切なノーツのため操作を行いません。", true);
                        return false;
                    }
            }
        }

        public bool _PutStepToAirHold(AirHold airHold, Note step)
        {
            // UNDONE
            throw new NotImplementedException();
            //return true;
        }

        /// <summary>
        /// 配置済みAirableノーツに対して新規Airノーツを配置し取り付けます。
        /// </summary>
        public bool AttachAirToAirableNote(AirableNote airable, Air air)
        {
            if (airable == null || air == null)
            {
                Logger.Error("Airを取り付けできません。引数にnullが含まれます。", true);
                return false;
            }
            if (!Contains(airable))
            {
                Logger.Error("Airノーツ取り付け先のAirableノーツはすでに配置されている必要があります。");
                return false;
            }
            if (airable.IsAirAttached)
            {
                Logger.Error("Air取り付け先のAirableノーツにはすでにAirが取り付けられています。");
                return false;
            }
            airable.AttachAir(air);
            airNotes.Add(air);
            return true;
        }

        /// <summary>
        /// 配置済みショートノーツに対して新規AirHoldと新規AirUpCを配置し取り付けます。
        /// </summary>
        public bool AttachAirHoldToAirableNote(AirableNote airable, AirHold airHold, AirUpC air)
        {
            if (airable == null || airHold == null || air == null)
            {
                Logger.Error("AirHoldを取り付けできません。引数にnullが含まれます。", true);
                return false;
            }
            if (!Contains(airable))
            {
                Logger.Error("AirHold取り付け先のAirableノーツはすでに配置されている必要があります。", true);
                return false;
            }
            if (!airable.IsAirHoldAttachable)
            {
                Logger.Error("AirHold取り付け先のAirableノーツにはすでにAirやAirHoldが取り付けられています。", true);
                return false;
            }
            airable.AttachAirHold(airHold);
            airHoldNotes.Add(airHold);
            if (!airable.IsAirAttached)
            {
                airable.AttachAir(air);
                airNotes.Add(air);
            }
            return true;
        }

        /// <summary>
        /// 配置されているショートノーツまたはアトリビュートノーツを取り除きます。
        /// ショートノーツにAirやAirHoldが付随していた場合、それらも除去しますが、ショートノーツとの取り外しは行いません。
        /// </summary>
        public bool UnPut(Note note)
        {
            if (note == null)
            {
                Logger.Error("引数noteがnullのため処理を行えません。", true);
                return false;
            }
            switch (note)
            {
                case Tap _:
                case ExTap _:
                case AwesomeExTap _:
                case ExTapDown _:
                case Flick _:
                case HellTap _:
                    {
                        if (!shortNotes.Remove(note))
                        {
                            Logger.Warn("ShortNoteの削除に失敗しました。", true);
                            return false;
                        }
                        var airable = note as AirableNote;
                        airNotes.Remove(airable.Air);
                        airHoldNotes.Remove(airable.AirHold);
                        return true;
                    }
                case AttributeNote att:
                    {
                        if (!attributeNotes.Remove(att))
                        {
                            Logger.Warn("AttributeNoteの削除に失敗しました。", true);
                            return false;
                        }
                        return true;
                    }
                default:
                    {
                        Logger.Error("対象のnoteはこの操作で削除できません。", true);
                        return false;
                    }
            }
        }

        /// <summary>
        /// Hold, Slideを取り除きます。
        /// 終端ノーツにAirやAirHoldが付随していた場合、それらも除去しますが、Hold,Slideとの取り外しはしません。
        /// </summary>
        public bool UnPut(LongNote lnote)
        {
            if (lnote == null)
            {
                Logger.Error("引数lnoteがnullのため処理を行えません。", true);
                return false;
            }
            switch (lnote)
            {
                case Hold hold:
                    {
                        if (!holdNotes.Remove(hold))
                        {
                            Logger.Warn("Holdノーツの削除に失敗しました。");
                            return false;
                        }
                        var end = hold.EndNote as AirableNote;
                        airNotes.Remove(end?.Air);
                        airHoldNotes.Remove(end?.AirHold);
                        return true;
                    }
                case Slide slide:
                    {
                        if (!slideNotes.Remove(slide))
                        {
                            Logger.Warn("Slideノーツの削除に失敗しました。");
                            return false;
                        }
                        var end = slide.EndNote as AirableNote;
                        airNotes.Remove(end?.Air);
                        airHoldNotes.Remove(end?.AirHold);
                        return true;
                    }
                case AirHold _:
                    {
                        Logger.Error("AirHoldはこの操作では削除できません。", true);
                        return false;
                    }
                default:
                    {
                        Logger.Error("不明なロングノーツです。", true);
                        return false;
                    }
            }
        }

        public bool _UnPutStepFromSlide(Slide slide, out Note step)
        {
            step = null;
            // UNDONE
            throw new NotImplementedException();
            //return true;
        }

        public bool _UnPutStepFromAirHold(AirHold airHold, out Note step)
        {
            step = null;
            // UNDONE
            throw new NotImplementedException();
            //return true;
        }

        /// <summary>
        /// AirableノーツからAirを取り外します。
        /// 失敗した場合、出力引数はnullになります。
        /// </summary>
        public bool DetachAirFromAirableNote(AirableNote airable, out Air air)
        {
            air = null;
            if (airable == null)
            {
                Logger.Error("引数のAirableNoteがnullのため、操作を行えません。");
                return false;
            }
            if (!airable.IsAirAttached)
            {
                Logger.Error("Air取り外し対象のAirableNoteにはAirが取り付けられていませんでした。");
                return false;
            }
            air = airable.Air;
            airable.DetachAir();
            airNotes.Remove(air);
            return true;
        }

        /// <summary>
        /// AirableノーツからAirHoldおよびAirUpCを取り外します。
        /// AirHoldを取り外せた場合成功（true）となりますが、その場合AirUpCの出力引数がnullである可能性もあります。
        /// </summary>
        public bool DetachAirHoldFromAirableNote(AirableNote airable, out AirHold airHold, out AirUpC air)
        {
            airHold = null;
            air = null;
            if (airable == null)
            {
                Logger.Error("引数のAirableNoteがnullのため、操作を行えません。");
                return false;
            }
            if (!airable.IsAirHoldAttached)
            {
                Logger.Error("AirHold取り外し対象のAirableNoteにはAirHoldが取り付けられていませんでした。");
                return false;
            }
            airHold = airable.AirHold;
            airable.DetachAirHold();
            airHoldNotes.Remove(airHold);
            if (airable.IsAirAttached)
            {
                // NOTE: 本来AirHoldは単体で配置できず、かならずAirUpCが伴うはずであるが、何らかの原因でそうではない場合のために処理を分岐する。
                air = airable.Air as AirUpC;
                if (air != null)
                {
                    airable.DetachAir();
                    airNotes.Remove(air);
                }
                else
                {
                    // NOTE: 取り付けられていたAirがAirUpCでないときはAirを取り外さず、出力引数airもnullとなる。
                    Logger.Warn("取り付けられていたAirはAirUpCではありませんでした。Airを削除しません。");
                }
            }
            return true;
        }

        /// <summary>
        /// クリックされてるノーツがあったら投げる
        /// なかったらnullを投げる
        /// ノーツのどのへんがクリックされたかも特定する
        /// </summary>
        public Note SelectedNote(PointF location, ref NoteArea noteArea)
        {
            Note selectedNote;
            //AirHold
            foreach (AirHold airHold in airHoldNotes.Reverse<AirHold>())
            {
                if (!Status.IsAirHoldVisible) break;
                selectedNote = airHold.Find(x => x.Contains(location));
                if (selectedNote != null && !(selectedNote is AirHoldBegin))
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //Air
            selectedNote = airNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null && Status.IsAirVisible)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            //ShortNote
            selectedNote = shortNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null && Status.IsShortNoteVisible)
            {
                MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                return selectedNote;
            }
            //Slide
            foreach (Slide slide in slideNotes.Reverse<Slide>())
            {
                if (!Status.IsSlideVisible) break;
                selectedNote = slide.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //Hold
            foreach (Hold hold in holdNotes.Reverse<Hold>())
            {
                if (!Status.IsHoldVisible) break;
                selectedNote = hold.Find(x => x.Contains(location));
                if (selectedNote != null)
                {
                    MyUtil.SetNoteArea(selectedNote, location, ref noteArea);
                    return selectedNote;
                }
            }
            //AttributeNote
            selectedNote = attributeNotes.FindLast(x => x.Contains(location));
            if (selectedNote != null)
            {
                noteArea = NoteArea.Center;
                return selectedNote;
            }
            return null;
        }

        /// <summary>
        /// クリックされてるノーツがあったら投げる
        /// なかったらnullを投げる
        /// </summary>
        public Note SelectedNote(in PointF location)
        {
            // NOTE: この変数は使用しない
            NoteArea _area = NoteArea.None;
            return SelectedNote(location, ref _area);
        }

        public Slide SelectedSlide(PointF locationVirtual, LaneBook laneBook)
        {
            return slideNotes.FindLast(x => x.Contains(locationVirtual, laneBook));
        }

        public AirHold SelectedAirHold(PointF locationVirtual, LaneBook laneBook)
        {
            return airHoldNotes.FindLast(x => x.Contains(locationVirtual, laneBook));
        }

        public void UpdateNoteLocation(LaneBook laneBook)
        {
            shortNotes.ForEach(x => x.UpdateLocation(laneBook));
            holdNotes.ForEach(x => x.UpdateLocation(laneBook));
            slideNotes.ForEach(x => x.UpdateLocation(laneBook));
            airHoldNotes.ForEach(x => x.UpdateLocation(laneBook));
            airNotes.ForEach(x => x.UpdateLocation(laneBook));
            attributeNotes.ForEach(x => x.UpdateLocation(laneBook));
        }

        public void RelocateNoteTickAfterScoreTick(int scoreTick, int deltaTick)
        {
            shortNotes.
                Where(x => x.Position.Tick >= scoreTick).ToList().
                ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
            holdNotes.ForEach(x => x.RelocateNoteTickAfterScoreTick(scoreTick, deltaTick));
            slideNotes.ForEach(x => x.RelocateNoteTickAfterScoreTick(scoreTick, deltaTick));
            airHoldNotes.ForEach(x => x.RelocateNoteTickAfterScoreTick(scoreTick, deltaTick));
            airNotes.
                Where(x => x.Position.Tick >= scoreTick).ToList().
                ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
            attributeNotes.
                Where(x => x.Position.Tick >= scoreTick).ToList().
                ForEach(x => x.RelocateOnly(new Position(x.Position.Lane, x.Position.Tick + deltaTick)));
        }

        public List<Note> GetNotesFromTickRange(int startTick, int endTick)
        {
            var notes = shortNotes.Where(x => startTick <= x.Position.Tick && x.Position.Tick <= endTick);
            slideNotes.ForEach(
                x =>
                {
                    var list = x.Where(y => startTick <= y.Position.Tick && y.Position.Tick <= endTick);
                    if(!(list.Where(y => y is SlideBegin || y is SlideEnd).Any()))
                    {
                        notes = notes.Union(list);
                    }
                });
            airHoldNotes.ForEach(
                x =>
                {
                    var list = x.Where(y => startTick <= y.Position.Tick && y.Position.Tick <= endTick);
                    if (!(list.Where(y => y is AirHoldBegin || y is AirHoldEnd).Any()))
                    {
                        notes = notes.Union(list);
                    }
                });
            notes = notes.Union(airNotes.Where(x => startTick <= x.Position.Tick && x.Position.Tick <= endTick));
            notes = notes.Union(attributeNotes.Where(x => startTick <= x.Position.Tick && x.Position.Tick <= endTick));
            return notes.ToList();
        }

        public List<LongNote> GetLongNotesFromTickRange(int startTick, int endTick)
        {
            var longNotes = new List<LongNote>();
            holdNotes.ForEach(
                x =>
                {
                    if ((startTick <= x.StartTick && x.StartTick <= endTick) || (startTick <= x.EndTick && x.EndTick <= endTick))
                    {
                        longNotes.Add(x);
                    }
                });
            slideNotes.ForEach(
                x =>
                {
                    if ((startTick <= x.StartTick && x.StartTick <= endTick) || (startTick <= x.EndTick && x.EndTick <= endTick))
                    {
                        longNotes.Add(x);
                    }
                });
            airHoldNotes.ForEach(
                x =>
                {
                    if ((startTick <= x.StartTick && x.StartTick <= endTick) || (startTick <= x.EndTick && x.EndTick <= endTick))
                    {
                        longNotes.Add(x);
                    }
                });
            return longNotes;
        }

        public void Paint(Graphics g, Point drawLocation, LaneBook laneBook)
		{
            // NOTE: 先にコードを記述したノーツほど下に描画される

            // AttributeNote
            if (attributeNotes != null)
            {
                var drawNotes = attributeNotes
                    .Where(x => x.Position.Tick >= Status.DrawTickFirst && x.Position.Tick <= Status.DrawTickLast);
                foreach(var note in drawNotes)
                {
                    note.Draw(g, drawLocation);
                }
            }
            // Hold
            if (holdNotes != null && Status.IsHoldVisible)
            {
                var drawNotes = holdNotes.Where(x => x.IsDrawable());
                foreach(var note in drawNotes)
                {
                    note.Draw(g, drawLocation, laneBook);
                }
            }
            // Slide
            if (slideNotes != null && Status.IsSlideVisible)
            {
                var drawNotes = slideNotes.Where(x => x.IsDrawable());
                foreach(var note in drawNotes)
                {
                    note.Draw(g, drawLocation, laneBook);
                }
            }
            // ShortNote
            if (shortNotes != null && Status.IsShortNoteVisible)
            {
                var drawNotes = shortNotes
                    .Where(x => x.Position.Tick >= Status.DrawTickFirst && x.Position.Tick <= Status.DrawTickLast);
                foreach(var note in drawNotes)
                {
                    note.Draw(g, drawLocation);
                }
            }
            // Air
            if (airNotes != null && Status.IsAirVisible)
            {
                var drawNotes = airNotes
                    .Where(x => x.Position.Tick >= Status.DrawTickFirst && x.Position.Tick <= Status.DrawTickLast);
                foreach(var note in drawNotes)
                {
                    note.Draw(g, drawLocation);
                }
            }
            // AirHold
            if (airHoldNotes != null && Status.IsAirHoldVisible)
            {
                var drawNotes = airHoldNotes.Where(x => x.IsDrawable());
                foreach(var note in drawNotes)
                {
                    note.Draw(g, drawLocation, laneBook);
                }
            }
        }
	}
}
