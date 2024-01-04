using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models.dtos;
using draftio.models.enums;
using draftio.models.objects.@base;


namespace draftio.viewmodels
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

        public delegate void DrawToolOptionDelegate();
        public DrawToolOptionDelegate? drawToolOptionDelegate;

        public delegate void InitToolOptionDelegate();
        public InitToolOptionDelegate? initToolOptionDelegate;

        public ToolOptionsViewModel() 
        {
            rectToolDTO = new RectToolDTO();
        }

        public void SetToolOption(ToolOption toolOption)
        {
            ToolOption = toolOption;
            ToolName = toolOption.ToString();

           
            if(drawToolOptionDelegate != null)
            {
                drawToolOptionDelegate.Invoke();
            }
        }
    }
}
