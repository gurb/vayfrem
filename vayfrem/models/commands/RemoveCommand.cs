using vayfrem.models.dtos;
using vayfrem.models.interfaces;
using vayfrem.models.objects.@base;
using System;

namespace vayfrem.models.commands
{
    public class RemoveCommand : ICommand
    {
        public GObject? RemovedObject { get; set; }

        public void Execute(UndoRedoDTO urdto)
        {
            throw new NotImplementedException();
        }
    }
}
