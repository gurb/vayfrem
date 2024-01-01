using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models.structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace draftio.viewmodels
{
    public partial class PropertyViewModel: ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Property> properties = new();


        public PropertyViewModel()
        {
            properties = new ObservableCollection<Property>()
            {
                new Property("Width", "100px"),
                new Property("Height", "50px"),
                new Property("Background", "60px")
            };

        }
    }
}
