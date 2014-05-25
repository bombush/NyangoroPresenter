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
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Layout manager
        private Nyangoro.Core.Layout.LayoutManager layoutManager;

        //Windows
        public Window controlWindow { get; private set; }
        public Window presentationWindow { get; private set; }

        //Plugins
        private Nyangoro.Core.Host.PluginHolder plugins;

        private void Nyangoro_Startup(object sender, StartupEventArgs e)
        {
                layoutManager = new Nyangoro.Core.Layout.LayoutManager(controlWindow, presentationWindow);
                plugins = new Nyangoro.Core.Host.PluginHolder();

                InitWindows();
                layoutManager.BuildLayout();
        }

        public void InitWindows()
        {
            Window winControl = new Nyangoro.Core.Host.ControlWindow();
            this.controlWindow = winControl;
            winControl.Show();

            Window winPres = new Nyangoro.Core.Host.PresentationWindow();
            this.presentationWindow = winPres;
            winPres.Show();
        }

        public bool Compose()
        {
            return true;
        }

    }
}
