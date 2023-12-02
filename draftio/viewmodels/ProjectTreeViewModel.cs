using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.viewmodels
{
    public partial class ProjectTreeViewModel: ObservableObject
    {
        [ObservableProperty]
        string? str1;
        [ObservableProperty]
        string? str2;

        [RelayCommand]
        public void pass()
        {
            
        }
    }
}
