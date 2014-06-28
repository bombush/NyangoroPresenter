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
        public IEnumerable<T> members{get; protected set;}

        public bool Load()
        {
            this.Compose();

            return true;
        }

        public bool Compose()
        {
            string fullResourcesPath = this.GetFullResourcesPath(this.resourcesPath);

            DirectoryCatalog catalog = new DirectoryCatalog(this.resourcesPath, "*.dll");
            CompositionContainer container = new CompositionContainer(catalog);

            container.SatisfyImportsOnce(this);

            return true;
        }

        public T GetByType(string type)
        {
            foreach(T member in this.members)
            {
                if (member.GetType().Name == type)
                {
                    return member;
                }
            }

            return default(T);
        }

        /*
         * Gets full resources path. MEF uses paths relative to the location
         * of the .exe by default. We need to set an absolute path
         * in order to be able to use a different working directory for imports.
         */
        protected string GetFullResourcesPath(string relativePath)
        {
            return String.Concat(Config.Get("working_dir"), relativePath);
        }
    }
}
