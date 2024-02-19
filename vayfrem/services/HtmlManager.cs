using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            htmlBuilder.Append(new String('\t', 2)).AppendLine("<link href='style.css' rel='stylesheet'/>");
            htmlBuilder.Append(new String('\t', 1)).AppendLine("</head>");
            htmlBuilder.Append(new String('\t', 1)).AppendLine("<body>");
            counter = 2;
            HtmlBuild(file.Objects, htmlBuilder, counter);

            htmlBuilder.Append(new String('\t', 1)).AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

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
                    if (canvasObj.Role == models.enums.CanvasRole.Content)
                    {

                    }
                    if (canvasObj.Role == models.enums.CanvasRole.ContentFluid)
                    {

                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Row)
                    {

                    }
                    if (canvasObj.Role == models.enums.CanvasRole.Column)
                    {

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

                    htmlBuilder.Append(new String('\t', counter)).AppendLine("<p>");
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
            cssBuilder.AppendLine("body {");
            cssBuilder.Append(new String('\t', 1)).AppendLine("");
            cssBuilder.AppendLine("}");

            List<string> classes = new List<string>();
            List<string> ids = new List<string>();

            foreach (var obj in objects)
            {
                cssBuilder.Append($"#{obj.Tag}").AppendLine("{");
                cssBuilder.Append(new String('\t', 1)).Append("background:").AppendLine($"#{obj.BackgroundColor.ToHex()};");
                cssBuilder.Append(new String('\t', 1)).Append("opacity:").AppendLine($"{(obj.Opacity/255.0).ToString().Replace(',', '.')};");
                cssBuilder.Append(new String('\t', 1)).Append("width:").AppendLine($"{obj.Width.ToString()}px;");
                cssBuilder.Append(new String('\t', 1)).Append("height:").AppendLine($"{obj.Height.ToString()}px;");
                cssBuilder.Append(new String('\t', 1)).Append("border-color:").AppendLine($"#{obj.BorderColor.ToHex()};");
                cssBuilder.Append(new String('\t', 1)).Append("border:").AppendLine($"{(int)obj.BorderThickness}px solid #{obj.BorderColor.ToHex()};");
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
    }
}
