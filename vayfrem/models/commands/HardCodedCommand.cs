﻿using vayfrem.models.dtos;
using vayfrem.models.interfaces;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.commands
{
    public class HardCodedCommand: ICommand
    {
        public List<GObject>? Objects { get; set; } = new List<GObject>();

        public HardCodedCommand(List<GObject>? hardCodedObjects)
        {
            if(hardCodedObjects != null)
            {
                Objects.AddRange(hardCodedObjects);
            }
        }

        public void Execute(UndoRedoDTO urdto) 
        {
            foreach (var obj in Objects)
            {
                if(obj.ParentGuid != null)
                {
                    var parentObj = Objects.FirstOrDefault(x => x.Guid == obj.ParentGuid);

                    if(parentObj != null)
                    {
                        var canvasObj = (CanvasObj)parentObj;
                        obj.Parent = parentObj;
                        canvasObj.Children.Add(obj);
                    }
                } else
                {
                    urdto.page!.Objects.Add(obj!);
                }
            }

        }
    }
}
