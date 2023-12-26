using draftio.models.interfaces;
using draftio.models.structs;
using System;

namespace draftio.models.commands
{
    public class TranslateCommand : ICommand
    {
        public Vector2? OldPosition { get; set; }
        public Vector2? NewPosition { get; set; }
        public string? ObjectGuid { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
