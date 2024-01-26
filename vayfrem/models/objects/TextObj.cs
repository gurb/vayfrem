using Avalonia.Controls;
using vayfrem.models.dtos;
using vayfrem.models.objects.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.objects
{
    public class TextObj: GObject
    {
        public bool IsEditMode { get; set; }

        public string? Text { get; set; }

        public string? FontFamily { get; set; } = "Arial";
        public ColorDTO FontColor { get; set; } = new ColorDTO(255, 0, 0, 0);
        public int FontSize { get; set; } = 14;

        public enums.ContentAlignment ContentAlignment { get; set; } = enums.ContentAlignment.Left;

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
