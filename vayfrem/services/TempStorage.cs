using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.services
{
    public class TempStorage
    {
        public Dictionary<string, object> Storage { get; set; }

        public TempStorage() 
        {
            Storage = new Dictionary<string, object>();
        }

        public void AddTempData(string key, object val)
        {
            if(!Storage.ContainsKey(key))
            {
                Storage.Add(key, val);
            }
        }

        public object? GetTempData(string key)
        {
            if(Storage.ContainsKey(key))
            {
                return Storage[key];
            }
            return null;
        }

    }
}
