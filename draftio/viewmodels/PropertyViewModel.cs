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

        GObject? activeObj; 

        public PropertyViewModel()
        {
            properties = new ObservableCollection<Property>()
            {
                
            };
        }

        public void SetActiveObject (GObject? obj)
        {
            activeObj = obj;
            if (obj != null)
            {
                SetProperties();
            }
        }

        public void RefreshPropertyView(GObject? obj)
        {
            if (activeObj == obj)
            {
                SetProperties();
            }
        }

        private void SetProperties()
        {
            if(activeObj!.ObjectType == models.enums.ObjectType.Canvas)
            {
                Properties = new ObservableCollection<Property>()
                {
                    new Property("Name", activeObj.ObjectType.ToString()),
                    new Property("X", activeObj.X),
                    new Property("Y", activeObj.Y),
                    new Property("Width", activeObj.Width),
                    new Property("Height", activeObj.Height),
                    new Property("Background", activeObj.BackgroundColor),
                    new Property("Opacity", activeObj.Opacity),
                    new Property("Border Color", activeObj.BorderColor),
                    new Property("Border Radius", activeObj.BorderRadius),
                    new Property("Border Thickness", activeObj.BorderThickness),
                };
            }

            if (activeObj!.ObjectType == models.enums.ObjectType.Text)
            {
                Properties = new ObservableCollection<Property>()
                {
                    new Property("Name", activeObj.ObjectType.ToString()),
                    new Property("X", activeObj.X),
                    new Property("Y", activeObj.Y),
                    new Property("Width", activeObj.Width),
                    new Property("Height", activeObj.Height),
                    new Property("Background", activeObj.BackgroundColor)
                };
            }

        }
    }
}
