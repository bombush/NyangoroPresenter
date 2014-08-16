using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nyangoro.Core.Host
{
    /*
     * Layout manager takes care of integrating module outputs into the respective
     * windows.
     */
    [Export(typeof(Nyangoro.Core.Host.LayoutManager))]
    public class LayoutManager
    {
        //path to directory containing screens in XAML
        string screenPath = @"screens\";

        private Window controlWindow;
        private Window presentationWindow;
        private List<Nyangoro.Core.Layout.PresentationScreen> screens;
        public Nyangoro.Core.Layout.PresentationScreen activeScreen { get; private set; }
        public Nyangoro.Core.Layout.ControlScreen controlScreen { get; private set; }
        
        public LayoutManager(Window controlWindow, Window presentationWindow)
        {
            this.controlWindow = controlWindow;
            this.presentationWindow = presentationWindow;
            this.screens = new List<Nyangoro.Core.Layout.PresentationScreen>();
        }

        public void InitLayout()
        {
            this.LoadScreensAvailable();
            this.DisplayDefaultScreen();
            this.DisplayControlScreen();
        }

        public void BuildLayout()
        {
        }

        private void LoadScreensAvailable()
        {
            if (!Directory.Exists(this.screenPath))
                throw new DirectoryNotFoundException("Screens directory not found");

            string[] files = Directory.GetFiles(this.screenPath, "*.xaml");
            if(files.Length == 0)
                throw new IOException("Screens directory is empty");

            foreach(string file in files){
                StreamReader sr = new StreamReader(file);
                FrameworkElement screenRoot = XamlReader.Load(sr.BaseStream) as FrameworkElement;
                Nyangoro.Core.Layout.PresentationScreen screen = new Nyangoro.Core.Layout.PresentationScreen(screenRoot);
                this.screens.Add(screen);
            }
        }

        private void DisplayDefaultScreen()
        {
            this.DisplayScreen(screens[0]);
        }

        private void RemoveCurrentScreen()
        {
            this.presentationWindow.Content = null;
        }

        public void DisplayPlugins(PluginHolder plugins)
        {
            this.activeScreen.DisplayPlugins(plugins);
            this.DisplayControls(plugins);
        }

        public void SwitchToScreen(int screenIndex)
        {
            this.RemoveCurrentScreen();
            this.DisplayScreen(this.screens[screenIndex]);
        }

        //plz REFACTOR!!!
        private void DisplayScreen(Nyangoro.Core.Layout.PresentationScreen screen)
        {
            Viewbox presentationContentRoot = (Viewbox)presentationWindow.Content;
            presentationContentRoot.Child = (FrameworkElement)screen.rootElement;
            this.activeScreen = screen;
        }

        private void DisplayControls(PluginHolder holder)
        {
            foreach(Nyangoro.Interfaces.IPlugin plugin in holder.members){
                this.controlScreen.DisplayPlugin(plugin);
            }
        }

        private void DisplayControlScreen()
        {
            Panel controlRoot = this.controlWindow.Content as Panel;
            if (controlRoot == null)
                throw new Exception("No control root found");

            this.controlScreen = new Layout.ControlScreen(controlRoot);
        }
    }
}
