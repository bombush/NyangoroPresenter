using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Core.Host
{
    public class PluginManager
    {
        private Nyangoro.Core.Host.PluginHolder holder = null;
        private Nyangoro.Core.Host.LayoutManager layoutManager = null;

        public PluginManager(Nyangoro.Core.Host.PluginHolder holder, Nyangoro.Core.Host.LayoutManager layoutManager)
        {
            this.holder = holder;
            this.layoutManager = layoutManager;
        }

        public void InitPlugins()
        {
            holder.InitAll();
        }

        public void DisplayPlugins()
        {
            layoutManager.DisplayPlugins(this.holder);
        }

        public void RunDisplayed()
        {
            List<Nyangoro.Core.Layout.PluginAnchor> anchors = this.layoutManager.activeScreen.anchors;
            foreach (Nyangoro.Core.Layout.PluginAnchor anchor in anchors)
            {
                anchor.anchoredPlugin.Run();
            }
        }
    }
}
