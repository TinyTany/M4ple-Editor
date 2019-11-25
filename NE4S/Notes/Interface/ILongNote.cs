using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes.Interface
{
    public interface ILongNote<T>
        where T : IStepNote<T>
    {
        bool Put(T step);
        bool UnPut(T step);
    }
}
