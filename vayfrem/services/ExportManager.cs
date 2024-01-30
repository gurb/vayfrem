using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models;
using vayfrem.viewmodels;

namespace vayfrem.services
{
    public class ExportManager
    {
        private readonly ProjectManager projectManager;
        private readonly RenderManager renderManager;
        private readonly DrawingViewModel drawingViewModel;
        public ExportManager()
        { 
            projectManager = App.GetService<ProjectManager>();
            renderManager = App.GetService<RenderManager>();
            drawingViewModel = App.GetService<DrawingViewModel>();
        }

        public bool ProjectValid()
        {
            bool result = projectManager.CurrentProject!.Nodes.Any(x => x.Type == models.enums.NodeType.File);

            return result;
        }

        public bool CurrentFileValid()
        {
            if(drawingViewModel.CurrentFile != null)
            {
                return true;
            }

            return false;
        }

        public RenderTargetBitmap GenerateCurrentPng()
        {
            Canvas display = new Canvas();
            display.Background = Avalonia.Media.Brushes.White;
            display.Width = drawingViewModel.CurrentFile!.PageWidth;
            display.Height = drawingViewModel.CurrentFile!.PageHeight;

            renderManager.Render(display, drawingViewModel.CurrentFile.Objects);

            display.UpdateLayout();
            display.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            display.Arrange(new Rect(display.DesiredSize));

            var renderSize = new PixelSize((int)display.Width, (int)display.Height);

            var bitmap = new RenderTargetBitmap(renderSize, new Vector(96, 96));
            bitmap.Render(display);

            return bitmap;
        }

        public List<RenderTargetBitmap> GenerateAllPagesPng()
        {
            List<RenderTargetBitmap> bitmaps = new List<RenderTargetBitmap>();

            foreach (var node in projectManager.CurrentProject!.Nodes)
            {
                if(node.Type == models.enums.NodeType.File)
                {
                    bitmaps.Add(GetBitmap((File)node));
                }   
            }

            return bitmaps;
        }

        private RenderTargetBitmap GetBitmap(File file)
        {
            Canvas display = new Canvas();
            display.Background = Avalonia.Media.Brushes.White;
            display.Width = file.PageWidth;
            display.Height = file.PageHeight;

            renderManager.Render(display, file.Objects);

            display.UpdateLayout();
            display.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            display.Arrange(new Rect(display.DesiredSize));

            var renderSize = new PixelSize((int)display.Width, (int)display.Height);

            var bitmap = new RenderTargetBitmap(renderSize, new Vector(96, 96));
            bitmap.Render(display);

            return bitmap;
        }
    }
}
