using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    [Export(typeof(Nyangoro.Interfaces.IServiceHolder))]
    public class ServiceHolder : MEFHolder<Nyangoro.Interfaces.IService>, Nyangoro.Interfaces.IServiceHolder
    {

        public ServiceHolder()
        {
           this.resourcesPath = @"services\";
        }
    }
}
