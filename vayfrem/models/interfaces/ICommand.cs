using vayfrem.models.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.interfaces
{
    public interface ICommand
    {
        void Execute(UndoRedoDTO urdto);
    }
}
