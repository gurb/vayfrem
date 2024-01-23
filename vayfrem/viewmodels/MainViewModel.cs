using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using vayfrem.models;
using vayfrem.models.dtos;
using vayfrem.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.viewmodels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ProjectManager projectManager;
        private readonly UndoRedoManager undoRedoManager;


        [ObservableProperty]
        private string? _message = "this is a test";

        public VMResponse loadResponse {get; set;}


        public MainViewModel()
        {
            var fileManager = App.GetService<FileManager>();
            projectManager = App.GetService<ProjectManager>();
            undoRedoManager = App.GetService<UndoRedoManager>();


            string[]? args = fileManager.Args;

            loadResponse = LoadProject();
        }


        public VMResponse LoadProject()
        {
            VMResponse response = new VMResponse();

            var res = projectManager.InitializeProject();
            response = res.Result;

            if(response.Success && response.Result != null)
            {
                Project? project = (Project)response.Result;

                foreach (var node in project.Nodes)
                {
                    if(node.Type == models.enums.NodeType.File)
                    {
                        File file = (File)node;

                        // set hardcoded objects for undo/redo management
                        undoRedoManager.SetHardCoded(file.Guid!, file.Objects);
                    }
                }
               
            }

            return response;
        }


    }
}
