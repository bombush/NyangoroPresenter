using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    public class PluginHolder
    {
        [ImportMany(typeof(Nyangoro.Interfaces.IPlugin))]
        private IEnumerable<Nyangoro.Interfaces.IPlugin> plugins;

        public PluginHolder()
        {

        }
    }
}
