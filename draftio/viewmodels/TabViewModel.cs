using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using draftio.models;
using draftio.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace draftio.viewmodels
{
    public partial class TabViewModel: ObservableObject
    {

        [ObservableProperty]
        List<Node> nodes = new();


        public TabViewModel() 
        {
        }

        [RelayCommand]
        public void AddTab(Node node)
        {
            if(node.Type == NodeType.File)
            {
                Nodes.Add(node);
            }
        }
    }
}
