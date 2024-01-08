using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models.objects.@base;
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
        [ObservableProperty]
        GObject? activeObj;

        public delegate void SetPropertyDelegate();
        public SetPropertyDelegate? setProperty;

        public delegate void DrawDelegate();
        public DrawDelegate? drawDelegate;

        public PropertyViewModel()
        {
            properties = new ObservableCollection<Property>()
            {
                
            };
        }

        public void SetActiveObject (GObject? obj)
        {
            activeObj = obj;
            
            if (setProperty != null)
            {
                setProperty.Invoke();
            }
        }

        public void RefreshPropertyView(GObject? obj)
        {
            if (activeObj == obj)
            {
                if(setProperty != null)
                {
                    setProperty.Invoke();
                }
            }
        }


        public void RefreshDraw()
        {
            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }

    }
}
