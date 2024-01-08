using Avalonia.Controls;
using draftio.models.objects.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects
{
    public class TextObj: GObject
    {
        public bool IsEditMode { get; set; }

        public string? Text { get; set; }

        public string? FontFamily { get; set; } = "Arial";
        public Avalonia.Media.Color FontColor { get; set; } = new Avalonia.Media.Color(255, 0, 0, 0);
        public int FontSize { get; set; } = 14;


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
