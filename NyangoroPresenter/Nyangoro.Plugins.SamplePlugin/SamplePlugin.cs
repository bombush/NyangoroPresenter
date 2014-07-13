using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.SamplePlugin
{
    public class SamplePlugin : Nyangoro.Plugins.Plugin
    {
        new SamplePluginController controller;

        public SamplePlugin()
        {
            this.presentationRoot = new PresentationRoot();
            this.controlRoot = new ControlRoot();
            this.controller = new SamplePluginController(this, (ControlRoot)this.controlRoot, (PresentationRoot)this.presentationRoot);

            ControlRoot controlRoot = this.controlRoot as ControlRoot;
            controlRoot.SetController(controller);
        }
    }
}
