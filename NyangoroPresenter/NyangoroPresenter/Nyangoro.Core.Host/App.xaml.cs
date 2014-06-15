using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel.Composition;

namespace Nyangoro.Core.Host
{
    /// <summary>
    /// The main application which holds all the basic components
    /// </summary>
    public partial class App : Application
    {
        //Managers
        public Nyangoro.Core.Host.LayoutManager layoutManager { get; private set; }
        public Nyangoro.Core.Host.PluginManager pluginManager { get; private set; }
        public Nyangoro.Core.Host.ServiceManager serviceManager { get; private set; }

        //Windows
        public Window controlWindow { get; private set; }
        public Window presentationWindow { get; private set; }

        //export services to plugins!
        [Export] 
        public Nyangoro.Core.Host.ServiceHolder services { get; private set; }


        private void Nyangoro_Startup(object sender, StartupEventArgs e)
        {
                Nyangoro.Core.Host.Config.buildConfig();
                this.InitWindows();

                PluginHolder plugins = new Nyangoro.Core.Host.PluginHolder();
                this.services = new Nyangoro.Core.Host.ServiceHolder();
                this.services.Load();
                plugins.Load();

                this.layoutManager = new Nyangoro.Core.Host.LayoutManager(controlWindow, presentationWindow);
                this.pluginManager = new Nyangoro.Core.Host.PluginManager(plugins, layoutManager);
                this.serviceManager = new Nyangoro.Core.Host.ServiceManager(this.services);

                this.layoutManager.InitLayout();
                this.pluginManager.InitPlugins();
                this.pluginManager.DisplayPlugins();
        }


        /*
         * Initializes the control and the presentation window 
         */
        public void InitWindows()
        {
            Window winControl = new Nyangoro.Core.Host.ControlWindow();
            this.controlWindow = winControl;
            winControl.Show();

            Window winPres = new Nyangoro.Core.Host.PresentationWindow();
            this.presentationWindow = winPres;
            winPres.Show();
        }

    }
}
