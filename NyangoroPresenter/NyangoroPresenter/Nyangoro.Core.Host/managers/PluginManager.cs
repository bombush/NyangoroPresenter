using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Core.Host
{
    public class PluginManager
    {
        private Nyangoro.Core.Host.PluginHolder holder = null;

        public PluginManager(Nyangoro.Core.Host.PluginHolder holder)
        {
            this.holder = holder;
        }
    }
}
