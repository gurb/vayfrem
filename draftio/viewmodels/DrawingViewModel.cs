using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.commands;
using draftio.models.dtos;
using draftio.models.enums;
using draftio.models.objects;
using draftio.models.objects.@base;
using draftio.models.structs;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace draftio.viewmodels
{
    public partial class DrawingViewModel : ObservableObject
    {

        private readonly ShortsViewModel shortsViewModel;
        private readonly UndoRedoManager undoRedoManager;
        private readonly PageTreeViewModel pageTreeViewModel;
        private readonly ToolOptionsViewModel toolOptionsViewModel;
        private readonly PropertyViewModel propertyViewModel;

        [ObservableProperty]
        List<GObject> objects = new();

        [ObservableProperty]
        GObject? selectedObject;

        [ObservableProperty]
        GObject? activeTextObject;

        [ObservableProperty]
        bool isEmpty = true;

        [ObservableProperty]
        bool isSelect = false;

        [ObservableProperty]
        bool isScale = false;

        [ObservableProperty]
        bool isOverScalePoint;

        [ObservableProperty]
        string? getOverScalePoint;

        [ObservableProperty]
        File? currentFile;


        public delegate void DrawDelegate();
        public DrawDelegate? drawDelegate;

        public delegate void DrawOverlayDelegate();

        public DrawingViewModel() 
        {
            shortsViewModel = App.GetService<ShortsViewModel>();
            shortsViewModel.drawDelegate += ChangeFile;
            undoRedoManager = App.GetService<UndoRedoManager>();
            pageTreeViewModel = App.GetService<PageTreeViewModel>();
            pageTreeViewModel.drawDelegate += Draw;
            toolOptionsViewModel = App.GetService<ToolOptionsViewModel>();
            propertyViewModel = App.GetService<PropertyViewModel>();
            propertyViewModel.drawDelegate += Draw;
        }

        [RelayCommand]
        public void AddObject(PassData passData)
        {
            SetSaveStateCurrentFile();
            SetSelectedObject(null);

            if (passData.Width < 10 && passData.Height < 10) return;

            if (passData.SelectedObjectType == ObjectType.Canvas)
            {
                AddCanvas(passData);
            }
            else if(passData.SelectedObjectType == ObjectType.Text)
            {
                AddText(passData);
            }

            if(pageTreeViewModel.drawPageView != null)
            {
                pageTreeViewModel.Refresh(CurrentFile!);
            }
        }

        private void AddCanvas(PassData passData)
        {
            CanvasObj canvasObj = new CanvasObj();
            canvasObj.Guid = Guid.NewGuid().ToString();

            canvasObj.BorderRadius = toolOptionsViewModel.RectToolDTO.BorderRadius;
            canvasObj.BorderThickness = toolOptionsViewModel.RectToolDTO.BorderThickness;
            canvasObj.BackgroundColor = toolOptionsViewModel.RectToolDTO.Background;
            canvasObj.BorderColor = toolOptionsViewModel.RectToolDTO.BorderColor;
            canvasObj.Opacity = toolOptionsViewModel.RectToolDTO.Opacity;

            canvasObj.X = (int)passData.X;
            canvasObj.Y = (int)passData.Y;
            canvasObj.Width = (int)passData.Width;
            canvasObj.Height = (int)passData.Height;

            if (SelectedObject != null)
            {
                if(SelectedObject.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj selectedCanvas = (CanvasObj)SelectedObject;
                    canvasObj.Parent = selectedCanvas;
                    canvasObj.ParentGuid = selectedCanvas.Guid;
                    canvasObj.X = canvasObj.X - selectedCanvas.WorldX;
                    canvasObj.Y = canvasObj.Y - selectedCanvas.WorldY;

                    selectedCanvas.Add(canvasObj);
                    undoRedoManager.AddCommand(CurrentFile!.Guid!, new AddCommand(canvasObj));
                    shortsViewModel.ChangeUndoState(true);
                    shortsViewModel.ChangeRedoState(false);
                    return;
                } 
            } 

            Objects.Add(canvasObj);
            undoRedoManager.AddCommand(CurrentFile!.Guid!, new AddCommand(canvasObj));
            shortsViewModel.ChangeUndoState(true);
            shortsViewModel.ChangeRedoState(false);
        }

        public void SetSelectedObject(GObject? obj)
        {
            if(CurrentFile == null)
            {
                return;
            }

            if(CurrentFile.Selection!.SelectedObject != null)
            {
                CurrentFile.Selection!.SelectedObject.ZIndex = 1;
            }

            CurrentFile.Selection!.SelectedObject = obj;
            propertyViewModel.SetActiveObject(obj);
            // if this function called from pageview menu
            IsSelect = true;

            
            if(obj != null)
            {
                CurrentFile.Selection!.SelectedObject!.ZIndex = CurrentFile.Selection!.ZIndex;
            }
        }

        public GObject? GetSelectionObject()
        {
            if (CurrentFile == null)
                return null;
            return CurrentFile.Selection!.SelectedObject;
        }


        private void SetSaveStateCurrentFile ()
        {
            if(CurrentFile != null)
            {
                CurrentFile.IsSaved = false;
                shortsViewModel.ChangeSaveState(false);
            }
        }


        public void ActiveEditText(GObject? obj)
        {
            SetSelectedObject(obj);

            if(obj.ObjectType == ObjectType.Text)
            {
                TextObj textObj = (TextObj)obj;
                if(textObj.IsEditMode)
                {
                    return;
                }

                textObj.IsEditMode = true;
                ActiveTextObject = textObj;
            }

        }


        private void AddText(PassData passData)
        {
            CloseEditMode();
            TextObj textObj = new TextObj();

            textObj.FontColor = toolOptionsViewModel.TextToolDTO.FontColor;
            textObj.FontSize = toolOptionsViewModel.TextToolDTO.FontSize;
            textObj.FontFamily = toolOptionsViewModel.TextToolDTO.FontFamily;

            textObj.Guid = Guid.NewGuid().ToString();
            textObj.X = passData.X;
            textObj.Y = passData.Y;
            textObj.Width = passData.Width;
            textObj.Height = passData.Height;
            textObj.IsEditMode = true;
            ActiveTextObject = textObj;

            if (SelectedObject != null)
            {
                if(SelectedObject.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj selectedCanvas = (CanvasObj)SelectedObject;
                    textObj.Parent = selectedCanvas;
                    textObj.ParentGuid = selectedCanvas.Guid;
                    textObj.X = textObj.X - selectedCanvas.WorldX;
                    textObj.Y = textObj.Y - selectedCanvas.WorldY;

                    selectedCanvas.Add(textObj);
                    return;
                }
            }
            Objects.Add(textObj);
        }


        public void CollisionDetectPoint(Vector2 mousePosition, CanvasObj? canvas = null)
        {
            bool isCollide = false;

            var tempObjects = canvas != null ? canvas.Children : Objects;

            tempObjects = tempObjects.OrderBy(x => x.ZIndex).ToList();

            foreach (var obj in tempObjects)
            {
                if (mousePosition.X >= obj.X &&
                    mousePosition.X <= obj.X + obj.Width &&
                    mousePosition.Y >= obj.Y &&
                    mousePosition.Y <= obj.Y + obj.Height)
                {
                    if(obj.ObjectType == models.enums.ObjectType.Canvas)
                    {
                        CanvasObj canvasObj = (CanvasObj)obj; 
                        if(canvasObj.Children.Count > 0)
                        {
                            var mouseOffset = new Vector2(mousePosition.X - canvasObj.X, mousePosition.Y - canvasObj.Y);

                            CollisionDetectPoint(mouseOffset, canvasObj);
                        } 
                        else
                        {
                            SelectedObject = obj;
                        }
                    } 
                    else
                    {
                        SelectedObject = obj;
                    }
                    isCollide = true;
                }
            }

            if(!isCollide)
            {
                if(canvas != null)
                {
                    SelectedObject = canvas;
                }
                else
                {
                    SelectedObject = null;
                }
                CloseEditMode();
            }
        }


        [RelayCommand]
        public void ChangeFile(File file)
        {
            CurrentFile = file;
            Objects = file.Objects;

            if(file.Selection != null)
            {
                SetSelectedObject(file.Selection.SelectedObject);
            }

            if(drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }


        public void Draw()
        {
            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }

        private void CloseEditMode()
        {
            if(ActiveTextObject != null)
            {
                TextObj obj = (TextObj)ActiveTextObject;
                obj.IsEditMode = false;
                ActiveTextObject = null;
            }
        }

        public void RefreshState()
        {
            propertyViewModel.RefreshPropertyView(SelectedObject);
        }

        [RelayCommand]
        public void SetEmptyState(bool isEmpty)
        {
            IsEmpty = isEmpty;

            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }
    }
}
