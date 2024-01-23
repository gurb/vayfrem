using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models
{
    public class Tab
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public File? File { get; set; }
        public string? GetFileName 
        { 
            get
            {
                return File != null ? File.Name : null;
            } 
        }
    }
}
