using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.enums;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace draftio.viewmodels
{
    public partial class TabViewModel: ObservableObject
    {

        private readonly DrawingViewModel drawingViewModel;
        private readonly ShortsViewModel shortsViewModel;
        private readonly PageTreeViewModel pageTreeViewModel;


        [ObservableProperty]
        ObservableCollection<Node> nodes = new();


        List<Node> lastOpenedNodes = new List<Node>();

        [ObservableProperty]
        Node? selectedNode;

        public delegate void DrawDelegate();
        public DrawDelegate? drawDelegate;


        public TabViewModel() 
        {
            drawingViewModel = App.GetService<DrawingViewModel>();
            shortsViewModel = App.GetService<ShortsViewModel>();
            pageTreeViewModel = App.GetService<PageTreeViewModel>();
        }

        [RelayCommand]
        public void AddTab(Node node)
        {
            if(node.Type == NodeType.File)
            {
                Nodes.Add(node);
                lastOpenedNodes.Add(node);
                
                SetSelected(node);
                SetSelectedOnly(node);
                
            }
        }

        [RelayCommand]
        public void SetSelected(Node node)
        {
            drawingViewModel.SetEmptyState(false);

            SelectedNode = node;

            lastOpenedNodes.Add(node);

            if (node.Type == models.enums.NodeType.File)
            {
                drawingViewModel.ChangeFile((File)node);
                shortsViewModel.SetCurrentFile((File)SelectedNode);
                pageTreeViewModel.SetCurrentFile((File)node);
            }
        }

        [RelayCommand]
        public void SetSelectedOnly(Node node)
        {
            drawingViewModel.SetEmptyState(false);


            SelectedNode = node;

            if(node.Type == NodeType.File)
            {
                shortsViewModel.SetCurrentFile((File)node);
            }
            
            lastOpenedNodes.Add(node);

            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }

        [RelayCommand]
        public void RemoveNode(Node node)
        {
            Predicate<Node> predicate = (x => x == node);

            lastOpenedNodes.RemoveAll(predicate);

            if (node == SelectedNode)
            {
                if(lastOpenedNodes.Count > 0)
                {
                    SetSelectedOnly(lastOpenedNodes.Last());
                }
                else if (Nodes.Count > 0)
                {
                    SetSelectedOnly(Nodes.Last());
                }

            } 
           
            Nodes.Remove(node);
            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }

            if(SelectedNode != null && SelectedNode.Type == NodeType.File)
            {
                drawingViewModel.ChangeFile((File)SelectedNode);
                pageTreeViewModel.SetCurrentFile((File)SelectedNode);
                shortsViewModel.SetCurrentFile((File)SelectedNode);
            } 
            

            if(Nodes.Count == 0)
            {
                // state is empty
                drawingViewModel.SetEmptyState(true);
                pageTreeViewModel.SetCurrentFile(null);
                shortsViewModel.SetCurrentFile(null);
                shortsViewModel.ChangeSaveState(true);
            }
        }
    }
}
