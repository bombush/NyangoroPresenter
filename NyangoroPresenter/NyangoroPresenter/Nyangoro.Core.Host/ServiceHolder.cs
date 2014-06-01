using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    [Export(typeof(Nyangoro.Interfaces.IServiceHolder))]
    public class ServiceHolder : MEFHolder, Nyangoro.Interfaces.IServiceHolder
    {
        [ImportMany(typeof(Nyangoro.Interfaces.IService))]
        new protected IEnumerable<Nyangoro.Interfaces.IService> members;

        public ServiceHolder()
        {
           this.resourcesPath = @"services\";
        }

        public Nyangoro.Interfaces.IService getByType(string type)
        {
            return base.getByType(type) as Nyangoro.Interfaces.IService;
        }
    }
}
