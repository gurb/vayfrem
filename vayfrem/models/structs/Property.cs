using Avalonia.Controls;
using vayfrem.views.sections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.structs
{
    public class Property
    {
        public ValueType ValueType { get; set; }
        public string? Name { get; set; }
        public object? Value { get; set; }

        public Control? Control { get; set; }

        public Property(ValueType type, Control control)
        {
            ValueType = type;
            Name = type.ToString();
            //Value = value;
            Control = control;
        }
    }
}
