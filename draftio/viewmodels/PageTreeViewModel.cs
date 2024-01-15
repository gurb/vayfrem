using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models;
using draftio.models.objects;
using draftio.models.objects.@base;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.viewmodels
{
    public partial class PageTreeViewModel: ObservableObject
    {
        private readonly PropertyViewModel propertyViewModel;

        [ObservableProperty]
        ObservableCollection<GObject> nodes;

        [ObservableProperty]
        File? currentFile;

        [ObservableProperty]
        GObject? selectedObject;

        public delegate void DrawPageViewDelegate();
        public DrawPageViewDelegate? drawPageView;

        // drawviewmodel
        public delegate void DrawDelegate();
        public DrawDelegate? drawDelegate;



        public delegate void SetSelectToolDelegate();
        public SetSelectToolDelegate? setSelect;

        public PageTreeViewModel()
        {
            propertyViewModel = App.GetService<PropertyViewModel>();
            nodes = new ObservableCollection<GObject>();
        }

        public void SetCurrentFile(File? file)
        {
            CurrentFile = file;
            Refresh(file);
        }

        public void SetSelectedObject(GObject? obj)
        {
            if (CurrentFile == null)
            {
                return;
            }

            if (CurrentFile.Selection!.SelectedObject != null)
            {
                CurrentFile.Selection!.SelectedObject.ZIndex = 1;
            }

            CurrentFile.Selection!.SelectedObject = obj;
            propertyViewModel.SetActiveObject(obj);

            if (setSelect != null)
            {
                setSelect.Invoke();
            }

            if (obj != null)
            {
                CurrentFile.Selection!.SelectedObject!.ZIndex = CurrentFile.Selection!.ZIndex;
            }
        }


        public void DeleteNode()
        {
            if (CurrentFile == null)
                return;

            if(SelectedObject == null)
            {
                return;
            }

            if(SelectedObject.ObjectType == models.enums.ObjectType.Canvas)
            {
                if (SelectedObject.ParentGuid != null)
                {
                    CanvasObj parentObj = (CanvasObj)SelectedObject.Parent!;
                    
                    List<GObject> DeletedObjects = GetRelatedObjects((CanvasObj)SelectedObject);

                    foreach (var item in DeletedObjects)
                    {
                        parentObj.Children.Remove(item);
                    }
                }
                else
                {
                    List<GObject> DeletedObjects = GetRelatedObjects((CanvasObj)SelectedObject);

                    foreach (var item in DeletedObjects)
                    {
                        CurrentFile.Objects.Remove(item);
                    }
                }
            }
            else
            {
                if(SelectedObject.ParentGuid != null)
                {
                    CanvasObj parentObj = (CanvasObj)SelectedObject.Parent!;
                    parentObj.Children.Remove(SelectedObject);
                }
                else
                {
                    CurrentFile.Objects.Remove(SelectedObject);
                }
            }

            Nodes = new ObservableCollection<GObject>(CurrentFile.Objects);

            Refresh(CurrentFile);
            SetSelected(null);
            propertyViewModel.SetActiveObject(null);
        }

        private List<GObject> GetRelatedObjects(CanvasObj obj)
        {
            List<GObject> objects = new List<GObject>();
            objects.Add(obj);

            foreach (var item in obj.Children)
            {
                if (item.ObjectType == models.enums.ObjectType.Canvas)
                    objects.AddRange(GetRelatedObjects((CanvasObj)item));
                else
                    objects.Add(item);
            }

            return objects;
        }

        public void SetSelected(GObject? obj)
        {
            SelectedObject = obj;
            if(CurrentFile != null)
            {
                CurrentFile.Selection!.SelectedObject = obj;
            }
            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }

        public void Refresh(File? file)
        {
            if (file != null)
            {
                Nodes = new ObservableCollection<GObject>(file.Objects);
                SelectedObject = file.Selection!.SelectedObject;
            }
            else
            {
                Nodes = new ObservableCollection<GObject>();
            }

            if (drawPageView != null)
            {
                drawPageView.Invoke();
            }
        }

    }
}
