using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
	public class LongNoteMaterial
	{
		private List<NoteMaterial> noteList;
		private Range longNoteRange;

		public LongNoteMaterial(LongNote longNote, Range longNoteRange)
		{
			foreach(Note note in longNote)
			{
				//noteList.Add(new NoteMaterial(note, ));
			}
			this.longNoteRange = longNoteRange;
		}
	}
}
