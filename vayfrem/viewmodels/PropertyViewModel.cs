using CommunityToolkit.Mvvm.ComponentModel;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace vayfrem.viewmodels
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
