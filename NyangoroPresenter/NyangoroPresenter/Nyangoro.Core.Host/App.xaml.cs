using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using System.Windows.Controls.Primitives;

namespace Nyangoro.Core.Host
{
    /// <summary>
    /// The main application which holds all the basic components
    /// </summary>
    public partial class App : System.Windows.Application
    {
        //Managers
        public Nyangoro.Core.Host.LayoutManager layoutManager { get; private set; }
        public Nyangoro.Core.Host.PluginManager pluginManager { get; private set; }
        public Nyangoro.Core.Host.ServiceManager serviceManager { get; private set; }

        //Windows
        public ControlWindow controlWindow { get; private set; }
        public Window presentationWindow { get; private set; }

        public Nyangoro.Core.Host.ServiceHolder services { get; private set; }


        private void Nyangoro_Startup(object sender, StartupEventArgs e)
        {
                // if no debugger attached, log unhandled exceptions to a file and show in MessageBox
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException_Raised);
                }

                Nyangoro.Core.Host.Config.BuildConfig();
                this.InitWindows();

                this.services = new Nyangoro.Core.Host.ServiceHolder();
                this.services.Load();
                PluginHolder plugins = new Nyangoro.Core.Host.PluginHolder(this.services);
                plugins.Load();

                this.layoutManager = new Nyangoro.Core.Host.LayoutManager(controlWindow, presentationWindow);
                this.pluginManager = new Nyangoro.Core.Host.PluginManager(plugins, layoutManager, this.services);
                this.serviceManager = new Nyangoro.Core.Host.ServiceManager(this.services);

                this.layoutManager.InitLayout();
                this.pluginManager.InitPlugins();
                this.pluginManager.DisplayPlugins();
                this.pluginManager.RunDisplayed();
        }


        /*
         * Initializes the control and the presentation window 
         */
        public void InitWindows()
        {
            this.controlWindow = new Nyangoro.Core.Host.ControlWindow();
            this.controlWindow.PresentationModeToggle += new ControlWindow.PresentationModeToggleEventHandler(this.controlWindow_PresentationModeToggle);
            //when closing take presentation window with you
            this.controlWindow.Closing += (sender, e) => this.presentationWindow.Close();

            controlWindow.Show();

            Window winPres = new Nyangoro.Core.Host.PresentationWindow();
            this.presentationWindow = winPres;
            winPres.Show();
        }

        protected void UnhandledException_Raised(object sender, UnhandledExceptionEventArgs e)
        {
            object s = sender;
            UnhandledExceptionEventArgs ea = e;
            string trace = System.Environment.StackTrace;

           // string msgText = Environment.NewLine + Environment.NewLine + "------------------------------------------";
            string msgText = "";
            msgText += "<Exception>";
            msgText += Environment.NewLine;
            msgText += DateTime.Now;
            msgText += Environment.NewLine + Environment.NewLine;
            msgText += "UNHANDLED EXCEPTION: " + e.ExceptionObject.ToString();
            msgText += Environment.NewLine + Environment.NewLine;
            msgText += "</Exception>";
            msgText += Environment.NewLine;

            try
            {
                StreamWriter sw = new StreamWriter(Config.Get("exception_log"), true);
                sw.Write(msgText);
                sw.Close();
            }
            catch
            {
                System.Windows.MessageBox.Show("Failed to log exception, press CTRL+C to copy: " + Environment.NewLine + msgText);
                Environment.Exit(Environment.ExitCode);
            }

            string boxText = "PRESS CTRL+C TO COPY (or look into the '" + Config.Get("exception_log") + "' file):" + Environment.NewLine + Environment.NewLine;
            boxText += msgText;
            System.Windows.MessageBox.Show(boxText);

            Environment.Exit(Environment.ExitCode);
        }


        #region big event handlers... REFACTORRRR
        private void controlWindow_PresentationModeToggle(object sender, ControlWindow.PresentationModeToggleEventArgs e)
        {
            if (!Screen.AllScreens.Any(sc => !sc.Primary))
            {
                System.Windows.MessageBox.Show("No secondary monitor connected: cannot toggle presentation mode");
                e.toggle.IsChecked = false;
                return;
            }

            if ((bool)e.toggle.IsChecked == true)
                this.StartPresentationMode();
            else
                this.StopPresentationMode();
        }

        private void StartPresentationMode()
        {
            this.FillSecondaryMonitor(this.presentationWindow);
        }

        private void StopPresentationMode()
        {
            this.ToPrimaryMonitor(this.presentationWindow);
        }

        private void FillSecondaryMonitor(Window window)
        {
            if (Screen.AllScreens.Any(sc => !sc.Primary))
            {
                Screen screen = Screen.AllScreens.First(sc => !sc.Primary);
                window.Left = screen.WorkingArea.Left;
                window.Top = screen.WorkingArea.Top;
                window.Width = screen.WorkingArea.Width;
                window.Height = screen.WorkingArea.Height;
            }
        }

        private void ToPrimaryMonitor(Window window)
        {
            if (Screen.AllScreens.Any(sc => sc.Primary))
            {
                Screen screen = Screen.AllScreens.First(sc => sc.Primary);
                window.Left = screen.WorkingArea.Left;
                window.Top = screen.WorkingArea.Top;
            }
        }

        #endregion

    }
}
