using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Core.Host
{
    public class ServiceManager
    {
        public Nyangoro.Core.Host.ServiceHolder services { get; private set; }

        public ServiceManager(Nyangoro.Core.Host.ServiceHolder serviceHolder)
        {
            this.services = serviceHolder;
        }
    }
}
