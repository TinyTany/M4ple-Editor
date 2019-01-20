using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;

namespace NE4S.Operation
{
    public class AddStepNoteOperation : Operation
    {
        public AddStepNoteOperation(Slide slide, SlideTap slideTap)
        {
            Invoke += () =>
            {
                slide.Add(slideTap);
            };
            Undo += () =>
            {
                slide.Remove(slideTap);
            };
        }

        public AddStepNoteOperation(Slide slide, SlideRelay slideRelay)
        {
            Invoke += () =>
            {
                slide.Add(slideRelay);
            };
            Undo += () =>
            {
                slide.Remove(slideRelay);
            };
        }

        public AddStepNoteOperation(Slide slide, SlideCurve slideCurve)
        {
            Invoke += () =>
            {
                slide.Add(slideCurve);
            };
            Undo += () =>
            {
                slide.Remove(slideCurve);
            };
        }

        public AddStepNoteOperation(AirHold airHold, AirAction airAction)
        {
            Invoke += () =>
            {
                airHold.Add(airAction);
            };
            Undo += () =>
            {
                airHold.Remove(airAction);
            };
        }
    }
}
