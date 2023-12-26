using draftio.models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.services
{
    public class UndoRedoManager
    {
        public Dictionary<string, Stack<ICommand>> UndoLogs { get; set; }
        public Dictionary<string, Stack<ICommand>> RedoLogs { get; set; }


        public UndoRedoManager() 
        {
            UndoLogs = new Dictionary<string, Stack<ICommand>>();
            RedoLogs = new Dictionary<string, Stack<ICommand>>();
        }


        public void AddCommand(string guid, ICommand command)
        {
            if(UndoLogs[guid] == null)
            {
                UndoLogs[guid] = new Stack<ICommand>();
            }

            UndoLogs[guid].Push(command);

            if (RedoLogs[guid] != null)
            {
                RedoLogs[guid].Clear();
            }
            else
            {
                RedoLogs[guid] = new Stack<ICommand>();
            }
        }

        public void Undo(string guid)
        {
            if (UndoLogs.ContainsKey(guid) && UndoLogs[guid].Count > 0)
            {
                ICommand command = UndoLogs[guid].Pop();

                RedoLogs[guid].Push(command);
            }
        }

        public void Redo(string guid)
        {
            if (RedoLogs.ContainsKey(guid) && RedoLogs[guid].Count > 0)
            {
                ICommand command = UndoLogs[guid].Pop();

                UndoLogs[guid].Push(command);
            }
        }

        public void Execute(string guid)
        {
            // we can reach the page node via guid parameter, and we can handle changes of object according to the command logs.

            if (UndoLogs.ContainsKey(guid) && UndoLogs[guid].Count > 0)
            {
                foreach (var command in UndoLogs[guid])
                {
                    command.Execute();
                }
            }
        }
    }
}
