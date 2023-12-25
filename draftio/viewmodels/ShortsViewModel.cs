using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.dtos;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.viewmodels
{
    public partial class ShortsViewModel : ObservableObject
    {
        private readonly FileManager fileManager;
        private readonly IOManager ioManager;
        private readonly ProjectManager projectManager;

        [ObservableProperty]
        bool isSaved = false;

        File? currentFile;


        public delegate void ChangeViewDelegate();
        public ChangeViewDelegate? changeDelegate;

        public ShortsViewModel()
        {
            fileManager = App.GetService<FileManager>();
            ioManager = App.GetService<IOManager>();
            projectManager = App.GetService<ProjectManager>();

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

        public void SetCurrentFile(File? file)
        {
            currentFile = file;
        }


        public VMResponse LoadProject(string data)
        {
            VMResponse response = new VMResponse();

            response = ioManager.LoadProject(data);

            projectManager.CurrentProject = ((SaveProjectData)response.Result!).Project;


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
    }
}
