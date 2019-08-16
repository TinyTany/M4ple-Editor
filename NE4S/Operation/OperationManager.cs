using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Operation
{
    public class OperationManager
    {
        private readonly Stack<Operation> stackUndo;
        private readonly Stack<Operation> stackRedo;

        public event Action<bool, bool> StatusChanged;
        public event Action Edited;

        public OperationManager()
        {
            stackUndo = new Stack<Operation>();
            stackRedo = new Stack<Operation>();
        }

        public void AddOperationAndInvoke(Operation operation)
        {
            if (!IsOperationValid(operation)) { return; }
            AddOperation(operation);
            operation.Invoke();
        }

        public void AddOperation(Operation operation)
        {
            if (!IsOperationValid(operation)) { return; }
            stackUndo.Push(operation);
            stackRedo.Clear();
            StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
            Edited.Invoke();
        }

        private bool IsOperationValid(Operation operation)
        {
            if (operation == null)
            {
                Logger.Error("操作を追加できません。引数operationがnullです。");
                return false;
            }
            if (operation.Canceled)
            {
                Logger.Warn("操作はキャンセルされたため破棄されます。");
                return false;
            }
            return true;
        }

        public void Undo()
        {
            if (stackUndo.Any())
            {
                var op = stackUndo.Pop();
                op.Undo();
                stackRedo.Push(op);
            }
            StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
            Edited.Invoke();
        }

        public void Redo()
        {
            if (stackRedo.Any())
            {
                var op = stackRedo.Pop();
                op.Invoke();
                stackUndo.Push(op);
            }
            StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
            Edited.Invoke();
        }

        public void OnStatusChanged() => StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
    }
}
