using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    class Line
    {
        public double len;
        public int x;
        public int y;
    }

    [Export(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public class PluginHolder : MEFHolder, Nyangoro.Interfaces.IPluginHolder
    {

        [ImportMany(typeof(Nyangoro.Interfaces.IPlugin))]
        new protected IEnumerable<Nyangoro.Interfaces.IPlugin> members;

        public PluginHolder()
        {
            var linez = new List<Line>();
            double length = linez.Where(li => li.len > 10).Count();

           this.resourcesPath = @"plugins\";
        }

        public Nyangoro.Interfaces.IPlugin getByType(string type)
        {
            return base.getByType(type) as Nyangoro.Interfaces.IPlugin;
        }
    }
}
