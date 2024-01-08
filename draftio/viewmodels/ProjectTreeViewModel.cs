using Avalonia.Controls;
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
        private readonly ShortsViewModel shortsViewModel;
        private readonly PageTreeViewModel pageTreeViewModel;
        private readonly PropertyViewModel propertyViewModel;

        [ObservableProperty]
        ObservableCollection<Node> nodes;

        [ObservableProperty]
        Folder? selectedFolder;

        [ObservableProperty]
        Node? selectedNode;

        public delegate void DrawProjectViewDelegate();
        public DrawProjectViewDelegate? drawProjectView;

        public ProjectTreeViewModel() {
            projectManager = App.GetService<ProjectManager>();
            tabViewModel = App.GetService<TabViewModel>();
            drawingViewModel = App.GetService<DrawingViewModel>();
            shortsViewModel = App.GetService<ShortsViewModel>();
            shortsViewModel.refreshProjectVM += Refresh;
            pageTreeViewModel = App.GetService<PageTreeViewModel>();
            propertyViewModel = App.GetService<PropertyViewModel>();
            
            if (projectManager.CurrentProject.RootFolder != null)
            {
                Refresh();
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
            file.Name = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            file.Guid = Guid.NewGuid().ToString();
            if (SelectedFolder != null)
            {
                file.ParentNode = SelectedFolder;
                file.ParentGuid = SelectedFolder.Guid;
                SelectedFolder.Children.Add(file);
                
                //Nodes.Add(file);
                projectManager.CurrentProject.Nodes.Add(file);
                Nodes = projectManager.CurrentProject.Nodes;
                tabViewModel.AddTab(file);

                shortsViewModel.ChangeSaveState(false);
                shortsViewModel.SetCurrentFile(file);
            }
        }

        [RelayCommand]
        public void AddFolder()
        {
            Folder folder = new Folder();
            folder.Name = "Folder";
            folder.Guid = Guid.NewGuid().ToString();
            if(SelectedFolder != null)
            {
                folder.ParentNode = SelectedFolder;
                folder.ParentGuid = SelectedFolder.Guid;
                SelectedFolder.Children.Add(folder);
                //Nodes.Add(folder);
                projectManager.CurrentProject.Nodes.Add(folder);
                Nodes = projectManager.CurrentProject.Nodes;
            }
        }

        [RelayCommand]
        public void Refresh()
        {
            Nodes = projectManager.CurrentProject.Nodes;
            var node = Nodes.FirstOrDefault(x => x.IsSelected == true);

            if(node != null)
            {
                SetSelected(node);

                if(node.ParentNode != null)
                {
                    SelectedFolder = (Folder)node.ParentNode;
                }
            }

            if (drawProjectView != null)
            {
                drawProjectView.Invoke();
            }
        }


        [RelayCommand]
        public void SetSelectedHover(Node node)
        {
            if (SelectedNode != null)
            {
                SelectedNode.IsSelected = false;
            }
            SelectedNode = node;
            SelectedNode.IsSelected = true;

        }

        [RelayCommand]
        public void SetSelected(Node node)
        {
            if(node == null)
            {
                return;
            }

            if(SelectedNode != null)
            {
                SelectedNode.IsSelected = false;
            }

            SelectedNode = node;
            SelectedNode.IsSelected = true;

            if(node.Type == models.enums.NodeType.File)
            {
                tabViewModel.SetSelectedOnly(node);

                drawingViewModel.ChangeFile((File)node);
                pageTreeViewModel.SetCurrentFile((File)node);

                // if tab nodes does not contain selected node in tab view canvas, add it
                var temp = tabViewModel.Nodes.FirstOrDefault(x => x == node);
                if(temp == null)
                {
                    tabViewModel.AddTab(node);

                    shortsViewModel.ChangeSaveState(false);
                }
                shortsViewModel.SetCurrentFile((File)node);
            }
            if(node.Type == models.enums.NodeType.Folder)
            {
                SelectedFolder = (Folder)node;
            }
        }
    }
}
