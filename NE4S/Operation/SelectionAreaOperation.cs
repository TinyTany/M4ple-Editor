using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NE4S.Notes;
using NE4S.Component;
using System.Windows.Forms;

namespace NE4S.Operation
{
    /// <summary>
    /// 矩形選択範囲内のノーツをクリップボードにコピーします
    /// </summary>
    /// reviewed on 2019/08/06
    public class CopyNotesOperation : Operation
    {
        public CopyNotesOperation(SelectionArea selectionArea)
        {
            // NOTE: ClipBoardに「送る」っていうのは多分送るオブジェクトに参照をするだけであって、
            //       その場でシリアライズされたりはしない感じなはず（ディープコピーするのはそのため）
            Clipboard.SetDataObject(selectionArea.DeepCopy());
            Status.OnPasteChanged(true);
            Canceled = true;

            Invoke += () => Logger.Warn("何もしない");
            Undo += () => Logger.Warn("何もしない");
        }
    }

    /// <summary>
    /// 矩形選択範囲内のノーツを切り取り、クリップボードに送ります
    /// </summary>
    /// reviewed on 2019/08/19
    public class CutNotesOperation : Operation
    {
        public CutNotesOperation(Model model, SelectionArea selectionArea)
        {
            if (model == null || selectionArea == null)
            {
                Logger.Error("引数にnullのものが含まれるため、操作を行えません。", true);
                Canceled = true;
                return;
            }

            var tmpArea = new SelectionArea(selectionArea);
            var clearOp = new ClearAreaNotesOperation(model, selectionArea);

            Invoke += () =>
            {
                new CopyNotesOperation(tmpArea).Invoke();
                clearOp.Invoke();
            };
            Undo += () =>
            {
                clearOp.Undo();
            };
        }
    }

    /// <summary>
    /// クリップボードのノーツを貼り付けます
    /// </summary>
    /// reviewed on 2019/08/06
    public class PasteNotesOperation : Operation
    {
        public PasteNotesOperation(Model model, SelectionArea selectionArea, Position position)
        {
            if (model == null || selectionArea == null || position == null)
            {
                Logger.Error("引数にnullのものが存在するため、操作を行えません。", true);
                Canceled = true;
                return;
            }

            var book = model.NoteBook;
            SelectionArea tmpArea = null;

            if (Clipboard.GetDataObject().GetData(typeof(SelectionArea)) is SelectionArea data)
            {
                tmpArea = data;
            }
            else
            {
                Logger.Error("クリップボードにデータが無かったため操作を行えません。");
                Canceled = true;
                return;
            }

            tmpArea.Relocate(position, model.LaneBook);

            Invoke += () =>
            {
                selectionArea.Reset(tmpArea);
                book.PutRange(tmpArea.SelectedNoteList);
                book.PutRange(tmpArea.SelectedLongNoteList);
            };
            Undo += () =>
            {
                selectionArea.Reset();
                book.UnPutRange(tmpArea.SelectedNoteList);
                book.UnPutRange(tmpArea.SelectedLongNoteList);
            };
        }
    }

    /// <summary>
    /// クリップボードのノーツを左右反転して貼り付けます
    /// </summary>
    /// reviewed on 2019/08/03
    public class PasteAndReverseNotesOperation : Operation
    {
        public PasteAndReverseNotesOperation(Model model, SelectionArea selectionArea, Position position)
        {
            if (model == null || selectionArea == null || position == null)
            {
                Logger.Error("引数にnullのものが含まれるため、操作を行えません。", true);
                Canceled = true;
                return;
            }

            var paste = new PasteNotesOperation(model, selectionArea, position);
            var reverse = new ReverseNotesOperation(model, selectionArea);

            Canceled = paste.Canceled && reverse.Canceled;

            Invoke += () =>
            {
                paste.Invoke();
                reverse.Invoke();
            };
            Undo += () =>
            {
                reverse.Undo();
                paste.Undo();
            };
        }
    }

    /// <summary>
    /// 選択矩形内に含まれるノーツを全削除します
    /// </summary>
    // reviewed on 2019/08/06
    public class ClearAreaNotesOperation : Operation
    {
        public ClearAreaNotesOperation(Model model, SelectionArea selectionArea)
        {
            if (model == null || selectionArea == null)
            {
                Logger.Error("引数にnullのものが含まれるため、操作を行えません。", true);
                Canceled = true;
                return;
            }

            var tmpArea = new SelectionArea(selectionArea);
            var book = model.NoteBook;
            selectionArea.Reset();

            Invoke += () =>
            {
                book.UnPutRange(tmpArea.SelectedNoteList);
                book.UnPutRange(tmpArea.SelectedLongNoteList);
            };
            Undo += () =>
            {
                book.PutRange(tmpArea.SelectedNoteList);
                book.PutRange(tmpArea.SelectedLongNoteList);
            };
        }
    }

    /// <summary>
    /// 選択矩形内のノーツを左右反転します
    /// </summary>
    /// reviewed on 2019/07/29
    public class ReverseNotesOperation : Operation
    {
        public ReverseNotesOperation(Model model, SelectionArea selectionArea)
        {
            Invoke += () =>
            {
                selectionArea.ReverseNotes(model.NoteBook, model.LaneBook);
            };
            Undo += () =>
            {
                selectionArea.ReverseNotes(model.NoteBook, model.LaneBook);
            };
        }
    }
}
