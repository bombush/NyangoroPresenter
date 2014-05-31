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
        //Layout manager
        public Nyangoro.Core.Layout.LayoutManager layoutManager { get; private set; }

        //Windows
        public Window controlWindow { get; private set; }
        public Window presentationWindow { get; private set; }

        //Plugins
        public Nyangoro.Core.Host.PluginHolder plugins { get; private set; }

        public IEnumerable<Nyangoro.Interfaces.IService> services { get; private set; }


        private void Nyangoro_Startup(object sender, StartupEventArgs e)
        {
                InitWindows();

                layoutManager = new Nyangoro.Core.Layout.LayoutManager(controlWindow, presentationWindow);
                plugins = new Nyangoro.Core.Host.PluginHolder();
               // services = new Nyangoro.Core.Host.ServiceHolder();

                layoutManager.BuildLayout();
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
