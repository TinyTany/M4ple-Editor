using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Operation
{
    public class OperationManager
    {
        private Stack<Operation> stackUndo, stackRedo;
        public event Action<bool, bool> StatusChanged;
        public event Action Edited;

        public OperationManager()
        {
            stackUndo = new Stack<Operation>();
            stackRedo = new Stack<Operation>();
        }

        public void AddOperationAndInvoke(Operation operation)
        {
            AddOperation(operation);
            operation.Invoke();
        }

        public void AddOperation(Operation operation)
        {
            stackUndo.Push(operation);
            stackRedo.Clear();
            StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
            Edited.Invoke();
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
