﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.dtos
{
    public class QuadraticBCToolDTO
    {
        public ColorPickerDTO BackgroundColorPicker { get; set; } = new ColorPickerDTO
        {
            Color = new ColorDTO(255, 0, 0, 0),
            ColorSelectPosition = new structs.Vector2(0, 0),
            BarPosition = new structs.Vector2(0, 0)
        };

        public double Opacity { get; set; }
        public double BorderThickness { get; set; } = 1;
        public BorderDTO BorderDTO { get; set; } = new BorderDTO();
        public ColorPickerDTO BorderColorPicker { get; set; } = new ColorPickerDTO
        {
            Color = new ColorDTO(255, 0, 0, 0),
            ColorSelectPosition = new structs.Vector2(0, 0),
            BarPosition = new structs.Vector2(0, 0)
        };
    }
}
