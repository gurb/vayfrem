using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.services
{
    public class IOManager
    {
        public IOManager() { }

        public string ReadFile (string path)
        {
            string data = File.ReadAllText(path);

            return data;
        }

        public string WriteFile (string path)
        {
            return "";
        }
    }
}
