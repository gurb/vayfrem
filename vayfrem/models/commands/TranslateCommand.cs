using vayfrem.models.dtos;
using vayfrem.models.interfaces;
using vayfrem.models.structs;
using System;

namespace vayfrem.models.commands
{
    public class TranslateCommand : ICommand
    {
        public Vector2? OldPosition { get; set; }
        public Vector2? NewPosition { get; set; }
        public string? ObjectGuid { get; set; }

        public void Execute(UndoRedoDTO urdto)
        {
            throw new NotImplementedException();
        }
    }
}
