using vayfrem.models;
using vayfrem.models.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace vayfrem.services
{
    public class ProjectManager
    {
        private readonly FileManager fileManager;
        private readonly IOManager ioManager;


        public List<Project> projects = new List<Project>();

        public Project? CurrentProject { get; set; }

        public delegate void SetDimensionDelegate();
        public SetDimensionDelegate? setDimensionDelegate;


        public ProjectManager()
        {
            fileManager = App.GetService<FileManager>();
            ioManager = App.GetService<IOManager>();
        }

        public async Task<VMResponse> InitializeProject()
        {
            VMResponse response = new VMResponse();

            try
            {
                if (fileManager.Args != null && fileManager.Args.Length > 0)
                {
                    Task<VMResponse> res = ioManager.LoadProjectFromStart(fileManager.Args[0]);
                    if (res.Result.Success)
                    {
                        CurrentProject = ((SaveProjectData)res.Result.Result!).Project;
                        projects.Add(CurrentProject!);

                        response.Result = CurrentProject;

                        CurrentProject!.RootFolder = (Folder)CurrentProject.Nodes[0];
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = res.Result.Message;
                    }
                }
                else
                {
                    CurrentProject = new Project();

                    projects.Add(CurrentProject);

                    CurrentProject.RootFolder = new Folder();
                    CurrentProject.RootFolder.Name = "Base";
                    CurrentProject.RootFolder.Guid = Guid.NewGuid().ToString();

                    CurrentProject.Nodes.Add(CurrentProject.RootFolder);
                }
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

                throw new Exception(ex.Message);
            }

            

            return response;
        }
    }
}
