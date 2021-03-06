﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using System.Diagnostics;

namespace NE4S.Operation
{
    /// <summary>
    /// Hold, Slideを取り除く操作を表します。
    /// </summary>
    /// reviewed on 2019/07/29
    public class DeleteLongNoteOperation : Operation
    {
        public DeleteLongNoteOperation(Model model, LongNote longNote)
        {
            if (model == null || longNote == null)
            {
                Logger.Error("引数にnullのものが存在するため削除操作を行えません。", true);
                Canceled = true;
                return;
            }

            var book = model.NoteBook;

            Invoke += () =>
            {
                book.UnPut(longNote);
            };

            Undo += () =>
            {
                book.Put(longNote);
            };
        }
    }
}
