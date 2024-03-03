using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;
using vayfrem.models.objects.components;

namespace vayfrem.services
{
    public class HTMLManager
    {
        int counter;


        public HTMLManager() { }

        public void Generate(vayfrem.models.File file, string dir, string path)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            StringBuilder cssBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.Append(new String('\t', 1)).AppendLine("<head>");
            htmlBuilder.Append(new String('\t', 2)).AppendLine("<meta charset = \"utf-8\">");
            htmlBuilder.Append(new String('\t', 2)).AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            htmlBuilder.Append(new String('\t', 2)).AppendLine("<title>Test</title>");
            htmlBuilder.Append(new String('\t', 2)).AppendLine("<link href='style.css' rel='stylesheet'/>");
            htmlBuilder.Append(new String('\t', 2)).AppendLine("<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC\" crossorigin=\"anonymous\">");
            htmlBuilder.Append(new String('\t', 1)).AppendLine("</head>");
            htmlBuilder.Append(new String('\t', 1)).AppendLine("<body>");
            counter = 2;
            HtmlBuild(file.Objects, htmlBuilder, counter);

            htmlBuilder.Append('\t', 2).AppendLine("<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM\" crossorigin=\"anonymous\"></script>");
            htmlBuilder.Append(new String('\t', 1)).AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");


            cssBuilder.AppendLine("body {");
            cssBuilder.Append(new String('\t', 1)).AppendLine("");
            cssBuilder.AppendLine("}");
            CSSBuild(file.Objects, cssBuilder);

            SaveFile(htmlBuilder, cssBuilder, dir, path);
        }

        private void SaveFile(StringBuilder htmlBuilder, StringBuilder cssBuilder, string dir, string path)
        {
            string htmlCode = htmlBuilder.ToString();
            string cssCode = cssBuilder.ToString();

            System.IO.File.WriteAllText(path, htmlCode);
            System.IO.File.WriteAllText(dir + "/style.css", cssCode);
        }


        private void HtmlBuild(List<GObject> objects, StringBuilder htmlBuilder, int counter)
        {
            foreach (var obj in objects)
            {

                if (obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    if (canvasObj.Role == models.enums.CanvasRole.None)
                    {
                        htmlBuilder.Append(new String('\t', counter)).AppendLine($"<div id='{canvasObj.Tag}'>");
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Container)
                    {
                        htmlBuilder.Append(new String('\t', counter)).AppendLine($"<div class='container' id='{canvasObj.Tag}'>");
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.ContainerFluid)
                    {
                        htmlBuilder.Append(new String('\t', counter)).AppendLine($"<div class='container-fluid' id='{canvasObj.Tag}'>");
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Row)
                    {
                        htmlBuilder.Append(new String('\t', counter)).AppendLine($"<div class='row' id='{canvasObj.Tag}'>");
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Column)
                    {
                        htmlBuilder.Append(new String('\t', counter)).AppendLine($"<div class='col' id='{canvasObj.Tag}'>");
                    }

                    counter = counter + 1;
                    if (canvasObj.Children.Count() > 0)
                    {
                        HtmlBuild(canvasObj.Children, htmlBuilder, counter);
                    }
                    counter = counter - 1;

                    htmlBuilder.Append(new String('\t', counter)).AppendLine("</div>");
                }

                if(obj.ObjectType == models.enums.ObjectType.Text)
                {
                    TextObj textObj = (TextObj)obj; 

                    htmlBuilder.Append(new String('\t', counter)).AppendLine($"<p id='{textObj.Tag}'>");
                    htmlBuilder.Append(textObj.Text);
                    htmlBuilder.AppendLine("</p>");
                }

                if (obj.ObjectType == models.enums.ObjectType.Image)
                {
                    ImageObj textObj = (ImageObj)obj;

                    htmlBuilder.Append(new String('\t', counter)).Append("<img src='data:image/png;base64,");
                    htmlBuilder.Append(textObj.Base64);
                    htmlBuilder.AppendLine("'>");
                    htmlBuilder.Append(new String('\t', counter)).AppendLine("</img>");
                }

                if (obj.ObjectType == models.enums.ObjectType.Button)
                {
                    ButtonObj buttonObj = (ButtonObj)obj;

                    htmlBuilder.Append(new String('\t', counter)).AppendLine("<button class='btn' href='#' role='button'>");
                    htmlBuilder.AppendLine(buttonObj.Text);
                    htmlBuilder.Append(new String('\t', counter)).AppendLine("</button>");
                }
            }
        }

        public void GenerateProject(ObservableCollection<Node> nodes, string path)
        {

        }

