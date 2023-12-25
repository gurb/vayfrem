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

            response.Result = loadedData;

            return response;
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
