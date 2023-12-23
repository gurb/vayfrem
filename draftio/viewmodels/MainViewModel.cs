using CommunityToolkit.Mvvm.ComponentModel;
using draftio.services;
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


        public MainViewModel()
        {
            var fileManager = App.GetService<FileManager>();

            string[]? args = fileManager.Args;
        }
    }
}
