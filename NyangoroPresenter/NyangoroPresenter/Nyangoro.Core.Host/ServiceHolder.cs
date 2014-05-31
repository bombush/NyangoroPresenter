using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    class ServiceHolder : MEFHolder
    {
        [ImportMany(typeof(Nyangoro.Interfaces.IService))]
        private IEnumerable<Nyangoro.Interfaces.IService> services;

        [Export(typeof(Nyangoro.Interfaces.IServiceHolder))]
        public ServiceHolder()
        {

        }
    }
}
