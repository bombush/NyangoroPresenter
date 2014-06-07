using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    [Export(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public class PluginHolder : MEFHolder<Nyangoro.Interfaces.IPlugin>, Nyangoro.Interfaces.IPluginHolder
    {

        public PluginHolder()
        {
           this.resourcesPath = @"plugins\";
        }
    }
}
