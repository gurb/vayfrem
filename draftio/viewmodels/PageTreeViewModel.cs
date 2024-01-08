﻿using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models;
using draftio.models.objects.@base;
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

        public void SetSelected(GObject? obj)
        {
            SelectedObject = obj;
            if(drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }

        public void Refresh(File? file)
        {
            if (file != null)
            {
                nodes = new ObservableCollection<GObject>(file.Objects);
                SelectedObject = file.Selection!.SelectedObject;
            }
            else
            {
                nodes = new ObservableCollection<GObject>();
            }

            if (drawPageView != null)
            {
                drawPageView.Invoke();
            }
        }

    }
}
