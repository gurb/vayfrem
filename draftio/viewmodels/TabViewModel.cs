using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.enums;
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


        [ObservableProperty]
        ObservableCollection<Node> nodes = new();

        [ObservableProperty]
        Node? selectedNode;

        public delegate void DrawDelegate();
        public DrawDelegate? drawDelegate;


        public TabViewModel() 
        {
            drawingViewModel = App.GetService<DrawingViewModel>();
        }

        [RelayCommand]
        public void AddTab(Node node)
        {
            if(node.Type == NodeType.File)
            {
                Nodes.Add(node);
                if (drawDelegate != null)
                {
                    drawDelegate.Invoke();
                }
            }
        }

        [RelayCommand]
        public void SetSelected(Node node)
        {
            
            SelectedNode = node;

            if (node.Type == models.enums.NodeType.File)
            {
                drawingViewModel.ChangeFile((File)node);
            }
        }

        [RelayCommand]
        public void SetSelectedOnly(Node node)
        {
            SelectedNode = node;
            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }

        [RelayCommand]
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
            if (drawDelegate != null)
            {
                drawDelegate.Invoke();
            }
        }
    }
}
