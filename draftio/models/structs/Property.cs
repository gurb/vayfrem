using draftio.views.sections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.structs
{
    public class Property
    {
        public ValueType ValueType { get; set; }
        public string? Name { get; set; }
        public object? Value { get; set; }

        public Property(ValueType type, object? value)
        {
            ValueType = type;
            Name = type.ToString();
            Value = value;
        }
    }
}
