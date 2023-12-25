using CommunityToolkit.Mvvm.ComponentModel;
using draftio.models.dtos;
using draftio.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.viewmodels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ProjectManager projectManager;

        [ObservableProperty]
        private string? _message = "this is a test";

        public VMResponse loadResponse {get; set;}


        public MainViewModel()
        {
            var fileManager = App.GetService<FileManager>();
            projectManager = App.GetService<ProjectManager>();

            string[]? args = fileManager.Args;

            loadResponse = LoadProject();
        }


        public VMResponse LoadProject()
        {
            VMResponse response = new VMResponse();

            var res = projectManager.InitializeProject();
            response = res.Result;

            return response;
        }


    }
}
