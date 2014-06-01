using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Nyangoro.Core.Host
{
    abstract public class MEFHolder
    {
        protected string resourcesPath = "";
        protected IEnumerable<object> members = null;

        public bool Load()
        {
            this.Compose();

            return true;
        }

        public bool Compose()
        {
            DirectoryCatalog catalog = new DirectoryCatalog(this.resourcesPath, "*.dll");
            CompositionContainer container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);

            return true;
        }

        public object getByType(string type)
        {
            foreach(object member in this.members)
            {
                if (member.GetType() == Type.GetType(type))
                {
                    return member;
                }
            }

            return null;
        }
    }
}
