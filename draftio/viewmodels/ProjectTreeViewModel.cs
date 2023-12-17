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
        private readonly DrawingViewModel drawingViewModel;

        [ObservableProperty]
        List<Node> nodes = new();

        [ObservableProperty]
        Folder? selectedFolder;

        [ObservableProperty]
        Node selectedNode;


        public ProjectTreeViewModel() {
            projectManager = App.GetService<ProjectManager>();
            tabViewModel = App.GetService<TabViewModel>();
            drawingViewModel = App.GetService<DrawingViewModel>();


            if (projectManager.CurrentProject.RootFolder != null)
            {
                Nodes.Add(projectManager.CurrentProject.RootFolder);

                File file = new File();
                file.Name = "New";
                file.ParentFolder = projectManager.CurrentProject.RootFolder;
                projectManager.CurrentProject.RootFolder.Children.Add(file);

                tabViewModel.AddTab(file);
                
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

        [RelayCommand]
        public void AddPage()
        {
            File file = new File();
            file.Name = "New";
            if (projectManager.CurrentProject.RootFolder != null)
            {
                file.ParentFolder = projectManager.CurrentProject.RootFolder;
                projectManager.CurrentProject.RootFolder.Children.Add(file);
                Nodes.Add(file);
                tabViewModel.AddTab(file);
            }
        }

        [RelayCommand]
        public void SetSelected(Node node)
        {
            if(SelectedNode != null)
            {
                SelectedNode.IsSelected = false;
            }
            SelectedNode = node;
            SelectedNode.IsSelected = true;

            if(node.Type == models.enums.NodeType.File)
            {
                drawingViewModel.ChangeFile((File)node);
            }
        }




    }
}
