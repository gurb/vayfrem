using HarfBuzzSharp;
using draftio.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.services
{
    public class TabManager
    {
        public Dictionary<string, Tab>? TabList;
        public Tab? ActiveTab { get; set; }
        private Stack<Guid> openedGuids;

        public TabManager()
        {
            TabList = new Dictionary<string, Tab>();
            openedGuids = new Stack<Guid>();
        }

        public void OpenTabFromFile(File file)
        {
            try
            {
                if (file.TabGuid != null && TabList!.ContainsKey(file.TabGuid))
                {
                    ActiveTab = TabList[file.TabGuid];
                    openedGuids.Push(ActiveTab.Guid);
                }
                else
                {
                    Tab tab = new()
                    {
                        File = file
                    };
                    TabList!.Add(tab.Guid.ToString(), tab);

                    ActiveTab = TabList[tab.Guid.ToString()];
                    openedGuids.Push(ActiveTab.Guid);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void OpenTabFromTab(Tab tab)
        {
            try
            {
                ActiveTab = TabList![tab.Guid.ToString()];
                openedGuids.Push(ActiveTab.Guid);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void CloseTab(Tab tab)
        {
            try
            {
                if (TabList!.ContainsKey(tab.Guid.ToString()))
                {
                    TabList.Remove(tab.Guid.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }   
    }
}