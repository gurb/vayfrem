using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.dtos
{
    public class VMResponse
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public object? Result { get; set; }
    }
}
