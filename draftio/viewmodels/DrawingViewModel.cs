using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models.dtos;
using draftio.models.objects;
using System.Collections.Generic;

namespace draftio.viewmodels
{
    public partial class DrawingViewModel : ObservableObject
    {
        [ObservableProperty]
        List<IObject> objects = new();

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
    }
}
