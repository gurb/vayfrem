using vayfrem.models.objects.@base;
using System.Collections.Generic;

namespace vayfrem.models.dtos
{
    public class UndoRedoDTO
    {
        public File? page { get; set; }
        public List<GObject>? objects { get; set; }
    }
}
