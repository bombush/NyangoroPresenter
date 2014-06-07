using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Nyangoro.Core.Host
{
    abstract public class MEFHolder<T>
    {
        protected string resourcesPath = "";

        [ImportMany]
        protected IEnumerable<T> members = null;

        public bool Load()
        {
            this.Compose();

            return true;
        }

        public bool Compose()
        {
            string fullResourcesPath = String.Concat(Config.Get("working_dir"),this.resourcesPath);

            DirectoryCatalog catalog = new DirectoryCatalog(this.resourcesPath, "*.dll");
            CompositionContainer container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);

            return true;
        }

        public T getByType(string type)
        {
            foreach(T member in this.members)
            {
                if (member.GetType() == Type.GetType(type))
                {
                    return member;
                }
            }

            return default(T);
        }
    }
}
