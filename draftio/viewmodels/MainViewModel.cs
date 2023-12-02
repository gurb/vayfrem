using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.viewmodels
{
    public partial class MainViewModel: ObservableObject
    {
        [ObservableProperty]
        private string? _message = "this is a test";
    }
}
