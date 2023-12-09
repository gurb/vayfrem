using Avalonia.Controls;
using draftio.models.objects.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects
{
    public class TextObj: Object
    {
        public bool IsEditMode { get; set; }

        public string? Text { get; set; }

        public TextObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Text;
        }

        public void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            if (textBox != null)
            {
                Text = textBox.Text;
            }
        }

    }
}
