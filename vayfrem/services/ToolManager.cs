using vayfrem.models.enums;
using vayfrem.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.services
{
    public class ToolManager
    {
        private readonly ToolOptionsViewModel toolOptionsViewModel;

        public ToolOption SelectedToolOption { get; set; }
        public ObjectType SelectedObjectType { get; set; }

        public ToolManager() 
        {
            toolOptionsViewModel = App.GetService<ToolOptionsViewModel>();

            SelectedToolOption = ToolOption.Rect;
            SelectedObjectType = ObjectType.Canvas;
        }

        public void SetToolOption(ToolOption option)
        {
            SelectedToolOption = option;

            toolOptionsViewModel.SetToolOption(option);

            switch (option)
            {
                case ToolOption.Rect:
                    SelectedObjectType = ObjectType.Canvas;
                    break;
                case ToolOption.Text:
                    SelectedObjectType = ObjectType.Text;
                    break;
                case ToolOption.QBC:
                    SelectedObjectType = ObjectType.QuadraticBC;
                    break;
                default:
                    SelectedObjectType = ObjectType.Canvas;
                    break;
            }
        }
    }
}
