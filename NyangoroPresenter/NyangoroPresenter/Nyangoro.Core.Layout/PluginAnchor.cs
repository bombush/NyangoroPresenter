using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Nyangoro.Core.Layout
{
    public class PluginAnchor : UserControl
    {
        /* 
         * XAML attribute 
         * Type of the plugin to be anchored
         */
        public string PluginType { get; set; }

        public Nyangoro.Interfaces.IPlugin anchoredPlugin { get; set; }


        public void AnchorPlugin(Nyangoro.Interfaces.IPlugin plugin)
        {
            this.anchoredPlugin = plugin;
            this.Content = anchoredPlugin.GetPresentationRoot();
            anchoredPlugin.Display();
        }

        public void RemovePlugin()
        {
            this.anchoredPlugin = null;
            this.Content = null;
        }


        public void RunPlugin()
        {
            if (this.anchoredPlugin != null)
                this.anchoredPlugin.Run();
        }
    }
}
