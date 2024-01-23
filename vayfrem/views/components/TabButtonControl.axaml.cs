using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace vayfrem.views.components
{
    public class TabButtonControl : TemplatedControl
    {
        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<TabButtonControl, string>(nameof(Title), defaultValue:"Tab");

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
