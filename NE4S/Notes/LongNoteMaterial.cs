using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Notes
{
	public class LongNoteMaterial
	{
		//このオブジェクトを持っているScoreLane内に含まれるNoteMaterialを格納
		private List<NoteMaterial> noteList;
		//このオブジェクトが操作するLongNote
		private LongNote longNote;
		//このオブジェクトを持っているScoreLaneがロングノーツ全体のどの範囲を持っているか
		private Range longNoteRange;

		public LongNoteMaterial(LongNote longNote, Range longNoteRange)
		{
			foreach(Note note in longNote)
			{
				//noteList.Add(new NoteMaterial(note, ));
			}
			this.longNote = longNote;
			this.longNoteRange = longNoteRange;
		}
	}
}
