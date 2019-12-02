using NE4S.Notes.Abstract;
using NE4S.Notes.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
    public enum NoteType
    {
        Unknown,
        Tap,
        ExTap,
        AwExTap,
        ExTapDown,
        Flick,
        Damage,
        AirUpL,
        AirUpC,
        AirUpR,
        AirDownL,
        AirDownC,
        AirDownR,
        HoldBegin,
        HoldEnd,
        SlideBegin,
        SlideTap,
        SlideRelay,
        SlideCurve,
        SlideEnd,
        AirHoldBegin,
        AirAction,
        AirHoldEnd,
        Bpm,
        HighSpeed
    }

    public enum LongNoteType
    {
        Unknown,
        Hold,
        Slide,
        AirHold
    }
}

namespace NE4S.Notes.Interface
{
    public interface INote
    {
        NoteType NoteType { get; }
    }

    public interface ISizableNote : INote
    {
        int NoteSize { get; }
        Position Position { get; }
        bool ReSize(int size);
        bool Relocate(Position position);
    }

    public interface IAttributeNote : INote
    {
        double Value { get; }
        int Tick { get; }
        bool SetValue(double value);
        bool Relocate(int tick);
    }

    public interface IAirNote : ISizableNote { }

    public interface IAirableNote : ISizableNote
    {
        Air Air { get; }
        AirHold AirHold { get; }
        bool IsAirAttached { get; }
        bool IsAirHoldAttached { get; }
        bool AttachAir(Air air);
        bool DetachAir(out Air air);
        bool AttachAirHold(AirHold ah);
        bool DetachAirHold(out AirHold ah);
    }

    public interface IStepNote<T> : ISizableNote
        where T : IStepNote<T>
    {
        event Func<T, Position, bool> StepNotePositionChanging;
    }
}
