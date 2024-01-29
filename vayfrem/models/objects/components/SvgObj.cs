using vayfrem.models.objects.@base;

namespace vayfrem.models.objects.components
{
    public class SvgObj: GObject
    {
        public string? Path { get; set; }

        public SvgObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Svg;

            SetStyle();
        }

        public override void SetStyle()
        {
            base.SetStyle();

            Width = 64;
            Height = 64;
        }
    }
}
