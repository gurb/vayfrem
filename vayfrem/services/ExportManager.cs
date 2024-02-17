using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Imaging;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private readonly PdfManager pdfManager;
        private readonly HTMLManager bootstrapManager;

        public ExportManager()
        { 
            projectManager = App.GetService<ProjectManager>();
            renderManager = App.GetService<RenderManager>();
            drawingViewModel = App.GetService<DrawingViewModel>();
            pdfManager = App.GetService<PdfManager>();
            bootstrapManager = App.GetService<HTMLManager>();
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

        public RenderTargetBitmap GenerateCurrentPng(bool ignoreText = false)
        {
            Canvas display = new Canvas();
            display.Background = Avalonia.Media.Brushes.White;
            display.Width = drawingViewModel.CurrentFile!.PageWidth;
            display.Height = drawingViewModel.CurrentFile!.PageHeight;

            renderManager.Render(display, drawingViewModel.CurrentFile.Objects, ignoreText);

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
                    bitmaps.Add(GetBitmap((vayfrem.models.File)node));
                }   
            }

            return bitmaps;
        }

        private RenderTargetBitmap GetBitmap(vayfrem.models.File file)
        {
            Canvas display = new Canvas();
            display.Background = Avalonia.Media.Brushes.White;
            display.Width = file.PageWidth;
            display.Height = file.PageHeight;

            renderManager.Render(display, file.Objects, true);

            display.UpdateLayout();
            display.Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            display.Arrange(new Rect(display.DesiredSize));

            var renderSize = new PixelSize((int)display.Width, (int)display.Height);

            var bitmap = new RenderTargetBitmap(renderSize, new Vector(96, 96));
            bitmap.Render(display);

            return bitmap;
        }

        public void GenerateCurrentPdf(string path)
        {
            PdfDocument document = new PdfDocument();
            document.Options.ColorMode = PdfColorMode.Rgb;

            if (document.Version < 14)
                document.Version = 14;

            RenderTargetBitmap bitmap = GenerateCurrentPng(true);

            pdfManager.DrawPage(document, drawingViewModel.CurrentFile!, bitmap);

            document.Save(path);
        }

        public void GenerateAllPdf(string path)
        {
            PdfDocument document = new PdfDocument();
            document.Options.ColorMode = PdfColorMode.Rgb;

            if (document.Version < 14)
                document.Version = 14;

            List<RenderTargetBitmap> list = GenerateAllPagesPng();

            foreach (var item in list)
            {
                pdfManager.DrawPage(document, drawingViewModel.CurrentFile!, item);
            }

            document.Save(path);
        }

        public void GenerateCurrentHTML(string path)
        {
            bootstrapManager.Generate(drawingViewModel.CurrentFile!, path);
        }

        public void GenerateAllHTML(string path)
        {
            bootstrapManager.GenerateProject(projectManager.CurrentProject!.Nodes, path);
        }
    }
}
