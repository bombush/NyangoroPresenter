using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    [Export(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public class PluginHolder : MEFHolder, Nyangoro.Interfaces.IPluginHolder
    {

        [ImportMany(typeof(Nyangoro.Interfaces.IPlugin))]
        new protected IEnumerable<Nyangoro.Interfaces.IPlugin> members;

        public PluginHolder()
        {
           this.resourcesPath = @"plugins\";
        }

        public Nyangoro.Interfaces.IPlugin getByType(string type)
        {
            return base.getByType(type) as Nyangoro.Interfaces.IPlugin;
        }
    }
}
