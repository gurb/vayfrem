﻿using Avalonia.Animation.Easings;
using vayfrem.models;
using vayfrem.models.commands;
using vayfrem.models.dtos;
using vayfrem.models.interfaces;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.services
{
    public class UndoRedoManager
    {

        private readonly ProjectManager projectManager;

        public Dictionary<string, Stack<ICommand>> UndoLogs { get; set; }
        public Dictionary<string, Stack<ICommand>> RedoLogs { get; set; }
        public Dictionary<string, List<GObject>> HardCoded { get; set; }

        private List<GObject>? currentFileObjects = new List<GObject>();


        public UndoRedoManager() 
        {
            projectManager = App.GetService<ProjectManager>();

            UndoLogs = new Dictionary<string, Stack<ICommand>>();
            RedoLogs = new Dictionary<string, Stack<ICommand>>();
            HardCoded = new Dictionary<string, List<GObject>>();
        }

        // save objects from saved and opened project file
        public void SetHardCoded(string guid, List<GObject> hardCodedObjects)
        {
            if(hardCodedObjects.Count == 0)
            {
                return;
            }

            if (!HardCoded.ContainsKey(guid))
            {
                HardCoded[guid] = new List<GObject>();
            }

            foreach (var obj in hardCodedObjects)
            {
                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    var canvasObj = (CanvasObj)obj;
                    SetHardCoded(guid, canvasObj.Children);
                }

                HardCoded[guid].Add(obj);
            }
        }


        public void SetCurrentFileObjects (List<GObject> objects)
        {
            currentFileObjects!.Clear();

            foreach (var item in objects)
            {
                currentFileObjects.Add(item);
            }
        }

        public void AddCommand(string guid, ICommand command)
        {
            if(!UndoLogs.ContainsKey(guid))
            {
                UndoLogs[guid] = new Stack<ICommand>();
            }

            UndoLogs[guid].Push(command);

            if (RedoLogs.ContainsKey(guid))
            {
                RedoLogs[guid].Clear();
            }
            else
            {
                RedoLogs[guid] = new Stack<ICommand>();
            }
        }

        public void Undo(string guid)
        {
            if (UndoLogs.ContainsKey(guid) && UndoLogs[guid].Count > 0)
            {
                ICommand command = UndoLogs[guid].Pop();

                RedoLogs[guid].Push(command);

                Execute(guid);
            }
        }

        public bool CheckUndo(string guid)
        {
            if (UndoLogs.ContainsKey(guid))
            {
                if (UndoLogs[guid].Count > 0) return true;
                else return false;
            }
            return false;
        }

        public void Redo(string guid)
        {
            if (RedoLogs.ContainsKey(guid) && RedoLogs[guid].Count > 0)
            {
                ICommand command = RedoLogs[guid].Pop();

                UndoLogs[guid].Push(command);

                Execute(guid);
            }
        }

        public bool CheckRedo(string guid)
        {
            if (RedoLogs.ContainsKey(guid))
            {
                if (RedoLogs[guid].Count > 0) return true;
                else return false;
            }
            return false;
        }

        public void Execute(string guid)
        {
            // we can reach the page node via guid parameter, and we can handle changes of object according to the command logs.

            var node = GetNode(guid);

            if(HardCoded.ContainsKey(guid))
            {
                HardCodedCommand hc_command = new HardCodedCommand(HardCoded[guid]);

                UndoRedoDTO hc_urdto = new UndoRedoDTO
                {
                    page = node,
                };

                hc_command.Execute(hc_urdto);
            }

            if (UndoLogs.ContainsKey(guid) && UndoLogs[guid].Count > 0 && node != null)
            {
                foreach (var command in UndoLogs[guid])
                {
                    UndoRedoDTO urdto = new UndoRedoDTO
                    {
                        page = node,
                        objects = currentFileObjects
                    };

                    command.Execute(urdto);
                }
            }
        }

        private File? GetNode(string guid)
        {
            if(projectManager.CurrentProject != null)
            {
                var node = projectManager.CurrentProject.Nodes.FirstOrDefault(x => x.Guid == guid);
                if(node != null && node.Type == models.enums.NodeType.File)
                {
                    return (File)node;
                }
            }
            return null;
        }
    }
}
