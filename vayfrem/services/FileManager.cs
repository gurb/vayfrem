using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.services
{
    public class FileManager
    {

        private string[]? args;

        public string[]? Args 
        { 
            get
            {
                return args;
            } 
        }
       
        public FileManager() 
        {
            args = new string[0];
        }

        public void SetArgs(string[]? args)
        {
            this.args = args;
        }
    }
}
