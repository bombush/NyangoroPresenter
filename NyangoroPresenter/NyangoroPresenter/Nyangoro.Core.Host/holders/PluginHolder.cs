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
        private Nyangoro.Core.Host.ServiceHolder services { set; get; }

        public PluginHolder(Nyangoro.Core.Host.ServiceHolder services) : base()
        {
           this.resourcesPath = @"plugins\";
           this.services = services;
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
                plugin.SetServicesReference(this.services);
                plugin.Init();
            }
        }

        public string GetPluginDir(Nyangoro.Interfaces.IPlugin plugin)
        {
            return Nyangoro.Core.Host.Config.GetPluginDir(plugin.GetName());
        }
    }
}
