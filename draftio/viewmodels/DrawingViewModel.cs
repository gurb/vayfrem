using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models.dtos;
using draftio.models.objects;
using draftio.models.structs;
using System.Collections.Generic;

namespace draftio.viewmodels
{
    public partial class DrawingViewModel : ObservableObject
    {
        [ObservableProperty]
        List<IObject> objects = new();

        [ObservableProperty]
        IObject? selectedObject;

        public DrawingViewModel() { }

        [RelayCommand]
        public void AddObject(PassData passData)
        {
            AddCanvas(passData);
        }

        private void AddCanvas(PassData passData)
        {
            CanvasObj canvasObj = new CanvasObj();
            canvasObj.X = passData.X;
            canvasObj.Y = passData.Y;
            canvasObj.Width = passData.Width;
            canvasObj.Height = passData.Height;

            if(SelectedObject != null)
            {
                if(SelectedObject.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj selectedCanvas = (CanvasObj)SelectedObject;
                    canvasObj.Parent = selectedCanvas;
                    canvasObj.X = canvasObj.X - selectedCanvas.X;
                    canvasObj.Y = canvasObj.Y - selectedCanvas.Y;

                    selectedCanvas.Add(canvasObj);
                }
            } else
            {
                Objects.Add(canvasObj);
            }
        }

        public void CollisionDetectPoint(Vector2 mousePosition, CanvasObj? canvas = null)
        {
            bool isCollide = false;

            var tempObjects = canvas != null ? canvas.Children : Objects;

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
            }
        }
    }
}
