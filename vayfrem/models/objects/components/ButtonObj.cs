using Avalonia.Controls;
using vayfrem.models.dtos;
using vayfrem.models.objects.@base;

namespace vayfrem.models.objects.components
{
    public class ButtonObj : GObject
    {
        public bool IsEditMode { get; set; }

        public string? Text { get; set; } = "Button";

        public string? FontFamily { get; set; } = "Arial";
        public ColorDTO FontColor { get; set; } = new ColorDTO(255, 0, 0, 0);
        public int FontSize { get; set; } = 14;

        public enums.TextAlignment TextAlignment { get; set; } = enums.TextAlignment.MiddleCenter;

        public ButtonObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Button;

            SetStyle();
        }

        public override void SetStyle()
        {
            base.SetStyle();

            Width = 200;
            Height = 50;
            
            BorderColor = new dtos.ColorDTO(255, 0, 0, 0);
            BorderThickness = 3;
            BorderRadius = 5;
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
