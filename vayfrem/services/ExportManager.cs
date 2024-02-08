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
        public ExportManager()
        { 
            projectManager = App.GetService<ProjectManager>();
            renderManager = App.GetService<RenderManager>();
            drawingViewModel = App.GetService<DrawingViewModel>();
            pdfManager = App.GetService<PdfManager>();
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

            renderManager.Render(display, file.Objects);

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

            // PDF dosyasını kaydet
            //document.Save("example.pdf");
            //XRect rect = new XRect(100, 100, 200, 100);

            //XBrush brush = XBrushes.Red; // Dikdörtgenin dolgu rengi
            //XPen pen = new XPen(XColors.Black, 2); // Dikdörtgenin kenar rengi ve kalınlığı
            //gfx.DrawRectangle(pen, brush, rect);

            //// Dikdörtgen içine metin ekle
            //XFont font = new XFont("Arial", 12); // Yazı tipi ve boyutu
            //XStringFormat format = new XStringFormat(); // Metin formatı (opsiyonel)
            //format.Alignment = XStringAlignment.Center; // Metnin ortalanması
            //format.LineAlignment = XLineAlignment.Center; // Metnin dikey ortalanması
            //gfx.DrawString("Merhaba, Dikdörtgen!", font, XBrushes.White, rect, format);

            document.Save(path);
        }
    }
}
