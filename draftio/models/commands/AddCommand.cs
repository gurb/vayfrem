using draftio.models.interfaces;
using draftio.models.objects.@base;
using System;

namespace draftio.models.commands
{
    public class AddCommand : ICommand
    {
        public GObject? AddedObject { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
