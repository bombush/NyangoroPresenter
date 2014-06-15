using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Nyangoro.Core.Host
{
    [Export(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public class PluginHolder : MEFHolder<Nyangoro.Interfaces.IPlugin>, Nyangoro.Interfaces.IPluginHolder
    {
        public Nyangoro.Core.Host.ServiceHolder services { set; private get; }

        public PluginHolder()
        {
           this.resourcesPath = @"plugins\";
        }

        public Nyangoro.Core.Host.ServiceHolder GetServicesReference()
        {
            return this.services;
        }

        public void InitAll()
        {
            foreach(Nyangoro.Interfaces.IPlugin plugin in this.members)
            {
                plugin.SetHolderReference(this);
                plugin.Init();
            }
        }
    }
}
