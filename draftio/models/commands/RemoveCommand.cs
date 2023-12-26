using draftio.models.interfaces;
using draftio.models.objects.@base;
using System;

namespace draftio.models.commands
{
    public class RemoveCommand : ICommand
    {
        public GObject? RemovedObject { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
