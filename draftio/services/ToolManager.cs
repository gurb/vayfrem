using draftio.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.services
{
    public class ToolManager
    {
        public ToolOption SelectedToolOption { get; set; }

        public ToolManager() 
        {
            SelectedToolOption = ToolOption.Rect;
        }

        public void SetToolOption(ToolOption option)
        {
            SelectedToolOption = option;
        }
    }
}
