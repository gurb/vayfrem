using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using vayfrem.models;
using vayfrem.models.commands;
using vayfrem.models.dtos;
using vayfrem.models.enums;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;
using vayfrem.services;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia;

namespace vayfrem.viewmodels
{
    public partial class DrawingViewModel : ObservableObject
    {
        private readonly ProjectManager projectManager;
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

        public delegate void SetDimensionDelegate();
        public SetDimensionDelegate? setDimensionDelegate;

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

            projectManager = App.GetService<ProjectManager>();
            SetCurrentFileIfExist();
        }

        private void SetCurrentFileIfExist()
        {
            foreach (var node in projectManager.CurrentProject.Nodes)
            {
                if (node.Type == models.enums.NodeType.File)
                {
                    CurrentFile = (File)node;
                    return;
                }
            }
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
            else if (passData.SelectedObjectType == ObjectType.QuadraticBC)
            {
                AddQuadraticBC(passData);
            }

            if(pageTreeViewModel.drawPageView != null)
            {
                pageTreeViewModel.Refresh(CurrentFile!);
            }
        }

        public void AddDirectObject(GObject? gobject, GObject? parentObj = null)
        {
            if(gobject != null)
            {
                if(parentObj != null)
                {
                    CanvasObj parentCanvas = (CanvasObj)parentObj;

                    gobject.Parent = parentCanvas;
                    gobject.ParentGuid = parentCanvas.Guid;
                    gobject.X = gobject.X - parentCanvas.WorldX;
                    gobject.Y = gobject.Y - parentCanvas.WorldY;

                    parentCanvas.Add(gobject);
                }
                else
                {
                    Objects.Add(gobject);
                }
                if (drawDelegate != null)
                {
                    drawDelegate.Invoke();
                }


                if (pageTreeViewModel.drawPageView != null)
                {
                    pageTreeViewModel.Refresh(CurrentFile!);
                }
            }
        }

        private void AddQuadraticBC(PassData passData)
        {
            QuadraticBCObj quadraticBCObj = new QuadraticBCObj();
            quadraticBCObj.Guid = Guid.NewGuid().ToString();

            quadraticBCObj.BorderThickness = toolOptionsViewModel.QuadraticBCToolDTO.BorderThickness;
            quadraticBCObj.BackgroundColor = toolOptionsViewModel.QuadraticBCToolDTO.BackgroundColorPicker.Color.toCopy();
            quadraticBCObj.BorderColor = toolOptionsViewModel.QuadraticBCToolDTO.BorderColorPicker.Color;
            quadraticBCObj.Opacity = toolOptionsViewModel.QuadraticBCToolDTO.Opacity;

            quadraticBCObj.StartPoint = passData.StartPoint!;
            quadraticBCObj.Point1 = passData.Point1!;
            quadraticBCObj.Point2 = passData.Point2!;


            if (SelectedObject != null)
            {
                if (SelectedObject.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj selectedCanvas = (CanvasObj)SelectedObject;
                    quadraticBCObj.Parent = selectedCanvas;
                    quadraticBCObj.ParentGuid = selectedCanvas.Guid;
                    //quadraticBCObj.X = textObj.X - selectedCanvas.WorldX;
                    //quadraticBCObj.Y = textObj.Y - selectedCanvas.WorldY;

                    selectedCanvas.Add(quadraticBCObj);
                    return;
                }
            }
            Objects.Add(quadraticBCObj);
        }


        private void AddCanvas(PassData passData)
        {
            CanvasObj canvasObj = new CanvasObj();
            canvasObj.Guid = Guid.NewGuid().ToString();

            canvasObj.BorderRadius = toolOptionsViewModel.RectToolDTO.BorderRadius;
            canvasObj.BorderThickness = toolOptionsViewModel.RectToolDTO.BorderThickness;
            canvasObj.BackgroundColor = toolOptionsViewModel.RectToolDTO.BackgroundColorPicker.Color.toCopy();
            canvasObj.BorderColor = toolOptionsViewModel.RectToolDTO.BorderColorPicker.Color;
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

            pageTreeViewModel.SetSelected(obj);
            if(pageTreeViewModel.drawPageView != null)
            {
                pageTreeViewModel.drawPageView.Invoke();
            }
            
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
                if(obj.ObjectType == ObjectType.QuadraticBC)
                {
                    QuadraticBCObj qbcObj = (QuadraticBCObj)obj;
                    if(!isCollide)
                        isCollide = IsPointOnEdgeWithThickness(mousePosition, qbcObj.StartPoint, qbcObj.Point1, qbcObj.Point2, qbcObj.BorderThickness);
                }
                else if (mousePosition.X >= obj.X &&
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

        public bool IsPointOnEdgeWithThickness(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2, double thickness)
        {
            // Üçgenin kenarlarının birim vektörlerini hesaplayın
            Vector2 edge0 = p1 - p0;
            Vector2 edge1 = p2 - p1;
            Vector2 edge2 = p0 - p2;

            // Kenar birim vektörlerinin normalize edilmiş versiyonlarını alın
            Vector2 edge0Normal = Vector2.Normalize(new Vector2(-edge0.Y, edge0.X));
            Vector2 edge1Normal = Vector2.Normalize(new Vector2(-edge1.Y, edge1.X));
            Vector2 edge2Normal = Vector2.Normalize(new Vector2(-edge2.Y, edge2.X));

            // Noktanın her bir kenara dik mesafesini hesaplayın
            double dist0 = Vector2.Dot(p - p0, edge0Normal);
            double dist1 = Vector2.Dot(p - p1, edge1Normal);
            double dist2 = Vector2.Dot(p - p2, edge2Normal);

            // Kenarların kalınlığına göre bir alan oluşturun
            double halfThickness = thickness / 2f;

            // Noktanın bu alanın içinde olup olmadığını kontrol edin
            if (Math.Abs(dist0) <= halfThickness || Math.Abs(dist1) <= halfThickness || Math.Abs(dist2) <= halfThickness)
                return true;

            return false;
        }



        // this method is used for dragging object the objects if interacted then add the drag object to collided object if there is.
        public GObject? CollisionPointWithObject(Vector2 mousePosition, CanvasObj? canvas = null)
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
                    isCollide = true;
                    if (obj.ObjectType == models.enums.ObjectType.Canvas)
                    {
                        CanvasObj canvasObj = (CanvasObj)obj;
                        if (canvasObj.Children.Count > 0)
                        {
                            var mouseOffset = new Vector2(mousePosition.X - canvasObj.X, mousePosition.Y - canvasObj.Y);

                            return CollisionPointWithObject(mouseOffset, canvasObj);
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    else
                    {
                        return canvas;
                    }
                }
            }

            if (canvas != null)
            {
                return canvas;
            }
            else
            {
                return null;
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

            if(setDimensionDelegate != null)
            {
                setDimensionDelegate.Invoke();
            }

            if(drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }


        public void Draw()
        {
            if (setDimensionDelegate != null)
            {
                setDimensionDelegate.Invoke();
            }

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
            if(CurrentFile != null)
            {
                propertyViewModel.RefreshPropertyView(CurrentFile.Selection.SelectedObject);
            }
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
