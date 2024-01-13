using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.dtos
{
    public class NewProjectResponse
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Name { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
