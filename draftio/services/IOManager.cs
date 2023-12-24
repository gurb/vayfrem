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
        public VMResponse LoadFile()
        {
            VMResponse response = new VMResponse();

            return response;
        }
    }
}
