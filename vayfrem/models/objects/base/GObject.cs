﻿using Avalonia.Controls;
using vayfrem.models.dtos;
using vayfrem.models.enums;
using vayfrem.models.structs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia.Media;
using vayfrem.models.objects.components;

namespace vayfrem.models.objects.@base
{
    [JsonDerivedType(typeof(CanvasObj), typeDiscriminator: "CanvasObjType")]
    [JsonDerivedType(typeof(TextObj), typeDiscriminator: "TextObjType")]
    [JsonDerivedType(typeof(ButtonObj), typeDiscriminator: "ButtonObjType")]
    [JsonDerivedType(typeof(SvgObj), typeDiscriminator: "SvgObjType")]
    [JsonDerivedType(typeof(QuadraticBCObj), typeDiscriminator: "QuadraticBCObjType")]
    [JsonDerivedType(typeof(ImageObj), typeDiscriminator: "ImageObjType")]
    public class GObject : IObject
    {
        public string? Guid { get; set; }
        public string? ParentGuid { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ZIndex { get; set; } = 1;
        public double Opacity { get; set; }
        public ColorDTO BackgroundColor { get; set; } = new ColorDTO(255, 255, 255, 255);
        public ColorDTO BorderColor { get; set; } = new ColorDTO(0, 0, 0, 255);
        public double BorderThickness { get; set; }
        public BorderDTO BorderDTO { get; set; } = new BorderDTO();
        public double BorderRadius { get; set; }
        public bool IsBoxShadowActive { get; set; }
        public BoxShadowDTO BoxShadow { get; set; } = new BoxShadowDTO();
        public ColorDTO BoxShadowColor { get; set; } = new ColorDTO(255, 100, 100, 100);
        public ObjectType ObjectType { get; set; }

        public double CalcX
        {
            get
            {
                if(Parent != null)
                {
                    if (Parent.BorderDTO.Relative)
                    {
                        return Y + Parent.BorderDTO.LeftThickness;
                    }
                    return X + Parent.BorderDTO.Thickness;
                }
                else
                {
                    return X; 
                }
            }
        }

        public double CalcY
        {
            get
            {
                if (Parent != null)
                {
                    if(Parent.BorderDTO.Relative)
                    {
                        return Y + Parent.BorderDTO.TopThickness;
                    }
                    return Y + Parent.BorderDTO.Thickness;
                }
                else
                {
                    return Y;
                }
            }
        }


        public Vector2 TopLeft { 
            get 
            {
                return new Vector2(WorldX, WorldY);    
            } 
        }

        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2(WorldX, WorldY + Height);
            }
        }

        public Vector2 TopRight
        {
            get
            {
                return new Vector2(WorldX + Width, WorldY);
            }
        }

        public Vector2 BottomRight
        {
            get
            {
                return new Vector2(WorldX + Width, WorldY + Height);
            }
        }

        public List<string>? Classes { get; set; } = new List<string>() { "className" };
        public string? Tag { 
            get 
            {
                return "id-" + Guid;
            }
        }

        [JsonIgnore]
        public bool IsDrew { get; set; }

        [JsonIgnore]
        public bool IsVisible { get; set; } = true;

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public IObject? Parent { get; set; }

        [JsonIgnore]
        public Control? ConnectedMenuControl { get; set; }

        [JsonIgnore]
        public Control? ConnectedTabControl { get; set; }

        [JsonIgnore]
        public Control? CaretControl { get; set; }

        [JsonIgnore]
        public Control? CloseControl { get; set; }

        [JsonIgnore]
        public ContextMenu? ContextMenu { get; set; }

        public List<Property> Properties { get; set; } = new List<Property>();

        public double WorldX
        {
            get
            {
                if (Parent == null)
                    return X;
                else
                    return X + Parent.WorldX;
            }
        }

        public double WorldY
        {
            get
            {
                if (Parent == null)
                    return Y;
                else
                    return Y + Parent.WorldY;
            }
        }



        public double BorderOffsetX
        {
            get
            {
                if (Parent == null)
                {
                    if (BorderDTO.Relative)
                    {
                        return BorderDTO.LeftThickness;
                    }
                    else
                    {
                        return  BorderDTO.Thickness;

                    }
                }
                else
                {
                    if(BorderDTO.Relative)
                    {
                        return BorderDTO.LeftThickness + Parent.BorderOffsetX;
                    }
                    else
                    {
                        return BorderDTO.Thickness + Parent.BorderOffsetX;

                    }
                }
            }
        }

        public double BorderOffsetWidth
        {
            get
            {
                if (BorderDTO.Relative)
                {
                    return BorderDTO.LeftThickness + BorderDTO.RightThickness;
                }
                else
                {
                    return BorderDTO.Thickness*2;
                }
            }
        }

        public double ParentBorderOffsetX
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }
                else
                {
                    if (BorderDTO.Relative)
                    {
                        return Parent.BorderOffsetX;
                    }
                    else
                    {
                        return Parent.BorderOffsetX;

                    }
                }
            }
        }

        public double ParentBorderOffsetY
        {
            get
            {
                if (Parent == null)
                {
                    return 0;
                }
                else
                {
                    if (BorderDTO.Relative)
                    {
                        return Parent.BorderOffsetY;
                    }
                    else
                    {
                        return Parent.BorderOffsetY;

                    }
                }
            }
        }



        public double BorderOffsetY
        {
            get
            {
                if (Parent == null)
                {
                    if (BorderDTO.Relative)
                    {
                        return BorderDTO.TopThickness;
                    }
                    else
                    {
                        return BorderDTO.Thickness;

                    }
                }
                else
                {
                    if (BorderDTO.Relative)
                    {
                        return BorderDTO.TopThickness + Parent.BorderOffsetY;
                    }
                    else
                    {
                        return BorderDTO.Thickness + Parent.BorderOffsetY;

                    }
                }
            }
        }


        public virtual GObject Copy()
        {
            GObject obj = new GObject();
            return obj;
        }

        public virtual void InitializeObject () 
        {
        }

        public virtual void SetStyle()
        {

        }

        public Avalonia.Media.TextAlignment TextAlignmentConverter(enums.TextAlignment textAlignment)
        {
            if(textAlignment == enums.TextAlignment.MiddleCenter)
            {
                return Avalonia.Media.TextAlignment.Center;
            }
            else
            {
                return Avalonia.Media.TextAlignment.Left;
            }
        }
    }
}
