using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.objects;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<Node> nodes = new();

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
                SelectedFolder = projectManager.CurrentProject.RootFolder;

                Nodes.Add(projectManager.CurrentProject.RootFolder);

                File file = new File();
                file.Name = "New";
                file.ParentNode = projectManager.CurrentProject.RootFolder;
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
                file.ParentNode = projectManager.CurrentProject.RootFolder;
            }
            else
            {
                file.ParentNode = SelectedFolder;
            }
        }

        [RelayCommand]
        public void AddPage()
        {
            File file = new File();
            file.Name = "New";
            if (SelectedFolder != null)
            {
                file.ParentNode = SelectedFolder;
                SelectedFolder.Children.Add(file);
                Nodes.Add(file);
                tabViewModel.AddTab(file);
            }
        }

        [RelayCommand]
        public void AddFolder()
        {
            Folder folder = new Folder();
            folder.Name = "Folder";
            if(SelectedFolder != null)
            {
                folder.ParentNode = SelectedFolder; ;
                SelectedFolder.Children.Add(folder);
                Nodes.Add(folder);
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
            if(node.Type == models.enums.NodeType.Folder)
            {
                SelectedFolder = (Folder)node;
            }
        }




    }
}
