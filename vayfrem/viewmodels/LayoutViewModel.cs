using CommunityToolkit.Mvvm.ComponentModel;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace vayfrem.viewmodels
{
    public partial class LayoutViewModel : ObservableObject
    {

        [ObservableProperty]
        bool isDrag;

        [ObservableProperty]
        bool isDragCompleted;

        [ObservableProperty]
        GObject? dragObject;

        [ObservableProperty]
        int counter;

        [ObservableProperty]
        bool isOpenMenu;

        public LayoutViewModel()
        {
            Counter = 0;
            IsDragCompleted = true;
        }


        public void OpenMenu()
        {
            IsOpenMenu = true;
        }
    }
}