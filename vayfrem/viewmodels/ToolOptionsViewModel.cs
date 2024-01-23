using CommunityToolkit.Mvvm.ComponentModel;
using vayfrem.models.dtos;
using vayfrem.models.enums;
using vayfrem.models.objects.@base;


namespace vayfrem.viewmodels
{
    public partial class ToolOptionsViewModel : ObservableObject
    {
        [ObservableProperty]
        ToolOption toolOption;

        [ObservableProperty]
        string? toolName;

        [ObservableProperty]
        double width;

        [ObservableProperty]
        RectToolDTO rectToolDTO;

        [ObservableProperty]
        TextToolDTO textToolDTO;

        [ObservableProperty]
        string? toolNameTitle;

        public delegate void DrawToolOptionDelegate();
        public DrawToolOptionDelegate? drawToolOptionDelegate;

        public delegate void InitToolOptionDelegate();
        public InitToolOptionDelegate? initToolOptionDelegate;

        public ToolOptionsViewModel() 
        {
            rectToolDTO = new RectToolDTO();
            textToolDTO = new TextToolDTO();
        }

        public void SetToolOption(ToolOption toolOption)
        {
            ToolOption = toolOption;
            ToolName = toolOption.ToString();
            ToolNameTitle = "Selected Tool: " + ToolName; 
           
            if(drawToolOptionDelegate != null)
            {
                drawToolOptionDelegate.Invoke();
            }
        }
    }
}
