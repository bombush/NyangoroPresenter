using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.Clock
{
    public class Clock : Nyangoro.Plugins.Plugin
    {
        public Clock()
            : base()
        {
            this.presentationRoot = new PresentationRoot();
        }
    }
}
