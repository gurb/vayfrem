using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models.objects.@base;
using draftio.models.structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace draftio.viewmodels
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