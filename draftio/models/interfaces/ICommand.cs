using draftio.models.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.interfaces
{
    public interface ICommand
    {
        void Execute(UndoRedoDTO urdto);
    }
}
