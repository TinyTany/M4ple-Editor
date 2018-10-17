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

		public LongNoteMaterial(LongNote longNote)
		{
			noteList = new List<NoteMaterial>();
			foreach(Note note in longNote)
			{
				//noteList.Add(new NoteMaterial(note, ));
			}
			this.longNote = longNote;
		}
	}
}
