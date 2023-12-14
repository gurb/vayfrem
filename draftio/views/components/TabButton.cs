using Avalonia.Controls;
using Avalonia.Media;
using draftio.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.views.components
{
    public class TabButton: Canvas
    {
        Tab tab;
        TextBlock? textBlock;

        public TabButton(Tab tab) 
        {
            this.tab = tab;

            init();
        }

        private void init()
        {
            this.Background = Brushes.DarkGray;

            textBlock = new TextBlock();
            textBlock.Text = tab.GetFileName;

            this.Width = textBlock.Width;
            this.Height = 40;
            
            this.Children.Add(textBlock);
        }
    }
}