        private void CSSBuild(List<GObject> objects, StringBuilder cssBuilder)
        {
            

            List<string> classes = new List<string>();
            List<string> ids = new List<string>();

            

            foreach (var obj in objects)
            {
                bool ignore = false;
                bool isCol = false;
                bool isRow = false;
                bool isContainer = false;
                bool isContainerFluid = false;

                if (obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    if (canvasObj.Role == models.enums.CanvasRole.ContainerFluid || 
                        canvasObj.Role == models.enums.CanvasRole.Container ||
                        canvasObj.Role == models.enums.CanvasRole.Row ||
                        canvasObj.Role == models.enums.CanvasRole.Column)
                    {
                        ignore = true;
                    }
                    if(canvasObj.Role == models.enums.CanvasRole.Column)
                    {
                        isCol = true;
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Row)
                    {
                        isRow = true;
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Container)
                    {
                        isContainer = true;
                    }
                    if (canvasObj.Role == models.enums.CanvasRole.ContainerFluid)
                    {
                        isContainerFluid = true;
                    }
                }

                cssBuilder.Append($"#{obj.Tag}").AppendLine("{");
                cssBuilder.Append(new String('\t', 1)).Append("background-color:").AppendLine($"{obj.BackgroundColor.HTMLColor((byte)obj.Opacity)};");

                int percWidth, percHeight, percX, percY;
                if (obj.Parent != null)
                {
                    percWidth = GetPercentage(obj.Width, obj.Parent.Width);
                    percHeight = GetPercentage(obj.Height, obj.Parent.Height);
                    percX = GetPercentage(obj.X, obj.Parent.Width);
                    percY = GetPercentage(obj.Y, obj.Parent.Height);
                }
                else
                {
                    percWidth = GetPercentage(obj.Width, 1920);
                    percHeight = GetPercentage(obj.Height, 1080);
                    percX = GetPercentage(obj.X, 1920);
                    percY = GetPercentage(obj.Y, 1080);
                }

                if (!ignore && obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    cssBuilder.Append(new String('\t', 1)).Append("position:").AppendLine($"absolute;");
                    cssBuilder.Append(new String('\t', 1)).Append("left:").AppendLine($"{percX}%;");
                    cssBuilder.Append(new String('\t', 1)).Append("top:").AppendLine($"{percY}%;");
                    cssBuilder.Append(new String('\t', 1)).Append("overflow:").AppendLine($"hidden;");
                    cssBuilder.Append(new String('\t', 1)).Append("height:").AppendLine($"{percHeight}%;");
                    cssBuilder.Append(new String('\t', 1)).Append("width:").AppendLine($"{percWidth}%;");
                }
                if(isCol && obj.Parent != null)
                {
                    cssBuilder.Append(new String('\t', 1)).Append("flex:").AppendLine($"{(obj.Width/obj.Parent!.Width).ToString().Replace(',', '.')} 0 0%;");
                    cssBuilder.Append(new String('\t', 1)).Append("min-height:").AppendLine($"{obj.Height}px;");
                    cssBuilder.Append(new String('\t', 1)).Append("margin-right:").AppendLine($"{15}px;");
                }

                if (isRow && obj.Parent != null)
                {
                    cssBuilder.Append(new String('\t', 1)).Append("padding:").AppendLine($"{15}px;");
                    cssBuilder.Append(new String('\t', 1)).Append("margin-bottom:").AppendLine($"{15}px;");
                }

                if (isContainer)
                {
                    cssBuilder.Append(new String('\t', 1)).Append("padding:").AppendLine($"{15}px;");
                }

                if (isContainerFluid)
                {
                    cssBuilder.Append(new String('\t', 1)).Append("padding:").AppendLine($"{15}px;");
                }

                if(obj.ObjectType == models.enums.ObjectType.Text)
                {
                    TextObj textObj = (TextObj)obj;

                    cssBuilder.Append(new String('\t', 1)).Append("font-size:").AppendLine($"{textObj.FontSize}px;");
                    cssBuilder.Append(new String('\t', 1)).Append("color:").AppendLine($"#{textObj.FontColor.ToHex()};");
                    cssBuilder.Append(new String('\t', 1)).Append("font-weight:").AppendLine($"{(int)textObj.FontWeight};");
                }

                cssBuilder.Append(new String('\t', 1)).Append("border-color:").AppendLine($"#{obj.BorderColor.ToHex()};");
                cssBuilder.Append(new String('\t', 1)).Append("border:").AppendLine($"{(int)obj.BorderDTO.Thickness}px solid #{obj.BorderColor.ToHex()};");
                cssBuilder.Append(new String('\t', 1)).Append("border-radius:").AppendLine($"{(int)obj.BorderRadius}px");
                cssBuilder.AppendLine("}");

                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    if (canvasObj.Children.Count() > 0)
                    {
                        CSSBuild(canvasObj.Children, cssBuilder);
                    }
                }
            }

            cssBuilder.AppendLine();
        }

        private int GetPercentage(double width, double parent)
        {
            return (int)((width / (double)parent) * 100);
        }
    }
}
