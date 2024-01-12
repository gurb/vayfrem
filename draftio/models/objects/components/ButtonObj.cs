using Avalonia.Controls;
using draftio.models.objects.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects.components
{
    public class ButtonObj : GObject
    {
        public bool IsEditMode { get; set; }

        public string? Text { get; set; } = "Button";

        public string? FontFamily { get; set; } = "Arial";
        public Avalonia.Media.Color FontColor { get; set; } = new Avalonia.Media.Color(255, 0, 0, 0);
        public int FontSize { get; set; } = 14;


        public ButtonObj()
        {
            InitializeObject();

        }

        public override void InitializeObject()
        {
            Width = 200;
            Height = 50;
            this.ObjectType = enums.ObjectType.Button;
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
