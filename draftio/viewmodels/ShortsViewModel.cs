using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.dtos;
using draftio.models.objects;
using draftio.models.objects.@base;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace draftio.viewmodels
{
    public partial class ShortsViewModel : ObservableObject
    {
        private readonly FileManager fileManager;
        private readonly IOManager ioManager;
        private readonly ProjectManager projectManager;
        private readonly UndoRedoManager undoRedoManager;

        [ObservableProperty]
        bool isSaved = false;

        [ObservableProperty]
        bool isUndo = false;

        [ObservableProperty]
        bool isRedo = false;

        File? currentFile;


        public delegate void ChangeViewDelegate();
        public ChangeViewDelegate? changeDelegate;

        public delegate void ProjectVMDelegate();
        public ProjectVMDelegate? refreshProjectVM;

        public delegate void DrawDelegate(File node);
        public DrawDelegate? drawDelegate;

        public ShortsViewModel()
        {
            fileManager = App.GetService<FileManager>();
            ioManager = App.GetService<IOManager>();
            projectManager = App.GetService<ProjectManager>();
            undoRedoManager = App.GetService<UndoRedoManager>();

            if (currentFile == null)
            {
                IsSaved = true;
            }
        }

        [RelayCommand]
        public void ChangeSaveState(bool saveState)
        {
            IsSaved = saveState;
            if(changeDelegate != null)
            {
                changeDelegate.Invoke();
            }
        }


        [RelayCommand]
        public void ChangeUndoState(bool undoState)
        {
            IsUndo = undoState;
            if (changeDelegate != null)
            {
                changeDelegate.Invoke();
            }
        }


        [RelayCommand]
        public void ChangeRedoState(bool redoState)
        {
            IsRedo = redoState;
            if (changeDelegate != null)
            {
                changeDelegate.Invoke();
            }
        }

        public void SetCurrentFile(File? file)
        {
            currentFile = file;

            ChangeUndoState(undoRedoManager.CheckUndo(currentFile.Guid!));
            ChangeRedoState(undoRedoManager.CheckRedo(currentFile.Guid!));
        }

        public VMResponse LoadProject(string data)
        {
            VMResponse response = new VMResponse();

            response = ioManager.LoadProject(data);

            projectManager.CurrentProject = ((SaveProjectData)response.Result!).Project;
            projectManager.projects.Add(projectManager.CurrentProject!);

            if(refreshProjectVM != null)
            {
                refreshProjectVM.Invoke();
            }


            return response;
        }

        // save only current page
        public VMResponse Save()
        {
            VMResponse response = new VMResponse();
            
            if(currentFile != null)
            {
                currentFile.IsSaved = true;

                SaveProjectData saveProjectData = new SaveProjectData
                {
                    Project = projectManager.CurrentProject
                };

                response = ioManager.SaveFile(saveProjectData);

                ChangeSaveState(true);
            }
            else
            {
                response.Success = false;
                response.Message = "Not found current page for save operation!";
            }

            return response;
        }

        public VMResponse Undo()
        {
            VMResponse response = new VMResponse();

            if(currentFile != null)
            {
                List<GObject> objects = CopyObjects(currentFile.Objects);

                foreach(var obj in currentFile.Objects)
                {
                    if(obj.ObjectType == models.enums.ObjectType.Canvas)
                    {
                        var canvasObj = (CanvasObj)obj;
                        canvasObj.Children.Clear();
                    }
                }
                currentFile.Objects.Clear();

                undoRedoManager.SetCurrentFileObjects(objects);
                undoRedoManager.Undo(currentFile.Guid!);

                if (drawDelegate != null)
                {
                    drawDelegate.Invoke(currentFile);
                }

                ChangeUndoState(undoRedoManager.CheckUndo(currentFile.Guid!));
                ChangeRedoState(undoRedoManager.CheckRedo(currentFile.Guid!));

            }

            return response;
        }

        public VMResponse Redo()
        {
            VMResponse response = new VMResponse();

            if(currentFile != null)
            {
                List<GObject> objects = CopyObjects(currentFile.Objects);

                foreach (var obj in currentFile.Objects)
                {
                    if (obj.ObjectType == models.enums.ObjectType.Canvas)
                    {
                        var canvasObj = (CanvasObj)obj;
                        canvasObj.Children.Clear();
                    }
                }
                currentFile.Objects.Clear();

                undoRedoManager.SetCurrentFileObjects(objects);
                undoRedoManager.Redo(currentFile.Guid!);

                if (drawDelegate != null)
                {
                    drawDelegate.Invoke(currentFile);
                }

                ChangeUndoState(undoRedoManager.CheckUndo(currentFile.Guid!));
                ChangeRedoState(undoRedoManager.CheckRedo(currentFile.Guid!));
            }

            return response;
        }


        private List<GObject> CopyObjects(List<GObject> param)
        {
            List<GObject> objects = new List<GObject>();

            foreach(var obj in param)
            {
                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;
                    objects.AddRange(CopyObjects(canvasObj.Children));
                    canvasObj.Children.Clear();
                }
                
                objects.Add(obj);
            }

            return objects; 
        }
    }
}
