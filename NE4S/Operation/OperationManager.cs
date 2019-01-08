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

        public OperationManager()
        {
            stackUndo = new Stack<Operation>();
            stackRedo = new Stack<Operation>();
        }

        public void AddOperationAndInvoke(Operation operation)
        {
            stackUndo.Push(operation);
            operation.Invoke();
            stackRedo.Clear();
            StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
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
        }

        public void OnStatusChanged() => StatusChanged?.Invoke(stackUndo.Any(), stackRedo.Any());
    }
}
