using vayfrem.models.dtos;
using vayfrem.models.objects.@base;
using vayfrem.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.services
{
    public class ObjectMenuManager
    {
        private readonly DrawingViewModel drawingViewModel;

        public GObject? CurrentObject { get; set; }
        public GObject? CopyObject { get; set; }

        public ObjectMenuManager()
        {
            drawingViewModel = App.GetService<DrawingViewModel>();
        }

        public void SetObject(GObject? obj)
        {
            CurrentObject = obj;
        }

        public void Copy()
        {
            if(CurrentObject != null)
            {
                CopyObject = CurrentObject.Copy();
            }

            CurrentObject = null;
        }

        public void Paste()
        {
            if(CopyObject == null)
            {
                return;
            }

            drawingViewModel.AddDirectObject(CopyObject);

            CopyObject = null;
        }
    }
}
