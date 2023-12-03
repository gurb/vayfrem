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
            // if object type is Canvas
            AddCanvas(passData);
        }


        private void AddCanvas(PassData passData)
        {
            CanvasObj canvasObj = new CanvasObj();
            canvasObj.X = passData.X;
            canvasObj.Y = passData.Y;
            canvasObj.Width = passData.Width;
            canvasObj.Height = passData.Height;

            Objects.Add(canvasObj);
        }

        public void CollisionDetect(Vector2 mousePosition)
        {
            bool isCollide = false;

            foreach (var obj in Objects)
            {
                if (mousePosition.X >= obj.X &&
                    mousePosition.X <= obj.X + obj.Width &&
                    mousePosition.Y >= obj.Y &&
                    mousePosition.Y <= obj.Y + obj.Height)
                {

                    

                    SelectedObject = obj;
                    isCollide = true;
                }
            }

            if(!isCollide)
            {
                SelectedObject = null;
            }
        }
    }
}
