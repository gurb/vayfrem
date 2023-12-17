using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.objects;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.viewmodels
{
    public partial class ProjectTreeViewModel: ObservableObject
    {
        private readonly ProjectManager projectManager;
        private readonly TabViewModel tabViewModel;


        [ObservableProperty]
        List<Node> nodes = new();

        [ObservableProperty]
        Folder? selectedFolder;


        public ProjectTreeViewModel() {
            projectManager = App.GetService<ProjectManager>();
            tabViewModel = App.GetService<TabViewModel>();


            if (projectManager.CurrentProject.RootFolder != null)
            {
                Nodes.Add(projectManager.CurrentProject.RootFolder);

                File file = new File();
                file.Name = "New";
                file.ParentFolder = projectManager.CurrentProject.RootFolder;
                projectManager.CurrentProject.RootFolder.Children.Add(file);

                tabViewModel.AddTab(file);
                File test = new File();
                tabViewModel.AddTab(test);
            }
        }


        [RelayCommand]
        public void CreatePage()
        {
            File file = new File();
            file.Name = "New";

            if(SelectedFolder == null)
            {
                file.ParentFolder = projectManager.CurrentProject.RootFolder;
            }
            else
            {
                file.ParentFolder = SelectedFolder;
            }
        }
    }
}
