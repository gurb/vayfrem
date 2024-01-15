using draftio.models.dtos;
using draftio.viewmodels;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using draftio.models;
using draftio.models.objects;
using draftio.models.objects.@base;
using System.Collections.ObjectModel;

namespace draftio.services
{
    public class IOManager
    {
        private readonly EncodeManager encodeManager;

        public IOManager() 
        {
            encodeManager = App.GetService<EncodeManager>();
        }

        public VMResponse SaveFile (SaveProjectData data)
        {
            VMResponse response = new VMResponse();

            try
            {
                var options = new JsonSerializerOptions
                {
                        WriteIndented = true,
                };

                string jsonString = JsonSerializer.Serialize<Project>(data.Project!, options);
                //string encryptJsonData = encodeManager.Encode(jsonString);

                response.Result = jsonString;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;  
            }

            return response;
        }

        public VMResponse SaveAllFile ()
        {
            VMResponse response = new VMResponse();

            return response;
        }
        public VMResponse LoadProject(string data)
        {
            VMResponse response = new VMResponse();

            SaveProjectData loadedData = new SaveProjectData();
            loadedData.Project = JsonSerializer.Deserialize<Project>(data);

            HandleSaveData(loadedData.Project);


            response.Result = loadedData;

            return response;
        }

        // after loading
        private void HandleSaveData(Project? project)
        {
            if (project != null)
            {
                foreach (var node in project.Nodes)
                {

                    node.Children = new ObservableCollection<Node>(project.Nodes.Where(n => n.ParentGuid == node.Guid));

                    if(node.ParentGuid != null)
                    {
                        var parentNode = project.Nodes.FirstOrDefault(n => n.Guid == node.ParentGuid);

                        if(parentNode != null)
                        {
                            node.ParentNode = parentNode;
                        }
                    }

                    if(node.Type == models.enums.NodeType.File)
                    {
                        models.File file = (models.File)node;

                        foreach(var obj in file.Objects)
                        {
                            if (obj.ObjectType == models.enums.ObjectType.Canvas)
                            {
                                CanvasObj canvasObj = (CanvasObj)obj;

                                if(canvasObj.Children != null && canvasObj.Children.Count > 0)
                                {
                                    IterateChildren(canvasObj);
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                throw new Exception("Not found project");
            }
        }

        private void IterateChildren (CanvasObj parent)
        {
            foreach(var obj in parent.Children)
            {
                obj.Parent = parent;

                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    if (canvasObj.Children != null && canvasObj.Children.Count > 0)
                    {
                        IterateChildren(canvasObj);
                    }
                }
            }
        }


        // this method is used by ProjectManager from start of custom file (*.gdraft)
        public async Task<VMResponse> LoadProjectFromStart(string path)
        {
            VMResponse response = new VMResponse();
            try
            {
                if(System.IO.File.Exists(path))
                {
                    using (StreamReader reader = System.IO.File.OpenText(path))
                    {
                        string jsonString = reader.ReadToEnd();
                        SaveProjectData loadedData = new();

                        loadedData.Project = JsonSerializer.Deserialize<Project>(jsonString);

                        HandleSaveData(loadedData.Project);

                        response.Result = loadedData;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Cannot found the file by giving path";
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
