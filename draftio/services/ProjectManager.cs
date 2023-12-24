using draftio.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.services
{
    public class ProjectManager
    {
        public List<Project> projects = new List<Project>();

        public Project CurrentProject { get; set; }

        public ProjectManager() 
        { 
            CurrentProject = new Project();

            projects.Add(CurrentProject);

            CurrentProject.RootFolder = new Folder();
            CurrentProject.RootFolder.Name = "Base";

            CurrentProject.Nodes.Add(CurrentProject.RootFolder);
        }

    }
}
