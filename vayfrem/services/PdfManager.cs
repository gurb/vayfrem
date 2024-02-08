using Avalonia.Media.Imaging;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vayfrem.models;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;

namespace vayfrem.services
{
    public class PdfManager
    {
        public PdfManager() 
        {
        
        }


        public void DrawPage(PdfDocument document, vayfrem.models.File file, RenderTargetBitmap bitmap)
        {
            PdfPage page = document.AddPage();
            page.Width = XUnit.FromPoint(file.PageWidth);
            page.Height = XUnit.FromPoint(file.PageHeight);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(gfx);
            XBrush bg = XBrushes.White;
            XRect rect = new XRect(0, 0, page.Width, page.Height);
            gfx.DrawRectangle(bg, rect);


            SKBitmap skBitmap;
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (SKManagedStream skStream = new SKManagedStream(stream))
                {
                    skBitmap = SKBitmap.Decode(skStream);
                }
            }

            SKData data = skBitmap.Encode(SKEncodedImageFormat.Png, 100);

            XImage image = XImage.FromStream(() => data.AsStream());
            data.Dispose();

            // Resmi belirli bir dikdörtgen içine yerleştirin
            gfx.DrawImage(image, 0, 0, page.Width, page.Height);


            Render(page, file.Objects, gfx, null, tf);

            gfx.Dispose();
        }

        private void Render(PdfPage page, List<GObject> objects, XGraphics gfx, GObject? parent, XTextFormatter tf)
        {
            foreach (var obj in objects)
            {
                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    if(canvasObj.Children.Count() > 0)
                    {
                        Render(page, canvasObj.Children, gfx, obj, tf);
                    }
                }

                if (obj.ObjectType == models.enums.ObjectType.Text)
                {
                    TextObj textObj = (TextObj)obj;

                    double x = textObj.WorldX;
                    double y = textObj.WorldY;
                    double width = textObj.Width;
                    double height = textObj.Height;

                    if(textObj.ParentGuid != null)
                    {
                         
                    }

                    XRect rect = new XRect(x, y, width, height);
                    XBrush bg = new XSolidBrush(obj.BackgroundColor.ToXColor(0));
                    
                    XPen border = new XPen(obj.BorderColor.ToXColor(), obj.BorderThickness);

                    gfx.DrawRectangle(bg, rect);

                    XFont font = new XFont(textObj.FontFamily, textObj.FontSize); // Yazı tipi ve boyutu
                    XStringFormat format = new XStringFormat();
                    format.LineAlignment = XLineAlignment.Near;
                    format.Alignment = XStringAlignment.Near; // Metin formatı (opsiyonel)
                    //format.LineAlignment = XLineAlignment.Center; // Metnin dikey ortalanması
                    
                    tf.DrawString(textObj.Text, font, new XSolidBrush(textObj.FontColor.ToXColor()), rect, format);
                }
            }
        }
        
        //private XRect CheckOverflow(GObject parent, GObject child)
        //{
        //    //if()
        //}
    
    }


}
