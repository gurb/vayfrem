using draftio.models.dtos;
using draftio.models.interfaces;
using draftio.models.objects;
using draftio.models.objects.@base;
using System;
using System.Linq;

namespace draftio.models.commands
{
    public class AddCommand : ICommand
    {
        public GObject? Object { get; set; }
        public string? ParentGuid { get; set; }

        public AddCommand(GObject? addedObject)
        {
            Object = addedObject;
            ParentGuid = addedObject.ParentGuid;
        }

        public void Execute(UndoRedoDTO urdto)
        {
            if(ParentGuid != null)
            {
                var parentGObject = urdto.objects.FirstOrDefault(x => x.Guid == Object!.ParentGuid);

                if(parentGObject != null)
                {
                    Object!.Parent = parentGObject;
                    
                    if(parentGObject!.ObjectType == enums.ObjectType.Canvas)
                    {
                        CanvasObj parentCanvas = (CanvasObj)parentGObject;
                        parentCanvas.Children.Add(Object);
                    }
                }
            }else
            {
                urdto.page.Objects.Add(Object);
            }
        }
    }
}
